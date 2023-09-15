using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Code.Controller.GameController;
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
        //private readonly GameLogger _logger = new GameLogger("StoryHolder");
        // Dialogue and Nodes
        [NonSerialized] public StoryAsset CurrentChapter;
        [NonSerialized] private StoryNode _currentNode;
        [NonSerialized] private StoryNode _selectedChoice;
        // Booleans
        [NonSerialized] public bool IsStoryNode;

        private StoryNode[] _pastStoryNodes;
        private StoryNode[] _selectedChoices;
        private int _nodeIndex;
        private int _choiceIndex;
        
		#region Load Chapter

		/// <summary>
		/// Loads the Save Data or Starts a new Chapter
		/// </summary>
		/// <param name="chapter">Is either null or a new chapter</param>
        public bool LoadChapterProperties(StoryAsset chapter)
        {
            _choiceIndex = 0;
            var isSave = true;
            _pastStoryNodes = Array.Empty<StoryNode>();
            _selectedChoices = Array.Empty<StoryNode>();
            
            if (chapter != null){
                
                CurrentChapter = chapter;
                CurrentChapter = CurrentChapter.ReadNodes(chapter);
            
                _currentNode = CurrentChapter.GetRootNode();
                _pastStoryNodes = new StoryNode[CurrentChapter.GetAllStoryNodes().Count()];
                _pastStoryNodes[0] = _currentNode;
                _selectedChoices = new StoryNode[CurrentChapter.GetAllNodes().Count()];
                
                _nodeIndex = 0;
                IsStoryNode = false;
            }
            else
            {
                var saveData = GameDataController.GetSaveData();
                var stories = Resources.LoadAll($@"StoryAssets/", typeof(StoryAsset)).ToList();
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
                
                _pastStoryNodes = new StoryNode[CurrentChapter.GetAllStoryNodes().Count()];
                _selectedChoices = new StoryNode[CurrentChapter.GetAllNodes().Count()];

                var i = 0;
                foreach (var node in CurrentChapter.GetAllNodes())
                {
                    foreach (var p in saveData.PastStoryNodes)
                    {
                        if (p == null) break;
                        if (!node.name.Equals(p.name)) continue;
                        _pastStoryNodes[i] = node;
                        i++;
                        break;
                    }
                    
                    foreach (var c in saveData.SelectedChoices)
                    {
                        if (c == null) break;
                        if (!node.name.Equals(c.name)) continue;
                        _selectedChoices[_choiceIndex] = node;
                        _choiceIndex++;
                        break;
                    }
                }

                _nodeIndex = int.Parse(saveData.NodeIndex);
                isSave = false;
            }
            TimeAndProgress.CalculateProgress(CurrentChapter.name);
            return isSave;
        }
        
        #endregion

        #region Next Node or Node Before
        
        /// <summary>
        /// Returns the next story node
        /// </summary>
        /// <param name="selectedChoice">Parent that contains the next choices nodes</param>
        public StoryNode GetNextNode(StoryNode selectedChoice)
        {
            if (!_selectedChoices.Contains(selectedChoice))
            {
                _choiceIndex++;
                _selectedChoices[_choiceIndex] = selectedChoice;
            }

            foreach (var n in CurrentChapter.GetStoryNodes(selectedChoice))
                _currentNode = n;

            _nodeIndex++;
            _pastStoryNodes[_nodeIndex] = _currentNode;
            
            return _currentNode;
        }

        public StoryNode GetNodeBefore()
        {
            _nodeIndex--;
            return _pastStoryNodes[_nodeIndex];
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="choices"></param>
        /// <returns></returns>
        public bool CheckSelectedChoices(IEnumerable<StoryNode> choices)
        {
            foreach (var c in choices)
            {
                if (!_selectedChoices.Contains(c)) continue;
                _selectedChoice = c;
                return true;
            }

            return false;
        }

        #endregion

        #region Setter

        /// <summary>
        /// Decreases the Choice index by 1
        /// </summary>
        public void SetChoiceIndex()
        {
            _choiceIndex--;
        }

        #endregion
        
        #region Getter
        
        /// <summary>
        /// Returns true if the current node has children
        /// </summary>
        /// <param name="nodeToDisplay">currentNode</param>
        /// <returns></returns>
        public List<StoryNode> HasMoreNodes(StoryNode nodeToDisplay)
        {
            return CurrentChapter.GetAllChildNodes(nodeToDisplay);
        }

        public IEnumerable<StoryNode> GetChoices(StoryNode nodeToDisplay)
        {
            return CurrentChapter.GetChoiceNodes(nodeToDisplay);
        }

        public string GetNodeIndex()
        {
            return _nodeIndex.ToString();
        }

        public StoryNode[] GetPastStoryNodes()
        {
            return _pastStoryNodes;
        }        
        
        public StoryNode[] GetSelectedChoices()
        {
            return _selectedChoices;
        }

        public StoryNode GetCurrentNode()
        {
            return _currentNode;
        }
        
        public StoryNode GetSelectedChoice()
        {
            return _selectedChoice;
        }

        public bool GetIsEndOfStory()
        {
            return _currentNode.IsEndOfPart;
        }
        
        public bool GetIsEndOfChapter()
        {
            return _currentNode.IsEndOfChapter;
        }

        public bool GetIsGameOver()
        {
            return _currentNode.IsGameOver;
        }

        public string GetImage(StoryNode nodeToDisplay)
        {
            return nodeToDisplay.Image;
        }
        
        public string GetItem()
        {
            return _currentNode.Item;
        }

        #endregion
    }
}
