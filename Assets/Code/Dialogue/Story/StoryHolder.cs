using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Code.Logger;
using Random = UnityEngine.Random;

namespace Code.Dialogue.Story
{
    /// <summary>
    /// Holds the Story
    /// </summary>
    /// <para name="author">Kevin von Ballmoos></para>
    /// <para name="date">12.12.2022</para>
    public class StoryHolder : MonoBehaviour
    {
        // Logger
        private readonly GameLogger _logger = new GameLogger("StoryHolder");
        // Dialogue and Nodes
        [SerializeField] private Story selectedChapter;
        private StoryNode _parentNode = null;
        private StoryNode _currentNode = null;
        // Booleans
        private bool _isStoryNode = false;
        private bool _isNull = false;

        private void Awake()
        {
            _currentNode = selectedChapter.GetRootNode();
            _parentNode = _currentNode;
        }
        
        /// <summary>
        /// Get next Choice Nodes
        /// </summary>
        /// <param name="node"></param>
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
        }

        /// <summary>
        /// Get next Story Node
        /// </summary>
        public void Next()
        {
            if (!CheckNodeCount()) return;
            foreach (var n in selectedChapter.GetStoryNodes(_currentNode))
                _currentNode = n;

            _parentNode = _currentNode;
        }
        
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

        /// <summary>
        /// Gets the text of the Root node of the Dialog Editor
        /// </summary>
        /// <returns></returns>
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
