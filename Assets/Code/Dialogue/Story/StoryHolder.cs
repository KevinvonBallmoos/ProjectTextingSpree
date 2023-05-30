using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Code.GameData;
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
        [NonSerialized] public StoryAsset CurrentChapter;
        [NonSerialized] public StoryNode ParentNode;
        [NonSerialized] private StoryNode _currentNode;
        // Booleans
        [NonSerialized] public bool IsStoryNode;
        [NonSerialized] private bool _isNull;

				#region Load Chapter

				/// <summary>
				/// Loads the Save Data or Starts a new Chapter
				/// </summary>
				/// <param name="chapter">Is either null or a new chapter</param>
				public void LoadChapterProperties(StoryAsset chapter)
        {
            if (chapter != null){
                
                CurrentChapter = chapter;
                CurrentChapter = CurrentChapter.ReadNodes(chapter);
            
                _currentNode = CurrentChapter.GetRootNode();
                IsStoryNode = false;
            }
            else
            {
                GameDataController.LoadData();
                var saveData = GameDataController.GetSaveData();
                var stories =Resources.LoadAll($@"Story/", typeof(StoryAsset)).ToList();
                foreach (var asset in stories)
                {
                    if (!asset.name.Equals(saveData.CurrentChapter)) continue;
                    CurrentChapter = (StoryAsset)asset;
                    break;
                }

                foreach (var node in CurrentChapter.GetAllNodes())
                {
                    if (node.name.Equals(saveData.ParentNode))
                        _currentNode = node;
                }
                IsStoryNode = saveData.IsStoryNode;
            }
            ParentNode = _currentNode;
            _isNull = false;
            TimeAndProgress.CalculateProgress(CurrentChapter.name);
        }
        
        #endregion

        #region Next Methods
        
        /// <summary>
        /// Get next choice nodes
        /// </summary>
        /// <param name="node">Parent that contains the next choices nodes</param>
        public void Next(StoryNode node)
        {
            foreach (var n in CurrentChapter.GetStoryNodes(node))
                _currentNode = n;
            
            ParentNode = _currentNode;

            if (!CheckNodeCount()) return;
            {
                foreach (var n in CurrentChapter.GetAllChildNodes(ParentNode))
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
            foreach (var n in CurrentChapter.GetStoryNodes(_currentNode))
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
            if (CurrentChapter.GetAllChildNodes(ParentNode).Any())
                return true;

            _isNull = true;
            return false;
        }
        
        #endregion
        
        #region Getter
        
        /// <summary>
        /// Returns true if the current nod has children
        /// </summary>
        /// <returns></returns>
        public bool HasNext()
        {
            return CurrentChapter.GetAllChildNodes(_currentNode).Any();
        }
        
        public IEnumerable<StoryNode> GetChoices()
        {
            return CurrentChapter.GetChoiceNodes(_currentNode);
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

        #endregion
    }
}
