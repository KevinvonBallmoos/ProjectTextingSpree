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
        private StoryNode _parentNode = null;
        private StoryNode _currentNode = null;
        // Booleans
        private bool _isStoryNode;
        private bool _isNull;

        public void StartScript()
        {
            if (selectedChapter == null) return;
            _currentNode = selectedChapter.GetRootNode();
            _parentNode = _currentNode;
            _isStoryNode = false;
            _isNull = false;
        }
        
        /// <summary>
        /// Get next Choice Nodes
        /// </summary>
        /// <param name="node">parent node that contains the next choices nodes</param>
        public void Next(StoryNode node)
        {
            foreach (var n in selectedChapter.GetStoryNodes(node))
                _currentNode = n;
            
            _parentNode = _currentNode;

            if (!CheckNodeCount()) return;
            {
                foreach (var n in selectedChapter.GetAllChildNodes(_parentNode))
                    _isStoryNode = !n.IsChoiceNode();
            }
            _logger.LogEntry("Story Holder log", $"Retuning next Choice node {_currentNode.name}", GameLogger.GetLineNumber());

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
            
            _parentNode = _currentNode;
            _logger.LogEntry("Story Holder log", $"Retuning next Story node {_currentNode.name}", GameLogger.GetLineNumber());

        }
        
        /// <summary>
        /// Returns the count of child nodes the parent has
        /// </summary>
        /// <returns></returns>
        private bool CheckNodeCount()
        {
            if (selectedChapter.GetAllChildNodes(_parentNode).Any())
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
            return _parentNode.GetText();
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
            return _parentNode.IsRootNode();
        }
        
        public bool IsEndOfChapter()
        {
            return _currentNode.IsEndOfChapter();
        }

        public bool IsGameOver()
        {
            return _currentNode.IsGameOver();
        }

        public string GetImage()
        {
            return _currentNode.GetImage();
        }
    }
}
