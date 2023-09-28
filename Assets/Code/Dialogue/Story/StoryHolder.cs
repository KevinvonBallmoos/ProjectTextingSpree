using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Code.Controller.GameController;
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
        //private readonly GameLogger _logger = new GameLogger("StoryHolder");
        // Current Chapter
        [NonSerialized] public StoryAsset CurrentChapter;
        // Current node 
        [NonSerialized] private StoryNode _currentNode;
        // Selected choice node
        [NonSerialized] private StoryNode _selectedChoice;
        // Story node or not
        [NonSerialized] public bool IsStoryNode;
        // Stores all story nodes
        private StoryNode[] _pastStoryNodes;
        // Stores all selected choices
        private StoryNode[] _selectedChoices;
        // node indexes
        private int _nodeIndex;
        private int _choiceIndex;
        
		#region Load Chapter

		/// <summary>
		/// Sets the Information for the current chapter and nodes (New Game),
		/// or has to get the information from the savedata (Load Game)
		/// </summary>
		/// <param name="chapter">Is either null or a new chapter</param>
		/// <returns>True if the chapter is not null, and false if the chapter was null</returns>
        public bool LoadChapterProperties(StoryAsset chapter)
        {
            _choiceIndex = 0;
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
                
                TimeAndProgress.CalculateProgress(CurrentChapter.name);
                return true;
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
                
                TimeAndProgress.CalculateProgress(CurrentChapter.name);
                return false;
            }
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

        /// <summary>
        /// Gets the node that was selected before
        /// </summary>
        /// <returns></returns>
        public StoryNode GetNodeBefore()
        {
            _nodeIndex--;
            return _pastStoryNodes[_nodeIndex];
        }
        
        /// <summary>
        /// Checks if the array with selected choices contains the choice node
        /// Sets the selected choice if it was found
        /// </summary>
        /// <param name="choices"></param>
        /// <returns>true if the selected choice was found</returns>
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
        /// Get all child nodes from a specific node
        /// </summary>
        /// <param name="nodeToDisplay">from this node the child nodes are needed</param>
        /// <returns>list of all child nodes from a specific node</returns>
        public List<StoryNode> HasMoreNodes(StoryNode nodeToDisplay)
        {
            return CurrentChapter.GetAllChildNodes(nodeToDisplay);
        }

        /// <summary>
        /// Get all choice nodes from a specific node
        /// </summary>
        /// <param name="nodeToDisplay">rom this node the choice nodes are needed</param>
        /// <returns>list of all choice nodes from a specific node</returns>
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

        #region  node Properties
        
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
        
        #endregion
    }
}
