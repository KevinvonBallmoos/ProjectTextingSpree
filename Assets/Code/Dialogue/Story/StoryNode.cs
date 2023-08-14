﻿using System.Collections.Generic;
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
        [SerializeField] private string text;
        // Text that is in the node
        [SerializeField] private string labelText;
        // Node id
        [SerializeField] private string nodeId;
        // States and Nodes
        [SerializeField] private bool isChoiceNode;
        [SerializeField] private bool isRootNode;
        [SerializeField] private bool isEndOfStory;
        [SerializeField] private bool isEndOfChapter;
        [SerializeField] private bool isGameOver;
        // Image
        [SerializeField] private string image;
        // Item
        [SerializeField] private string item = "";
        // Player Character
        [SerializeField] private string background = "";
        // Combat
        [SerializeField] private string isCombat;
        // ChildNodes
        [SerializeField] private List<string> childNodes = new ();
        // Rect of Editor
        [SerializeField] private Rect storyRect = new (10, 10, 300, 180);
        [SerializeField] private Rect textRect;

		#region Setter

		/// <summary>
		/// Sets the Text of the node
		/// </summary>
		/// <param name="txt"></param>
		public void SetText(string txt)
        {
            text = txt;
        }

        /// <summary>
        /// Sets the label and id of the node
        /// </summary>
        /// <param name="label"></param>
        /// <param name="id"></param>
        public void SetLabelAndNodeId(string label, string id)
        {
            labelText = label;
            nodeId = id;
        }

        /// <summary>
        /// Sets the Item of the node
        /// </summary>
        /// <param name="itm"></param>
        public void SetItem(string itm)
        {
            item = itm;
        }

        /// <summary>
        /// Sets the Image of the node
        /// </summary>
        /// <param name="img"></param>
        public void SetImage(string img)
        {
            image = img;
        }

        /// <summary>
        /// Sets the Image of the node
        /// </summary>
        /// <param name="backgr"></param>
        public void SetBackground(string backgr)
        {
            background = backgr;
        }
        
        /// <summary>
        /// Sets the isRootNode
        /// </summary>
        /// <param name="isRoot"></param>
        public void SetIsRootNode(bool isRoot)
        {
            isRootNode = isRoot;
        }

        /// <summary>
        /// Sets the boolean IsGameOver
        /// </summary>
        /// <param name="isOver"></param>
        public void SetIsGameOver(bool isOver)
        {
            isGameOver = isOver;
        }

        /// <summary>
        /// Sets the boolean IsEndOfChapter
        /// </summary>
        /// <param name="isEnd"></param>
        public void SetIsEndOfChapter(bool isEnd)
        {
            isEndOfChapter = isEnd;
        }

        /// <summary>
        /// Sets the boolean IsEndOfChapter
        /// </summary>
        /// <param name="isEnd"></param>
        public void SetIsEndOfStory(bool isEnd)
        {
            isEndOfStory = isEnd;
        }

        /// <summary>
        /// Sets the value of isStoryChoice to true or false
        /// </summary>
        /// <param name="isChoice"></param>
        public void SetChoiceNode(bool isChoice)
        {
            isChoiceNode = isChoice;
        }      

        /// <summary>
        /// Sets the rect to a new position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetRect(float x, float y)
        {
            storyRect.position = new Vector2(x,y);
        }

        /// <summary>
        /// Sets the rect to a new position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetTextRect(float x, float y, float width, float height)
        {
            textRect = new Rect(x,y, width, height);
        }

		#endregion

		#region Child nodes

		/// <summary>
		/// Adds the node name to the child nodes list
		/// </summary>
		/// <param name="childId"></param>
		public void AddChildNode(string childId)
		{
			foreach (var c in GetChildNodes())
			{
				if (c.Equals(childId))
						return;
			}
			childNodes.Add(childId);
		}

		/// <summary>
		/// Removes node from child nodes
		/// </summary>
		/// <param name="childId"></param>
		public void RemoveChildNode(string childId)
		{
            childNodes.Remove(childId);
		}

		#endregion

		#region Getter


		public bool IsChoiceNode()
        {
            return isChoiceNode;
        }

        public bool IsRootNode()
        {
            return isRootNode;
        }        

        public bool IsEndOfStory()
        {
            return isEndOfStory;
        }    

        public bool IsEndOfChapter()
        {
            return isEndOfChapter;
        }        
         
        public bool IsGameOver()
        {
            return isGameOver;
        }

        public string GetText()
        {
            return text;
        }

        public string GetLabel()
        {
            return labelText;
        }

        public string GetNodeId()
        {
            return nodeId;
        }

        public string GetImage()
        {
            return image is null or "" ? "" : "image";
        }

        public string GetItem()
        {
            return !item.Equals("") ? item : "";
        }

        public string GetBackground()
        {
            return !background.Equals("") ? background : "";
        }

        public List<string> GetChildNodes()
        {
            return childNodes;
        }

        public Rect GetRect()
        {
            return storyRect;
        }

        public Rect GetTextRect()
        {
            return textRect;
        }

		#endregion
    }
}