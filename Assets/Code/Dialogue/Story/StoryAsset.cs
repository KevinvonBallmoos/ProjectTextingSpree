﻿using Formatting = Newtonsoft.Json.Formatting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Code.Controller.FileControllers;
using UnityEngine;

namespace Code.Dialogue.Story
{
    /// <summary>
    /// This class is needed to evaluate if a node needs to be added or removed
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">01.07.2023</para>
    internal class NodeInfo
    {
        public string NodeId { get; set; }
        public StoryNode Node { get; set; }
        public bool IsTrue { get; set; }
    }

    /// <summary>
    /// This class reads the content of the Story xml files
    /// Creates asset files by saving the node Information in Json Files
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
        // List Property of all nodes
        [field: SerializeField] public List<StoryNode> StoryNodes { get; private set; } = new();
        // Boolean that is true when the nodes have been read
        public bool HasReadNodes { get; set; }

        #region ReadNodes and Properties
        
        #region Read Nodes from Json and Xml File

        /// <summary>
        /// Reads the Node Information from the Xml File and puts them in the right order
        /// </summary>
        /// <param name="chapter">The name of the chapter is needed to find the according json file</param>
        public StoryAsset ReadNodes(StoryAsset chapter)
        {
            HasReadNodes = false;
            _currentAsset = chapter;
            var path = Application.persistentDataPath + "/StoryAssets/" + chapter.name + ".json";

            // Check if json file exists
            if (File.Exists(path))
                ReadJsonFile(path);
            
            // Read nodes and properties from xml file
            var xmlDoc = XmlController.GetXmlDocOfStoryFile(chapter.name);
            ReadNodesFromXmlFile(xmlDoc);

            // Set the node position
            SetNodePosition();
            
            // Save to the nodes into a json file
            SaveNodesToJson();

            HasReadNodes = true;
            return _currentAsset;
        }

        /// <summary>
        /// Reads the JsonFile that matches the chapter,
        /// and deserializes each object into a StoryNodeDataProperty
        /// </summary>
        /// <param name="path">The path to the json file</param>
        private void ReadJsonFile(string path)
        {
            // Checks if StoryNodes have been read already
            if (StoryNodes.Count != 0)
            {
                if (StoryNodes[0] == null)
                {
                    var json = File.ReadAllText(path,
                        Encoding.UTF8);
                    var nodeArray = JsonConvert.DeserializeObject<StoryNodeDataProperty[]>(json);
                    
                    if (nodeArray == null) return;
                    foreach (var n in nodeArray)
                    {
                        var node = CreateInstance<StoryNode>();
                        node.InitializeStoryNode(n);
                        _nodes.Add(new NodeInfo { Node = node, IsTrue = false});
                    }
                }
            }
        }

        /// <summary>
        /// Reads the Xml file that matches the chapter and Processes each node
        /// Remove nodes that ore not needed anymore
        /// Reads the properties of the rest of the nodes
        /// </summary>
        /// <param name="xmlDoc">The XmlDocument info of the file to read</param>
        private void ReadNodesFromXmlFile(XmlDocument xmlDoc)
        {
            foreach (XmlNode choice in xmlDoc.GetElementsByTagName("Choice"))
                ProcessNode(choice, true);

            foreach (XmlNode story in xmlDoc.GetElementsByTagName("Node"))
                ProcessNode(story, false);
            
            // Remove nodes that ore not needed anymore
            RemoveUnusedNodes();
            ReadXmlAttributes(xmlDoc);
        }

        /// <summary>
        /// Checks if the nodes exists
        /// if not a new node is created
        /// </summary>
        /// <param name="node">node to create</param>
        /// <param name="isChoice">true if the node is a choice node</param>
        private void ProcessNode(XmlNode node, bool isChoice)
        {
            var id = node.Attributes?[0].Value;
            if (CheckNodes(id)) return;

            var newNode = CreateNode(node, id, isChoice);
            _nodes.Add(new NodeInfo { NodeId = id, Node = newNode, IsTrue = true});
        }
        
        /// <summary>
        /// Checks if the node already exists 
        /// </summary>
        /// <param name="id">the id to check if its existing in the _nodes List</param>
        /// <returns>true when the node exists or false when the node is not in the List</returns>
        private bool CheckNodes(string id)
        {
            foreach (var n in _nodes)
            {
                if (n.Node.NodeId == id)
                {
                    n.IsTrue = true;
                    return true;
                }
            }
            return false;
        }
        
