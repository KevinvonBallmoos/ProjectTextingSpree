using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Newtonsoft.Json;
using UnityEngine;
using Formatting = Newtonsoft.Json.Formatting;

namespace Code.Dialogue.Story
{
    /// <summary>
    /// Reads the properties of the Story assets
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">10.05.2023</para>
    [CreateAssetMenu(fileName = "Chapter", menuName = "StoryViewer", order = 0)]
    public class StoryAsset : ScriptableObject
    {
        // Current Asset
        private StoryAsset _currentAsset;

        // Lists with nodes
        private readonly List<NodeInfo> _nodes = new();

        [field: SerializeField] public List<StoryNode> StoryNodes { get; private set; } = new();
        
        // Boolean that is true when the nodes have been read
        [NonSerialized] public bool HasReadNodes;

        // Is needed to evaluate if a node needs to be added or removed
        private class NodeInfo
        {
            public string NodeId { get; set; }
            public StoryNode Node { get; set; }
            public bool IsTrue { get; set; }
        }

        #region ReadNodes and Properties

        /// <summary>
        /// Reads the Nodes from the Xml File and puts them in the right order
        /// </summary>
        /// <param name="chapter">Chapter to be read in</param>
        public StoryAsset ReadNodes(StoryAsset chapter)
        {
            HasReadNodes = false;

            if (StoryNodes.Count != 0)
            {
                if (StoryNodes[0] == null)
                {
                    var json = File.ReadAllText(
                        Application.persistentDataPath + $"/StoryAssets/" + chapter.name + ".json",
                        Encoding.UTF8);
                    var array = JsonConvert.DeserializeObject<StoryNodeDataProperty[]>(json);

                    StoryNodes.Clear();
                    if (array != null)
                        foreach (var n in array)
                        {
                            var node = CreateInstance<StoryNode>();
                            node.InitializeStoryNode(n);
                            _nodes.Add(new NodeInfo { Node = node, IsTrue = true });
                        }
                }
            }

            var xmlDoc = new XmlDocument();
            var filePath = Path.Combine(Application.streamingAssetsPath, $"StoryFiles/{chapter.name}.xml");
            var xmlFile = File.ReadAllText(filePath);
            xmlDoc.LoadXml(xmlFile);

            var choiceNodes = xmlDoc.GetElementsByTagName("Choice");
            foreach (XmlNode choice in choiceNodes)
            {
                var id = choice.Attributes?[0].Value;
                if (CheckNodes(id)) continue;
                var node = CreateNode(choice, id, true);
                _nodes.Add(new NodeInfo { NodeId = id, Node = node, IsTrue = true });
            }

            var storyNodes = xmlDoc.GetElementsByTagName("Node");
            foreach (XmlNode story in storyNodes)
            {
                var id = story.Attributes?[0].Value;
                if (CheckNodes(id)) continue;
                var node = CreateNode(story, id, false);
                _nodes.Add(new NodeInfo { NodeId = id, Node = node, IsTrue = true });
            }

            var i = 0;
            while (i < _nodes.Count)
            {
                if (_nodes[i].IsTrue)
                {
                    i++;
                    continue;
                }

                foreach (var n in _nodes)
                {
                    if (n.Node.ChildNodes.Contains(_nodes[i].Node.name))
                        n.Node.RemoveChildNode(_nodes[i].Node.name);
                }

                _nodes.RemoveAt(i);
            }

            StoryNodes.Clear();
            foreach (var n in _nodes)
            {
                ReadProperties(n.Node, xmlDoc);
                StoryNodes.Add(n.Node);
            }

            foreach (var child in _nodes)
            {
                //child.Node.StoryRect = new Rect(10, 10, 300, 180);
                if (!child.Node.IsRootNode) continue;
                child.Node.TextRect = new Rect(child.Node.StoryRect.x + 5, child.Node.StoryRect.y + 5,
                    child.Node.StoryRect.width - 50, child.Node.StoryRect.height - 50);
                SetNodePosition(child.Node);
                break;
            }

            _currentAsset = chapter;
            
            SaveNodesToAssetDatabase();

            HasReadNodes = true;
            return _currentAsset;
        }

