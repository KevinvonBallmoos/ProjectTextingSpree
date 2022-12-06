using System;
using System.Collections.Generic;
using UnityEngine;

using Code.Logger;
using UnityEditor;

namespace Code.Dialogue.Story
{
    /// <summary>
    /// Creates the Story in the Editor
    /// </summary>
    /// <para name="author">Kevin von Ballmoos></para>
    /// <para name="date">04.12.2022</para>
    [CreateAssetMenu(fileName = "Chapter", menuName = "Chapter", order = 0)]
    public class Story : ScriptableObject, ISerializationCallbackReceiver
    {
        // Logger
        private readonly GameLogger _logger = new GameLogger("StoryEditor");
        // List with nodes
        [SerializeField] public List<StoryNode> nodes = new List<StoryNode>();
        // Vector 2
        [SerializeField] private Vector2 newNodeOffset = new Vector2(250, 0);
        // Dictionary to store nodes 
        [NonSerialized] private readonly Dictionary<string, StoryNode> _nodeLookup = new Dictionary<string, StoryNode>();

#if UNITY_EDITOR
        
        /// <summary>
        /// Is called everytime a Scriptable Object is loaded
        /// ## When exporting the Game OnValidate() does not get called automatically, so it will be called from Awake 
        /// </summary>
        private void Awake()
        {
            //OnValidate();
        }
        
#endif
        
        /// <summary>
        /// When a value is changed in the Inspector
        /// or when a scriptable Object is loaded
        /// </summary>
        private void OnValidate()
        {
            _nodeLookup.Clear();
            foreach (var node in GetAllNodes())
            {
                if (node != null)
                    _nodeLookup[node.name] = node;
            }
        }
        
        /// <summary>
        /// Return the List of DialogueNodes
        /// </summary>
        /// <returns>nodes</returns>
        public IEnumerable<StoryNode> GetAllNodes()
        {
            return nodes;
        }

        /// <summary>
        /// Returns RootNode
        /// </summary>
        /// <returns>if found rootNode else null</returns>
        public StoryNode GetRootNode()
        {
            foreach (var node in GetAllNodes())
            {
                if (node.IsRootNode())
                    return node;
            }
            return null;
        }

        /// <summary>
        /// Returns all Child nodes from the Parent node
        /// </summary>
        /// <param name="parentNode">Parent Node to get the child nodes from</param>
        /// <returns>Child nodes of the parent Node</returns>
        public IEnumerable<StoryNode> GetAllChildNodes(StoryNode parentNode)
        {
            foreach (var childID in parentNode.GetChildNodes())
            {
                // Checks the Node Dictionary if there is a key with this id
                if (_nodeLookup.ContainsKey(childID))
                {
                    yield return _nodeLookup[childID];
                }
            }
        }
        
        /// <summary>
        /// Returns all Choice nodes from current node
        /// </summary>
        /// <param name="currentNode">Current Node to get the choice nodes from</param>
        /// <returns>All choice nodes of the current Node</returns>
        public IEnumerable<StoryNode> GetChoiceNodes(StoryNode currentNode)
        {
            foreach (var child in GetAllChildNodes(currentNode))
            {
                if (child.IsChoiceNode())
                    yield return child;
            }
        }
        
        /// <summary>
        /// Returns all Story nodes from current node
        /// </summary>
        /// <param name="currentNode">Current Node to get the story Nodes from</param>
        /// <returns>All story nodes of the current Node</returns>
        public IEnumerable<StoryNode> GetStoryNodes(StoryNode currentNode)
        {
            foreach (var child in GetAllChildNodes(currentNode))
            {
                if (!child.IsChoiceNode())
                    yield return child;
            }
        }

#if UNITY_EDITOR

        /// <summary>
        /// Adds new Node and adds it to the Node List
        /// </summary>
        /// <param name="parentNode">Parent Node to add the child Node</param>
        public void AddNode(StoryNode parentNode)
        {
            var child = CreateNode(parentNode);
            Undo.RegisterCreatedObjectUndo(child, "Created Dialogue Node");
            
            Undo.RecordObject(this, "Added Dialogue Node");
            AddNodeToList(child);
        }
        
