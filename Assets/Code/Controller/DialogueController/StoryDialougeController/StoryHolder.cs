using System;
using System.Collections.Generic;
using System.Linq;
using Code.Controller.GameController;
using Code.Dialogue.Story;
using Code.GameData;
using Code.Model.NodeModels;
using UnityEngine;

namespace Code.Controller.DialogueController.StoryDialougeController
{
    /// <summary>
    /// Holds the Story and provides information about the selected nodes and next nodes
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">12.12.2022</para>
    public class StoryHolder : MonoBehaviour
    {
        // Logger
        //private readonly GameLogger _logger = new GameLogger("StoryHolder");
        // Current Chapter
        [NonSerialized] public StoryAssetModel CurrentChapter;
        // Selected choice node
        [NonSerialized] private StoryNodeModel _selectedChoice;
        // Story node or not
        [NonSerialized] public bool IsStoryNode;
        // Stores all story nodes
        private StoryNodeModel[] _pastStoryNodes;
        // Stores all selected choices
        private StoryNodeModel[] _selectedChoices;
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
        public bool LoadChapterProperties(StoryAssetModel chapter)
        {
            _choiceIndex = 0;
            _pastStoryNodes = Array.Empty<StoryNodeModel>();
            _selectedChoices = Array.Empty<StoryNodeModel>();
            
            if (chapter != null){
                
                CurrentChapter = chapter;
                CurrentChapter = CurrentChapter.ReadNodes(chapter);
            
                CurrentNode = CurrentChapter.GetRootNode();
                _pastStoryNodes = new StoryNodeModel[CurrentChapter.GetAllStoryNodes().Count()];
                _pastStoryNodes[0] = CurrentNode;
                _selectedChoices = new StoryNodeModel[CurrentChapter.GetAllNodes().Count()];
                
                _nodeIndex = 0;
                IsStoryNode = false;
                
                TimeAndProgress.CalculateProgress(CurrentChapter.name);
                return true;
            }
            else
            {
                var saveData = GameDataController.GetSaveData();
                var stories = Resources.LoadAll($@"StoryAssets/", typeof(StoryAssetModel)).ToList();
                foreach (var asset in stories)
                {
                    if (!asset.name.Equals(saveData.CurrentChapter)) continue;
                    CurrentChapter = (StoryAssetModel)asset;
                    break;
                }

                foreach (var node in CurrentChapter.GetAllNodes())
                {
                    if (node.name.Equals(saveData.ParentNode))
                        CurrentNode = node;
                }
                IsStoryNode = saveData.IsStoryNode;
                
                _pastStoryNodes = new StoryNodeModel[CurrentChapter.GetAllStoryNodes().Count()];
                _selectedChoices = new StoryNodeModel[CurrentChapter.GetAllNodes().Count()];

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
        public StoryNodeModel SetNextNode(StoryNodeModel selectedChoice)
        {
            if (!_selectedChoices.Contains(selectedChoice))
            {
                _choiceIndex++;
                _selectedChoices[_choiceIndex] = selectedChoice;
            }

            foreach (var n in CurrentChapter.GetStoryNodes(selectedChoice))
                CurrentNode = n;

            _nodeIndex++;
            _pastStoryNodes[_nodeIndex] = CurrentNode;
            
            return CurrentNode;
        }

        /// <summary>
        /// Gets the node that was selected before
        /// </summary>
        /// <returns></returns>
        public void SetNodeBefore()
        {
            _nodeIndex--;
            CurrentNode = _pastStoryNodes[_nodeIndex];
        }
        
        /// <summary>
        /// Checks if the array with selected choices contains the choice node
        /// Sets the selected choice if it was found
        /// </summary>
        /// <param name="choices"></param>
        /// <returns>true if the selected choice was found</returns>
        public bool CheckSelectedChoices(IEnumerable<StoryNodeModel> choices)
        {
            foreach (var c in choices)
            {
                if (!_selectedChoices.Contains(c)) continue;
                _selectedChoice = c;
                ChoiceNodes[0] = _selectedChoice;
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
        public List<StoryNodeModel> HasMoreNodes(StoryNodeModel nodeToDisplay)
        {
            return CurrentChapter.GetAllChildNodes(nodeToDisplay);
        }
        
        /// <summary>
        /// Get all child nodes from a specific node
        /// </summary>
        /// <param name="nodeToDisplay">from this node the child nodes are needed</param>
        /// <returns>list of all child nodes from a specific node</returns>
        public bool IsChoiceNode(StoryNodeModel nodeToDisplay)
        {
            return CurrentChapter.GetAllChildNodes(nodeToDisplay)[0].IsChoiceNode;
        }

        /// <summary>
        /// Get all choice nodes from a specific node
        /// </summary>
        /// <param name="nodeToDisplay">rom this node the choice nodes are needed</param>
        /// <returns>list of all choice nodes from a specific node</returns>
        public IEnumerable<StoryNodeModel> GetChoices(StoryNodeModel nodeToDisplay)
        {
            return CurrentChapter.GetChoiceNodes(nodeToDisplay);
        }

        public string GetNodeIndex()
        {
            return _nodeIndex.ToString();
        }

        public StoryNodeModel[] GetPastStoryNodes()
        {
            return _pastStoryNodes;
        }        
        
        public StoryNodeModel[] GetSelectedChoices()
        {
            return _selectedChoices;
        }

        public StoryNodeModel CurrentNode
        {
            get; private set;
        }

        public StoryNodeModel[] ChoiceNodes
        {
            get;
            set;
        } = new StoryNodeModel[3];
        
        public string GetBackground(StoryNodeModel node)
        {
            return node.Background;
        }

        public string GetChoiceText(StoryNodeModel node)
        {
            return node.Text;
        }

        public string GetCurrentNodeText()
        {
            return CurrentNode.Text;
        }
        
        public StoryNodeModel SetSelectedChoice(int index)
        {
            ChoiceNodes[index] = _selectedChoice;
            return _selectedChoice;
        }

        #region  node Properties

        public bool GetRootNode()
        {
            return CurrentNode.IsRootNode;
        }
        
        public bool GetIsEndOfStory()
        {
            return CurrentNode.IsEndOfPart;
        }
        
        public bool GetIsEndOfChapter()
        {
            return CurrentNode.IsEndOfChapter;
        }

        public bool GetIsGameOver()
        {
            return CurrentNode.IsGameOver;
        }

        public string GetImage(StoryNodeModel nodeToDisplay)
        {
            return nodeToDisplay.Image;
        }
        
        public string GetItem()
        {
            return CurrentNode.Item;
        }

        #endregion
        
        #endregion
    }
}
