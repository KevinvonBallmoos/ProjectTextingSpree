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
        public StoryNode currentNode = null;
        public bool isChoosing = false;

        // Logger
        private readonly GameLogger _logger = new GameLogger("StoryHolder");
        
        private void Awake()
        {
            currentNode = selectedChapter.GetRootNode();
        }
        
        public bool IsChoosing()
        {
            return isChoosing;
        }

        /// <summary>
        /// Gets the text of the Root node of the Dialog Editor
        /// </summary>
        /// <returns></returns>
        public string GetText()
        {
            if (currentNode == null)
                return "";
            else
                return currentNode.GetText();
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
        /// Select the choices
        /// </summary>
        /// <param name="chosenNode"></param>
        public void SelectChoice(StoryNode node)
        {
            currentNode = node;
            isChoosing = false;
            Next();
        }

        /// <summary>
        /// When the next Button is clicked
        /// The next Dialogue should appear
        /// </summary>
        public void Next()
        {
            _logger.LogEntry("Click", "Start", _logger.GetLineNumber());

            int numOfPlayerResponses = selectedChapter.GetChoiceNodes(currentNode).Count();
            if (numOfPlayerResponses > 0)
            {
                isChoosing = true;
                return;
            }
            
            StoryNode[] children = selectedChapter.GetStoryNodes(currentNode).ToArray();
            int index = Random.Range(0, children.Length);
            currentNode = children[index];
        }

        /// <summary>
        /// If there is more dialog returns true
        /// </summary>
        /// <returns></returns>
        public bool HasNext()
        {
            return selectedChapter.GetAllChildNodes(currentNode).Any();
        }
    }
}