        /// <summary>
        /// Checks if the node already exists 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool CheckNodes(string id)
        {
            foreach (var n in _nodes)
            {
                if (n.Node.NodeId == id)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Reads the Properties from the Xml to the according node
        /// </summary>
        /// <param name="node">Node whose properties must be read</param>
        /// <param name="xmlDoc">The currently opened xml document</param>
        private void ReadProperties(StoryNode node, XmlDocument xmlDoc)
        {
            var nodeList = node.IsChoiceNode
                ? xmlDoc.GetElementsByTagName("Choice")
                : xmlDoc.GetElementsByTagName("Node");
            var xmlNode = nodeList.Cast<XmlNode>().FirstOrDefault(n => node.Text == n.InnerText);
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
                        node.Image = xmlNode.Attributes[attribute].Value;
                        break;
                    case "item":
                        node.Item = xmlNode.Attributes[attribute].Value;
                        break;
                    case "isRootNode":
                        node.IsRootNode = Convert.ToBoolean(xmlNode.Attributes[attribute].Value);
                        break;
                    case "isGameOver":
                        node.IsGameOver = Convert.ToBoolean(xmlNode.Attributes[attribute].Value);
                        break;
                    case "isEndOfChapter":
                        node.IsEndOfChapter = Convert.ToBoolean(xmlNode.Attributes[attribute].Value);
                        break;
                    case "isEndOfPart":
                        node.IsEndOfPart = Convert.ToBoolean(xmlNode.Attributes[attribute].Value);
                        break;
                    case "background":
                        node.Background = xmlNode.Attributes[attribute].Value;
                        break;
                }
            }
        }

        /// <summary>
        /// Adds the Child/s to the parent Node
        /// </summary>
        private void AddStoryChild(StoryNode node, string id)
        {
            var ids = id.Split(',');
            foreach (var i in ids)
            {
                foreach (var n in _nodes)
                {
                    if (n.Node.NodeId != i) continue;
                    node.AddChildNode(n.Node.name);
                    break;
                }
            }
        }

        /// <summary>
        /// Sets the Position of all nodes
        /// </summary>
        /// <param name="node"></param>
        private void SetNodePosition(StoryNode node)
        {
            var children = node.ChildNodes;
            if (children.Count == 0) return;

            for (var i = 0; i < children.Count; i++)
            {
                var child = node.ChildNodes[i];
                foreach (var n in _nodes)
                {
                    if (n.Node.name != child) continue;
                    // ReSharper disable once CompareOfFloatsByEqualityOperator

                    if (n.Node.StoryRect.position.x != 10) continue;
                    n.Node.StoryRect = new Rect(node.StoryRect.position.x + 350, node.StoryRect.position.y + (i * 200), node.StoryRect.width, node.StoryRect.height);
                    n.Node.TextRect = new Rect(n.Node.StoryRect.x + 5, n.Node.StoryRect.y + 5, n.Node.StoryRect.width - 20,
                        n.Node.StoryRect.height - 20);
                    SetNodePosition(n.Node);
                    break;
                }
            }
        }

        #endregion

        #region Create, Add Nodes

        /// <summary>
        /// Creates a new Node and a unique GUID
        /// Saves the label, id, text and type of the node
        /// </summary>
        /// <param name="node">Node name</param>
        /// <param name="id">Id Attribute in xml File</param>
        /// <param name="isChoice">Declares if Node is a choice or not</param>
        /// <returns>new Node</returns>
        private static StoryNode CreateNode(XmlNode node, string id, bool isChoice)
        {
            var child = CreateInstance<StoryNode>();
            child.name = Guid.NewGuid().ToString();
            child.NodeId = id;
            child.LabelText = node.Name;
            child.Text = node.InnerText;
            child.IsChoiceNode = isChoice;

            return child;
        }

        #endregion

        #region Getter and Setter

        /// <summary>
        /// Return the List of DialogueNodes
        /// </summary>
        /// <returns>nodes</returns>
        public IEnumerable<StoryNode> GetAllNodes()
        {
            return StoryNodes;
        }

        /// <summary>
        /// Returns all StoryNodes
        /// </summary>
        /// <returns></returns>
        public IEnumerable<StoryNode> GetAllStoryNodes()
        {
            foreach (var node in StoryNodes)
            {
                if (!node.IsChoiceNode)
                    yield return node;
            }
        }

        /// <summary>
        /// Returns all Child nodes from the Parent node
        /// </summary>
        /// <param name="parentNode">Parent Node to get the child nodes from</param>
        /// <returns>Child nodes of the parent Node</returns>
        public List<StoryNode> GetAllChildNodes(StoryNode parentNode)
        {
            var childNodes = new List<StoryNode>();
            foreach (var nodeId in parentNode.ChildNodes)
                foreach (var node in _nodes)
                {
                    if (node.Node.name.Equals(nodeId))
                        childNodes.Add(node.Node);
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
                if (node.IsRootNode)
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
                if (child.IsChoiceNode)
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
                if (!child.IsChoiceNode)
                    yield return child;
            }
        }

        #endregion

        #region AssetDatabase
        /// <summary>
        /// Saves the nodes as a JSON File
        /// </summary>
        private void SaveNodesToAssetDatabase()
        {

            var jsonArray = new StoryNodeData[_nodes.Count];
            for (int i = 0;  i < _nodes.Count;i++)
                jsonArray[i] = new StoryNodeData(_nodes[i].Node);
            
            var json = JsonConvert.SerializeObject(jsonArray, Formatting.Indented);
            var filename = Application.persistentDataPath + "/StoryAssets";
            Directory.CreateDirectory(filename);
            
            filename += $"/{_currentAsset.name}.json";
            File.WriteAllText(filename, json);
        }
        #endregion
    }
}