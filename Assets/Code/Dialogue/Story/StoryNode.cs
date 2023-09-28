using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Code.Dialogue.Story
{
    /// <summary>
    /// Object Class for StoryNode
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">04.12.2022</para>
    [System.Serializable]
    public class StoryNode : ScriptableObject
    {
        // Text that is in the node
        [Header("Story Text")]
        [SerializeField] internal string text;
        // Node id, name
        [Header("Node ID and Name")]
        [SerializeField] internal string labelText;
        [SerializeField] internal string nodeId;
        // States and Nodes
        [Header("States and Nodes")]
        [SerializeField] internal bool isChoiceNode;
        [SerializeField] internal bool isRootNode;
        [SerializeField] internal bool isEndOfPart;
        [SerializeField] internal bool isEndOfChapter;
        [SerializeField] internal bool isGameOver;
        // Image, item and Player Character
        [Header("Image, item and Player Character")]
        [SerializeField] internal string image;
        [SerializeField] internal string item = "";
        [SerializeField] internal string background = "";
        // Combat (Not yet developed)
        [SerializeField] internal string combat;
        // ChildNodes
        [Header("Child nodes")]
        [SerializeField] internal List<string> childNodes = new ();
        // Rect of Editor
        [Header("Rects")]
        [SerializeField] internal Rect storyRect = new (10, 10, 300, 180);
        [SerializeField] internal Rect textRect;
        
        #region Initializing

        /// <summary>
        /// Initializes a new StoryNode
        /// </summary>
        /// <param name="node">needed to serialize and deserialize the story nodes</param>
        public void InitializeStoryNode(StoryNodeDataProperty node)
        {
            NodeId = node.NodeId;
            LabelText = node.LabelText;
            Text = node.Text;
            IsChoiceNode = node.IsChoiceNode;
            IsRootNode = node.IsRootNode;
            IsEndOfPart = node.IsEndOfStory;
            IsEndOfChapter = node.IsEndOfChapter;
            IsGameOver = node.IsGameOver;
            Image = node.Image;
            Item = node.Item;
            Background = node.Background;
            ChildNodes = node.ChildNodes;
        }
        
        #endregion

        #region StoryNode Properties
        
        public string Text
        {
            get => text;
            set => text = value;
        }

        public string LabelText
        {
            get => labelText;
            set => labelText = value;
        }

        public string NodeId
        {
            get => nodeId;
            set => nodeId = value;
        }

        public bool IsChoiceNode
        {
            get => isChoiceNode;
            set => isChoiceNode = value;
        }

        public bool IsRootNode
        {
            get => isRootNode;
            set => isRootNode = value;
        }

        public bool IsEndOfPart
        {
            get => isEndOfPart;
            set => isEndOfPart = value;
        }

        public bool IsEndOfChapter
        {
            get => isEndOfChapter;
            set => isEndOfChapter = value;
        }

        public bool IsGameOver
        {
            get => isGameOver;
            set => isGameOver = value;
        }

        public string Image
        {
            get => image is null or "" ? "" : "image";
            set => image = value;
        }

        public string Item
        {
            get => item ?? "";
            set => item = value;
        }

        public string Background
        {
            get => background?? "";
            set => background = value;
        }

        public string Combat
        {
            get => combat;
            set => combat = value;
        }

        public List<string> ChildNodes
        {
            get => childNodes;
            set => childNodes = value;
        }

        [JsonIgnore]
        public Rect StoryRect
        {
            get => storyRect;
            set => storyRect = new Rect(value.x, value.y, storyRect.width, storyRect.height);
        }

        [JsonIgnore]
        public Rect TextRect
        {
            get => textRect;
            set => textRect = new Rect(value.x,value.y, value.width, value.height);
        }
        
		#endregion
        
        #region Child nodes

        /// <summary>
        /// Adds the node name to the child nodes list
        /// </summary>
        /// <param name="childId">child to add to the list</param>
        public void AddChildNode(string childId)
        {
            foreach (var c in ChildNodes)
            {
                if (c.Equals(childId))
                    return;
            }
            ChildNodes.Add(childId);
        }

        /// <summary>
        /// Removes node from child nodes
        /// </summary>
        /// <param name="childId">child to remove from the list</param>
        public void RemoveChildNode(string childId)
        {
            ChildNodes.Remove(childId);
        }

        #endregion
    }
}