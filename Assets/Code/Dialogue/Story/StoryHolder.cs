using System;
using System.Collections.Generic;
using System.Linq;
using Code.DataPersistence;
using UnityEngine;

using Code.Logger;

namespace Code.Dialogue.Story
{
    /// <summary>
    /// Holds the Story
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">12.12.2022</para>
    public class StoryHolder : MonoBehaviour
    {
        // Logger
        private readonly GameLogger _logger = new GameLogger("StoryHolder");
        // Dialogue and Nodes
        public Story selectedChapter;
        [NonSerialized] public StoryNode ParentNode;
        [NonSerialized] public StoryNode CurrentNode;
        // Booleans
        [NonSerialized] public bool _isStoryNode;
        [NonSerialized] public bool _isNull;

        public void Start()
        {
            if (!DataPersistanceManager.LoadData())
            {
                if (selectedChapter == null)
                {
                    selectedChapter = null;
                }
                else
                {
                    CurrentNode = selectedChapter.GetRootNode();
                    ParentNode = CurrentNode;
                    _isStoryNode = false;
                    _isNull = false;
                }
            }
            else
            {
                var saveData = DataPersistanceManager.GetSaveData();
                var path = $@"Story/Part{int.Parse(selectedChapter.name[5].ToString())}/";
                selectedChapter = Resources.Load<Story>(path + saveData.CurrentChapter.name);

                foreach (var node in selectedChapter.GetAllNodes())
                {
                    if (node.name.Equals(saveData.ParentNode.name))
                        CurrentNode = node;
                }
                ParentNode = CurrentNode;
                _isStoryNode = saveData.IsStoryNode;
                _isNull = saveData.IsNull;
            }
        }

        /// <summary>
        /// Get next Choice Nodes
        /// </summary>
        /// <param name="node">parent node that contains the next choices nodes</param>
        public void Next(StoryNode node)
        {
            foreach (var n in selectedChapter.GetStoryNodes(node))
                CurrentNode = n;
            
            ParentNode = CurrentNode;

            if (!CheckNodeCount()) return;
            {
                foreach (var n in selectedChapter.GetAllChildNodes(ParentNode))
                    _isStoryNode = !n.IsChoiceNode();
            }
            _logger.LogEntry("Story Holder log", $"Returning next Choice node {CurrentNode.name}", GameLogger.GetLineNumber());
        }

        /// <summary>
        /// Get next Story Node
        /// # Overload  without parameter
        /// </summary>
        public void Next()
        {
            if (!CheckNodeCount()) return;
            foreach (var n in selectedChapter.GetStoryNodes(CurrentNode))
                CurrentNode = n;
            
            ParentNode = CurrentNode;
            _logger.LogEntry("Story Holder log", $"Returning next Story node {CurrentNode.name}", GameLogger.GetLineNumber());
        }
        
        /// <summary>
        /// Returns the count of child nodes the parent has
        /// </summary>
        /// <returns></returns>
        private bool CheckNodeCount()
        {Debug.Log(selectedChapter.GetAllChildNodes(ParentNode).Count());
            if (selectedChapter.GetAllChildNodes(ParentNode).Any())
                return true;

            _isNull = true;
            return false;
        }
        
        /// <summary>
        /// If there is more dialog returns true
        /// </summary>
        /// <returns></returns>
        public bool HasNext()
        {
            return selectedChapter.GetAllChildNodes(CurrentNode).Any();
        }
        
        /// <summary>
        /// Returns Choices
        /// </summary>
        /// <returns></returns>
        public IEnumerable<StoryNode> GetChoices()
        {
            return selectedChapter.GetChoiceNodes(CurrentNode);
        }
        
        public string GetRootNodeText()
        { 
            return selectedChapter.GetRootNode().GetText();
        }
        
        public string GetParentNodeText()
        {
            return ParentNode.GetText();
        }

        public bool IsNull()
        {
            return _isNull;
        }
        
        public bool IsStoryNode()
        {
            return _isStoryNode;
        }
        
        public bool IsRootNode()
        {
            return ParentNode.IsRootNode();
        }
        
        public bool IsEndOfStory()
        {
            return CurrentNode.IsEndOfStory();
        }
        
        public bool IsEndOfChapter()
        {
            return CurrentNode.IsEndOfChapter();
        }

        public bool IsGameOver()
        {
            return CurrentNode.IsGameOver();
        }

        public string GetImage()
        {
            return CurrentNode.GetImage();
        }
    }
}