        /// <summary>
        /// Adds new Node and adds it to the Node List
        /// Overload #1 takes an additional parameter to determine if it is a Choice Node or a Story Node
        /// This and the Overload of MakeNode() is needed that the nodes are not generated alternately
        /// </summary>
        /// <param name="parentNode">Parent Node to add the child Node</param>
        /// <param name="isChoice">Declares if Node is a choice or not</param>
        public void AddNode(StoryNode parentNode, bool isChoice)
        {
            var child = CreateNode(parentNode, isChoice);
            Undo.RegisterCreatedObjectUndo(child, "Created Dialogue Node");
            
            Undo.RecordObject(this, "Added Dialogue Node");
            AddNodeToList(child);
        }
        
        /// <summary>
        /// Delete selected Node
        /// Removes it from the Node List
        /// </summary>
        /// <param name="nodeToDelete">Node to Delete</param>
        public void DeleteNode(StoryNode nodeToDelete)
        {
            Undo.RecordObject(this, "Delete Dialogue Node");
            nodes.Remove(nodeToDelete);
            
            OnValidate();
            
            CleanChildNodes(nodeToDelete);
            Undo.DestroyObjectImmediate(nodeToDelete);
        }

        /// <summary>
        /// Add node to Node List
        /// </summary>
        /// <param name="child">Child Node to add</param>
        private void AddNodeToList(StoryNode child)
        {
            nodes.Add(child);
            OnValidate();
        }

        /// <summary>
        /// Create a new Node and add it to the parent
        /// </summary>
        /// <param name="parentNode">Parent Node to add the child Node</param>
        /// <returns>new child Node</returns>
        public StoryNode CreateNode(StoryNode parentNode)
        {
            StoryNode child = CreateInstance<StoryNode>();
            child.name = Guid.NewGuid().ToString();
            
            if (parentNode != null)
            {
                parentNode.AddChildNode(child.name);
                child.SetChoiceNode(!parentNode.IsChoiceNode());
                child.SetRect(parentNode.GetRect().position + newNodeOffset);
            }
            return child;
        }

        /// <summary>
        /// Create a new Node and add it to the parent
        /// Overload #1 takes an additional parameter to determine if it is a Choice Node or a Story Node
        /// </summary>
        /// <param name="parentNode">Parent Node to add the child Node</param>
        /// <param name="isChoice">Declares if Node is a choice or not</param>
        /// <returns>new child Node</returns>
        public StoryNode CreateNode(StoryNode parentNode, bool isChoice)
        {
            StoryNode child = CreateInstance<StoryNode>();
            child.name = Guid.NewGuid().ToString();

            if (parentNode != null)
            {
                parentNode.AddChildNode(child.name);
                child.SetChoiceNode(isChoice);
                child.SetRect(parentNode.GetRect().position + newNodeOffset);
            }
            return child;
        }
        
        /// <summary>
        /// Deletes referenced ChildNodes
        /// </summary>
        /// <param name="nodeToDelete">Node to delete</param>
        private void CleanChildNodes(StoryNode nodeToDelete)
        {
            foreach (StoryNode node in GetAllNodes())
            {
                node.RemoveChildNode(nodeToDelete.name);
            }
        }
        
#endif
        
        /// <summary>
        /// Creates the First node and adds it to the Node List
        /// Adds nodes to Asset Database
        /// Method from ISerializationCallbackReceiver, Interface
        /// </summary>
        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (nodes.Count == 0)
                AddNodeToList(CreateNode(null));

            // Add new Node to Asset Database
            if (AssetDatabase.GetAssetPath(this) == "") return;
            foreach (var node in GetAllNodes())
            {
                if (AssetDatabase.GetAssetPath(node) == "")
                    AssetDatabase.AddObjectToAsset(node, this);
            }
#endif
        }

        public void OnAfterDeserialize()
        {
        }
    }
}