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
        // Current Dialogue only for Testing purposes
        [SerializeField] private Story selectedChapter;
        private StoryNode parentNode = null;
        private StoryNode currentNode = null;

        private bool _isStoryNode = false;
        private bool _isNull = false;

        // Logger
        private readonly GameLogger _logger = new GameLogger("StoryHolder");
        
        private void Awake()
        {
            currentNode = selectedChapter.GetRootNode();
            parentNode = currentNode;
        }
        
        /// <summary>
        /// Get next Choice Nodes
        /// </summary>
        /// <param name="node"></param>
        public void Next(StoryNode node)
        {
            foreach (var n in selectedChapter.GetStoryNodes(node))
                currentNode = n;
            
            parentNode = currentNode;

            if (!CheckNodeCount()) return;
            {
                foreach (var n in selectedChapter.GetAllChildNodes(parentNode))
                    _isStoryNode = !n.IsChoiceNode();
            }
        }

        /// <summary>
        /// Get next Story Node
        /// </summary>
        public void Next()
        {
            if (!CheckNodeCount()) return;
            foreach (var n in selectedChapter.GetStoryNodes(currentNode))
                currentNode = n;

            parentNode = currentNode;
        }
        
        private bool CheckNodeCount()
        {
            if (selectedChapter.GetAllChildNodes(parentNode).Any())
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
            return selectedChapter.GetAllChildNodes(currentNode).Any();
        }
        
        /// <summary>
        /// Returns Choices
        /// </summary>
        /// <returns></returns>
        public IEnumerable<StoryNode> GetChoices()
        {
            return selectedChapter.GetChoiceNodes(currentNode);
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
            return parentNode.GetText();
        }

        public bool IsNull()
        {
            _logger.LogEntry("LogStart", _isNull.ToString(), _logger.GetLineNumber());

            return _isNull;
        }
        
        public bool IsStoryNode()
        {
            return _isStoryNode;
        }
        
        public bool IsRootNode()
        {
            return parentNode.IsRootNode();
        }
    }
}
