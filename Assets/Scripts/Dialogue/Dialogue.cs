using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue", order = 0)]
    public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] public List<DialogueNode> nodes = new List<DialogueNode>();

        [SerializeField] private Vector2 newNodeOffset = new Vector2(250, 0);

        // Stores Key as String and node as Dialoguenode
        [NonSerialized]
        private readonly Dictionary<string, DialogueNode> _nodeLookup = new Dictionary<string, DialogueNode>();

#if UNITY_EDITOR
        /// <summary>
        /// Is called everytime a Scriptable Object is loaded
        /// </summary>
        private void Awake()
        {
            // When exporting game OnValidate does not get called, so it will be called from Awake 
            //OnValidate();
        }
#endif

        /// <summary>
        /// When a value is changed in the Inspector
        /// Or when a scriptable Object is loaded
        /// </summary>
        private void OnValidate()
        {
            _nodeLookup.Clear();
            foreach (var node in GetAllNodes())
            {
                // Look for a node
                if (node != null)
                    _nodeLookup[node.name] = node;
            }
        }

        /// <summary>
        /// Return lists of DialogueNodes
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DialogueNode> GetAllNodes()
        {
            return nodes;
        }

        /// <summary>
        /// Gets RootNode
        /// </summary>
        /// <returns></returns>
        public DialogueNode GetRootNode()
        {
            foreach (DialogueNode node in GetAllNodes())
            {
                if (node.IsRootNode())
                    return node;
            }

            return null;
        }

        /// <summary>
        /// Returns child nodes
        /// </summary>
        /// <param name="parentNode"></param>
        /// <returns>node</returns>
        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
        {
            // Loops over every children of this parentNode
            foreach (var childID in parentNode.GetChildren())
            {
                // Checks if Dictionary has a key of this id
                if (_nodeLookup.ContainsKey(childID))
                {
                    yield return _nodeLookup[childID];
                }
            }
        }
        
        /// <summary>
        /// Gets all Children from the current Node
        /// </summary>
        /// <param name="currentNode"></param>
        /// <returns></returns>
        public IEnumerable<DialogueNode> GetPlayerChildren(DialogueNode currentNode)
        {
            foreach (var child in GetAllChildren(currentNode))
            {
                if (child.IsPlayerSpeaking())
                    yield return child;
            }
        }

        public int GetPlayerChildrenCount(DialogueNode rootNode)
        {
            var count = 0;
            foreach (var child in GetAllNodes())
            {
                if (child.IsPlayerSpeaking() && !child.IsRootNode())
                    count++;
            }

            return count;
        }

        /// <summary>
        /// Gets all AI children
        /// </summary>
        /// <param name="currentNode"></param>
        /// <returns></returns>
        public IEnumerable<DialogueNode> GetAIChildren(DialogueNode currentNode)
        {
            foreach (var child in GetAllChildren(currentNode))
            {
                if (!child.IsPlayerSpeaking())
                    yield return child;
            }
        }
        
        public int GetAIChildrenCount(DialogueNode rootNode)
        {
            var count = 0;
            foreach (var child in GetAllNodes())
            {
                if (!child.IsPlayerSpeaking() && !child.IsRootNode())
                    count++;
            }

            return count;
        }
    

#if UNITY_EDITOR

        /// <summary>
        /// Create Nodes
        /// </summary>
        /// <param name="parent"></param>
        public void CreateNode(DialogueNode parent)
        {
            var child = MakeNode(parent);
            Undo.RegisterCreatedObjectUndo(child, "Created Dialogue Node");
            Undo.RecordObject(this, "Added Dialogue Node");
            // Add node to Nodelist
            AddNode(child);
        }
        // EDIT 01.11.22
        // New Method
        /// <summary>
        /// Create Nodes
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="isSpeaking"></param>
        public void CreateNode(DialogueNode parent, bool isSpeaking)
        {
            var child = MakeNode(parent, isSpeaking);
            Undo.RegisterCreatedObjectUndo(child, "Created Dialogue Node");
            Undo.RecordObject(this, "Added Dialogue Node");
            // Add node to Nodelist
            AddNode(child);
        }
        // END
        /// <summary>
        /// Delete selected Node
        /// </summary>
        /// <param name="nodeToDelete"></param>
        public void DeleteNode(DialogueNode nodeToDelete)
        {
            Undo.RecordObject(this, "Delete Dialogue Node");
            nodes.Remove(nodeToDelete);
            OnValidate();
            CleanDanglingChildren(nodeToDelete);
            // Undo Delete Node
            Undo.DestroyObjectImmediate(nodeToDelete);
        }

        private void AddNode(DialogueNode child)
        {
            nodes.Add(child);
            OnValidate();
        }

        private DialogueNode MakeNode(DialogueNode parent)
        {
            // Add 
            DialogueNode child = CreateInstance<DialogueNode>();
            child.name = Guid.NewGuid().ToString();
            
            // Add child to Parent
            if (parent != null)
            {
                parent.AddChildren(child.name);
                child.SetPlayerSpeaking(!parent.IsPlayerSpeaking());
                child.SetRect(parent.GetRect().position + newNodeOffset);
            }

            return child;
        }
        
        // EDIT 01.11.22
        // New Method
        private DialogueNode MakeNode(DialogueNode parent, bool isSpeaking)
        {
            // Add 
            DialogueNode child = CreateInstance<DialogueNode>();
            child.name = Guid.NewGuid().ToString();
            
            // Add child to Parent
            if (parent != null)
            {
                parent.AddChildren(child.name);
                child.SetPlayerSpeaking(isSpeaking);
                child.SetRect(parent.GetRect().position + newNodeOffset);
            }

            return child;
        }
        // END

        /// <summary>
        /// Deletes referenced ChildNodes
        /// </summary>
        /// <param name="nodeToDelete"></param>
        private void CleanDanglingChildren(DialogueNode nodeToDelete)
        {
            foreach (DialogueNode node in GetAllNodes())
            {
                node.RemoveChildren(nodeToDelete.name);
            }
        }
#endif

        /// <summary>
        /// Method from ISerializationCallbackReceiver, Interface
        /// </summary>
        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (nodes.Count == 0)
            {
                var child = MakeNode(null);
                // Add node to Nodelist
                AddNode(child);
            }

            // Add new Node to Asset Database
            //AssetDatabase.AddObjectToAsset(child, this);
            if (AssetDatabase.GetAssetPath(this) == "") return;
            foreach (var node in GetAllNodes())
            {
                if (AssetDatabase.GetAssetPath(node) == "")
                    AssetDatabase.AddObjectToAsset(node, this);
            }
#endif
        }

        /// <summary>
        /// Method from ISerializationCallbackReceiver, Interface
        /// </summary>
        public void OnAfterDeserialize()
        {

        }
    }
}