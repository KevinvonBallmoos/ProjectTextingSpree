using System.Collections.Generic;
using UnityEngine;

namespace Code.Dialogue.Combat
{
    /// <summary>
    /// Object Class for CombatNode
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">14.07.2023</para>
    public class CombatNode : ScriptableObject
    {
        // Text that is in the node
        [SerializeField] private string text;
        // Text that is in the node
        [SerializeField] private string labelText;
        // Different Types of Nodes
        [SerializeField] private string nodeId;
        // Different Types of Nodes
        [SerializeField] private bool isChoiceNode;
        [SerializeField] private bool isRootNode;
        [SerializeField] private bool hasLost;
        // Image
        [SerializeField] private string image;
        // ChildNodes
        [SerializeField] private List<string> childNodes = new ();
        // Rect of Editor
        [SerializeField] private Rect dialogueRect = new (10, 10, 300, 180);
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
        /// Sets the Image of the node
        /// </summary>
        /// <param name="img"></param>
        public void SetImage(string img)
        {
            image = img;
        }
        
        /// <summary>
        /// Sets the HasLost Property of the node
        /// </summary>
        /// <param name="lost"></param>
        public void SetHasLost(bool lost)
        {
            hasLost = lost;
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
        /// Sets the value of isChoiceNode to true or false
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
            dialogueRect.position = new Vector2(x,y);
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

        public bool HasLost()
        {
            return hasLost;
        }

		public bool IsChoiceNode()
        {
            return isChoiceNode;
        }

        public bool IsRootNode()
        {
            return isRootNode;
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

        public List<string> GetChildNodes()
        {
            return childNodes;
        }

        public Rect GetRect()
        {
            return dialogueRect;
        }

        public Rect GetTextRect()
        {
            return textRect;
        }

		#endregion
    }
}