        /// <summary>
        /// // Remove nodes that ore not needed anymore
        /// </summary>
        private void RemoveUnusedNodes()
        {
            int i = 0;
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
        }
        
        #endregion

        #region Node Properties
        
        /// <summary>
        /// Clears the StoryNodes list
        /// Reads every property of the xml file and adds the node to the StoryNodes list
        /// </summary>
        /// <param name="xmlDoc">The XmlDocument info of the file to read</param>
        private void ReadXmlAttributes(XmlDocument xmlDoc)
        {
            StoryNodes.Clear();
            foreach (var n in _nodes)
            {
                ReadProperties(n.Node, xmlDoc);
                StoryNodes.Add(n.Node);
            }
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
        /// <param name="node">node to add a child node</param>
        /// <param name="id">comma separated string of all child node ids</param>
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
        
        #endregion

        #region Node Position

        /// <summary>
        /// Sets the Position of all nodes
        /// </summary>
        private void SetNodePosition()
        {
            foreach (var child in _nodes)
            {
                //child.Node.StoryRect = new Rect(10, 10, 300, 180);
                if (!child.Node.IsRootNode) continue;
                child.Node.TextRect = new Rect(child.Node.StoryRect.x + 5, child.Node.StoryRect.y + 5,
                    child.Node.StoryRect.width - 50, child.Node.StoryRect.height - 50);
                SetNodePosition(child.Node);
                break;
            }
        }

        /// <summary>
        /// Overload of SetNodePosition
        /// Sets the Position of all nodes recursive
        /// </summary>
        /// <param name="node">Node to set the position and its children</param>
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
        
        #endregion

        #region Create, Add Nodes

        /// <summary>
        /// Creates a new Node with a unique GUID
        /// an id, a label, a text and a type of the node (choice or story boolean)
        /// </summary>
        /// <param name="node">Node name</param>
        /// <param name="id">Id Attribute from xml File</param>
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
        /// Getter to get all nodes
        /// </summary>
        /// <returns>Returns the List of DialogueNodes</returns>
        public IEnumerable<StoryNode> GetAllNodes()
        {
            return StoryNodes;
        }

        /// <summary>
        /// Getter to get only the Story nodes
        /// </summary>
        /// <returns>Returns all StoryNodes</returns>
        public IEnumerable<StoryNode> GetAllStoryNodes()
        {
            foreach (var node in StoryNodes)
            {
                if (!node.IsChoiceNode)
                    yield return node;
            }
        }

        /// <summary>
        /// Getter to get all child nodes of a specific node
        /// </summary>
        /// <param name="parentNode">Parent Node to get the child nodes from</param>
        /// <returns>Returns all Child nodes from the Parent node</returns>
        public List<StoryNode> GetAllChildNodes(StoryNode parentNode)
        {
            var childNodes = new List<StoryNode>();
            foreach (var nodeId in parentNode.ChildNodes)
                foreach (var node in StoryNodes)
                {
                    if (node.name.Equals(nodeId))
                        childNodes.Add(node);
                }

            return childNodes;
        }

        /// <summary>
        /// Getter to get the root node
        /// Searches the node with the root node attribute = true
        /// </summary>
        /// <returns>Returns the root node if it was found, else returns null </returns>
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
        /// Getter to get all choice nodes of a specific node
        /// </summary>
        /// <param name="node">Current Node to get the choice nodes from</param>
        /// <returns>Returns all Choice nodes from current node</returns>
        public IEnumerable<StoryNode> GetChoiceNodes(StoryNode node)
        {
            foreach (var child in GetAllChildNodes(node))
            {
                if (child.IsChoiceNode)
                    yield return child;
            }
        }

        /// <summary>
        /// Getter to get all story nodes of a specific node
        /// </summary>
        /// <param name="node">Current Node to get the story Nodes from</param>
        /// <returns>Returns all Story nodes from current node</returns>
        public IEnumerable<StoryNode> GetStoryNodes(StoryNode node)
        {
            foreach (var child in GetAllChildNodes(node))
            {
                if (!child.IsChoiceNode)
                    yield return child;
            }
        }

        #endregion

        #region Nodes save
        
        /// <summary>
        /// Writes the nodes in a JSON File
        /// </summary>
        private void SaveNodesToJson()
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