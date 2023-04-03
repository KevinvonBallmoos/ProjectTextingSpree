using System;
using System.Collections.Generic;
using System.Linq;
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
        [NonSerialized] private StoryNode _currentNode;
        // Booleans
        [NonSerialized] public bool IsStoryNode;
        [NonSerialized] private bool _isNull;

        /// <summary>
        /// Loads the Save Data or Starts a new Chapter
        /// </summary>
        public void Start()
        {
            if (!GameDataManager.GameDataManager.LoadData())
            {
                _currentNode = selectedChapter.GetRootNode();
                ParentNode = _currentNode;
                IsStoryNode = false;
                _isNull = false;
            }
            else
            {
                var saveData = GameDataManager.GameDataManager.GetSaveData();
                var path = $@"Story/Part{int.Parse(selectedChapter.name[5].ToString())}/";
                selectedChapter = Resources.Load<Story>(path + saveData.CurrentChapter);

                foreach (var node in selectedChapter.GetAllNodes())
                {
                    if (node.name.Equals(saveData.ParentNode))
                        _currentNode = node;
                }
                ParentNode = _currentNode;
                IsStoryNode = saveData.IsStoryNode;
                _isNull = false;
            }
        }

        /// <summary>
        /// Get next Choice Nodes
        /// </summary>
        /// <param name="node">parent node that contains the next choices nodes</param>
        public void Next(StoryNode node)
        {
            foreach (var n in selectedChapter.GetStoryNodes(node))
                _currentNode = n;
            
            ParentNode = _currentNode;

            if (!CheckNodeCount()) return;
            {
                foreach (var n in selectedChapter.GetAllChildNodes(ParentNode))
                    IsStoryNode = !n.IsChoiceNode();
            }
            _logger.LogEntry("Story Holder log", $"Returning next Choice node {_currentNode.name}", GameLogger.GetLineNumber());
        }

        /// <summary>
        /// Get next Story Node
        /// # Overload  without parameter
        /// </summary>
        public void Next()
        {
            if (!CheckNodeCount()) return;
            foreach (var n in selectedChapter.GetStoryNodes(_currentNode))
                _currentNode = n;
            
            ParentNode = _currentNode;
            _logger.LogEntry("Story Holder log", $"Returning next Story node {_currentNode.name}", GameLogger.GetLineNumber());
        }
        
        /// <summary>
        /// Returns the count of child nodes the parent has
        /// </summary>
        /// <returns></returns>
        private bool CheckNodeCount()
        {
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
            return selectedChapter.GetAllChildNodes(_currentNode).Any();
        }
        
        /// <summary>
        /// Returns Choices
        /// </summary>
        /// <returns></returns>
        public IEnumerable<StoryNode> GetChoices()
        {
            return selectedChapter.GetChoiceNodes(_currentNode);
        }
        
        public string GetRootNodeText()
        { 
            return selectedChapter.GetRootNode().GetText();
        }
        
        public string GetParentNodeText()
        {
            return ParentNode.GetText();
        }

        public bool GetIsNull()
        {
            return _isNull;
        }
        
        public bool GetIsStoryNode()
        {
            return IsStoryNode;
        }
        
        public bool GetIsRootNode()
        {
            return ParentNode.IsRootNode();
        }
        
        public bool GetIsEndOfStory()
        {
            return _currentNode.IsEndOfStory();
        }
        
        public bool GetIsEndOfChapter()
        {
            return _currentNode.IsEndOfChapter();
        }

        public bool GetIsGameOver()
        {
            return _currentNode.IsGameOver();
        }

        public string GetImage()
        {
            return _currentNode.GetImage();
        }
        
        public string GetItem()
        {
            return _currentNode.GetItem();
        }
    }
}
