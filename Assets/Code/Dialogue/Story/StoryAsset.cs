using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Code.Logger;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Code.Dialogue.Story
{
    /// <summary>
    /// Reloads the properties of the Story assets
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">10.05.2023</para>
    [CreateAssetMenu(fileName = "Chapter", menuName = "Viewer", order = 0)]
    public class StoryAsset : ScriptableObject, ISerializationCallbackReceiver
    {
        // Object
        private static Object[] _storyAssets;
        private static Object[] _storyFiles;
        
        // Logger
        private readonly GameLogger _logger = new GameLogger("Story");
        // Lists with nodes
        private static readonly List<StoryNode> Nodes = new ();
        [field: SerializeField] public List<StoryNode> StoryNodes { get; private set; } = new();
        // Dictionary to store nodes 
        [NonSerialized] private readonly Dictionary<string, StoryNode> _nodeLookup = new ();
        
        #region OnValidate
        
        /// <summary>
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
        
        #endregion

        #region Create, Add Nodes

        /// <summary>
        /// Creates a new Node and a unique GUID
        /// saves the label, id, text and type of the node
        /// </summary>
        /// <param name="node">Node name</param>
        /// <param name="id">id Attribute in xml File</param>
        /// <param name="isChoice">Declares if Node is a choice or not</param>
        /// <returns>new Node</returns>
        private static StoryNode CreateNode(XmlNode node, string id, bool isChoice)
        {
            var child = CreateInstance<StoryNode>();
            child.name = Guid.NewGuid().ToString();
            if (node == null) return child;
            child.SetLabelAndNodeId(node.Name, id);
            child.SetText(node.InnerText);
            child.SetChoiceNode(isChoice);

            return child;
        }
        
        /// <summary>
        /// Add node to Node List
        /// </summary>
        /// <param name="node">Child Node to add</param>
        private void AddNodeToList(StoryNode node)
        {
            Nodes.Add(node);
            OnValidate();
        }

        #endregion
        
        #region Reload StoryAssets
        
        /// <summary>
        /// Loads all Story Assets
        /// </summary>
        public static void LoadStoryObjects()
        {
            _storyAssets = Resources.LoadAll($@"Story/", typeof(StoryAsset));
            _storyFiles = Resources.LoadAll($@"StoryFiles/", typeof(TextAsset));
        }
        
        /// <summary>
        /// Reloads all Story Assets
        /// </summary>
        public static void ReloadStoryAssets()
        {
            foreach (var s in _storyAssets)
            {
                var story = Resources.Load<StoryAsset>(s.name);
                ReadNodes(story);
            }
        }

        #endregion
        
        #region ReadNodes and Properties
        public static void ReadNodes(StoryAsset chapter)
        {
            chapter.EmptyNodes();
            
            var xmlDoc = new XmlDocument();
            var xmlFile = Resources.Load<TextAsset>($"StoryFiles/{chapter.name}");
            xmlDoc.LoadXml(xmlFile.text);
            
            var choiceNodes = xmlDoc.GetElementsByTagName("Choice");
            foreach (XmlNode choice in choiceNodes)
            {
                var id = choice.Attributes?[0].Value;
                var node = CreateNode(choice, id, true);
                chapter.AddNodeToList(node);
            }

            var storyNodes = xmlDoc.GetElementsByTagName("Node");
            foreach (XmlNode story in storyNodes)
            {
                var id = story.Attributes?[0].Value;
                var node = CreateNode(story, id, false);
                chapter.AddNodeToList(node);
            }

            foreach (var node in chapter.GetAllNodes())
                ReadProperties(node, xmlDoc);
            
            chapter.SetStoryNodes();   
        }

        /// <summary>
        /// Sets the Properties from the Xml to the according node
        /// </summary>
        /// <param name="node"></param>
        /// <param name="xmlDoc"></param>
        private static void ReadProperties(StoryNode node, XmlDocument xmlDoc)
        {
            var nodeList = node.IsChoiceNode()? xmlDoc.GetElementsByTagName("Choice"): xmlDoc.GetElementsByTagName("Node");
            var xmlNode = nodeList.Cast<XmlNode>().FirstOrDefault(n => node.GetText() == n.InnerText);
            int? attributesCount = xmlNode?.Attributes?.Count;

            if (attributesCount == 0) return;
            
            for (var i = 0; i < attributesCount; i++)
            {
                var attribute = xmlNode.Attributes?[i].Name;
                switch (attribute)
                {
                    case "node":
                        AddStoryChild(node, xmlNode.Attributes[attribute].Value);
                        break;
                    case "choice":
                        AddStoryChild(node, xmlNode.Attributes[attribute].Value);
                        break;
                    case "image":
                        node.SetImage(xmlNode.Attributes[attribute].Value);
                        break;
                    case "item":
                        node.SetItem(xmlNode.Attributes[attribute].Value);
                        break;
                    case "isRootNode":
                        node.SetIsRootNode(Convert.ToBoolean(xmlNode.Attributes[attribute].Value));
                        break;
                    case "isGameOver":
                        node.SetIsGameOver(Convert.ToBoolean(xmlNode.Attributes[attribute].Value));
                        break;
                    case "isEndOfChapter":
                        node.SetIsEndOfChapter(Convert.ToBoolean(xmlNode.Attributes[attribute].Value));
                        break;
                    case "isEndOfStory":
                        node.SetIsEndOfStory(Convert.ToBoolean(xmlNode.Attributes[attribute].Value));
                        break;
                    case "background":
                        node.SetBackground(xmlNode.Attributes[attribute].Value);
                        break;
                }
            }
            
            foreach (var child in Nodes)
            {
                if (child.IsRootNode())
                {
                    child.SetTextRect(child.GetRect().x + 5, child.GetRect().y + 5, child.GetRect().width - 50, child.GetRect().height -50);
                    SetNodePosition(child);
                    break;
                }
            }
        }

        /// <summary>
        /// Adds the Child/s to the parent Node
        /// </summary>
        private static void AddStoryChild(StoryNode node, string id)
        {
            var ids = id.Split(',');
            foreach (var i in ids)
            {
                foreach (var n in Nodes)
                {
                    if (n.GetNodeId() != i)continue;
                    node.AddChildNode(n.name);
                    break;
                }
            }
        }

        /// <summary>
        /// Sets the Position of all nodes
        /// </summary>
        /// <param name="node"></param>
        private static void SetNodePosition(StoryNode node)
        {
            var children = node.GetChildNodes();
            if (children.Count == 0) return;

            for (var i = 0; i < children.Count; i++)
            {
                var child = node.GetChildNodes()[i];
                foreach (var n in Nodes)
                {
                    if (n.name != child) continue;
                    n.SetRect(node.GetRect().position.x + 350, node.GetRect().position.y + (i * 200));
                    n.SetTextRect(n.GetRect().x + 5, n.GetRect().y + 5, n.GetRect().width - 20, n.GetRect().height -20);
                    SetNodePosition(n);
                    break;
                }
            }
        }
        
        #endregion
        
        #region Getter and Setter

        /// <summary>
        /// 
        /// </summary>
        private void SetStoryNodes() {
            StoryNodes = Nodes;
        }

        /// <summary>
        /// Clears the nodelist
        /// </summary>
        private void EmptyNodes()
        {
            Nodes.Clear();
        }
        
        /// <summary>
        /// Return the List of DialogueNodes
        /// </summary>
        /// <returns>nodes</returns>
        public IEnumerable<StoryNode> GetAllNodes()
        {
            return Nodes;
        }

        /// <summary>
        /// Returns all Child nodes from the Parent node
        /// </summary>
        /// <param name="parentNode">Parent Node to get the child nodes from</param>
        /// <returns>Child nodes of the parent Node</returns>
        public List<StoryNode> GetAllChildNodes(StoryNode parentNode)
        {
            var childNodes = new List<StoryNode>();
            foreach (var childID in parentNode.GetChildNodes())
            {
                // Checks the Node Dictionary if there is a key with this id
                if (_nodeLookup.ContainsKey(childID))
                    childNodes.Add(_nodeLookup[childID]);
            }
            return childNodes;
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

        #endregion
        
        #region AssetDatabase
        
        /// <summary>
        /// Creates the First node and adds it to the Node List
        /// Adds nodes to Asset Database
        /// Method from ISerializationCallbackReceiver, Interface
        /// </summary>
        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            // Create an Empty Node when there is none
            if (Nodes.Count == 0)
                AddNodeToList(CreateNode(null, "",false));
            if (GetAllNodes().Any()) return;
            
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
        
        #endregion
    }
}