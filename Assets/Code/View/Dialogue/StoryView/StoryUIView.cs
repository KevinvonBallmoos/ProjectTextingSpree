using System.Collections;
using System.Linq;
using Code.Controller.DialogueController.StoryDialogueController;
using Code.Controller.FileController;
using Code.Controller.GameController;
using Code.Logger;
using Code.Model.Dialogue.StoryModel;
using Code.Model.GameData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.View.Dialogue.StoryView 
{
    /// <summary>
    /// Displays the Story in the GUI
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">12.12.2022</para>
    public class  StoryUIView : MonoBehaviour
    {
        // Logger
        private readonly GameLogger _logger = new GameLogger("GameManager");
        // Story Holder
        private StoryHolderModel _storyHolderModel;
        // Text Control that holds the story text
        [Header("Story Text")]
        [SerializeField] private Text story;
        // Choice objects
        [Header("Choice Root and Prefab")]
        [SerializeField] private Transform choiceRoot;
        [SerializeField] private GameObject choicePrefab;
        // Buttons
        [Header("TopBar Buttons")] 
        [SerializeField] private Button nextButton;
        [SerializeField] private Button pageBackButton;
        // Object that displays image
        [Header("Image-holder")]
        [SerializeField] private GameObject[] imageHolder = new GameObject[2];
        // Object for the save animation
        [Header("Save Object")]
        [SerializeField] private GameObject saveStatus;
        // Ending Screen
        [Header("End screen")]
        [SerializeField] private GameObject messageBoxEndScreen;
        // Scrollbar
        [Header("Scrollbar")]
        [SerializeField] private Scrollbar storyScrollbar;
        // Current story chapter
        public StoryAssetController currentChapter;
        // Current node to display
        //private StoryNodeModel _nodeToDisplay;
        // Coroutine for the save status text
        private Coroutine _textCoroutine;
        // Image and Text of save status
        private Image _saveImage;
        private Text _saveText;
        // Chapter Title
        private string _chapterTitle;

        #region Start

		/// <summary>
		/// Hands over the current chapter to the Story holder,
		/// adds the next button click Event and updates the UI
		/// </summary>
		public void Start()
        {
            _storyHolderModel = GameObject.FindGameObjectWithTag("Story").GetComponent<StoryHolderModel>();
            var isSave = _storyHolderModel.LoadChapterProperties(currentChapter);
            
            currentChapter = _storyHolderModel.CurrentChapter;
            
            _saveImage = saveStatus.GetComponentInChildren<Image>();
            _saveText = saveStatus.GetComponentInChildren<Text>();
            
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(Next_Click);
            nextButton.GetComponentInChildren<Text>().text = "Next";
            nextButton.gameObject.SetActive(false);

            //_nodeToDisplay = _storyHolder.CurrentNode();
            _chapterTitle = XmlController.GetChapterTitle(currentChapter);
            
            UpdateUI(isSave, false);
        }

		#endregion

		#region Button Events

		/// <summary>
		/// When the next button is clicked, it loads the next part of the story
		/// </summary>
		private void Next_Click()
        {
            if (_textCoroutine != null)
                StopCoroutine(_textCoroutine);
            //_nodeToDisplay = _storyHolder.SetNextNode(_nodeToDisplay);
            _storyHolderModel.SetNextNode(_storyHolderModel.CurrentNode);
            UpdateUI(true, false);
        }
        
        /// <summary>
        /// Scrolls back one page
        /// </summary>
        public void ScrollBack_Click()
        {
            if (_textCoroutine != null)
                StopCoroutine(_textCoroutine);
            //_nodeToDisplay =
            _storyHolderModel.SetNodeBefore();
            UpdateUI(false, false);
            nextButton.GetComponentInChildren<Text>().text = "Next";
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(Next_Click);
        }

        /// <summary>
        /// Scrolls back to the last choice, but the already selected, cannot be chosen again
        /// </summary>
        public void ScrollBackGameOver_Click()
        {
            messageBoxEndScreen.SetActive(false);
            if (_textCoroutine != null)
                StopCoroutine(_textCoroutine);
            //_nodeToDisplay =
            _storyHolderModel.SetNodeBefore();
            UpdateUI(false, true);
        }

        #endregion

        #region Update UI

        /// <summary>
        /// Updates the UI, loads the next story or choice nodes and their properties
        /// Saves the node state
        /// </summary>
        /// <param name="isSave">true the game is saved, false then not</param>
        /// <param name="isGameOver">if true, then only the choices which haven't been selected are visible</param>
        private void UpdateUI(bool isSave, bool isGameOver)
        {
            pageBackButton.gameObject.SetActive(!_storyHolderModel.GetRootNode());
            //
            DisplayNodeProperties(); 
            UpdateNodeChoice(isGameOver);

            if (!isSave) return;
            SaveGameState();
            AddItemToInventory();
        }

        /// <summary>
        /// Updates the bottom UI with, either the choices or the next button
        /// </summary>
        /// <param name="isGameOver">if true, then only the choices which haven't been selected are visible</param>
        private void UpdateNodeChoice(bool isGameOver)
        {
            // Checks if the current node has children
            if (_storyHolderModel.HasMoreNodes(_storyHolderModel.CurrentNode).Any())
            {
                // if the children are choice nodes
                if (_storyHolderModel.IsChoiceNode(_storyHolderModel.CurrentNode))
                {
                    nextButton.gameObject.SetActive(false);
                    choiceRoot.gameObject.SetActive(true);
                    BuildChoiceList(isGameOver);
                }
                else
                {
                    nextButton.gameObject.SetActive(true);
                    choiceRoot.gameObject.SetActive(false);
                }
            }
            else
            {
                // When no more Nodes are available, continue with the next Chapter or Next Story or Ending
                nextButton.gameObject.SetActive(true);
                choiceRoot.gameObject.SetActive(false);
                NextChapter();
            }
        }

        /// <summary>
        /// Displays the Text, Image and Title of the node
        /// </summary>
        private void DisplayNodeProperties()
        {
            // Displays Story Text either one letter after another, or the whole text at once
            story.text = "";
            var text = _storyHolderModel.GetCurrentNodeText().Replace("{Name}", GameDataInfoModel.PlayerName);
            if (GameManager.Gm.IsTextSlowed)
                _textCoroutine = StartCoroutine(TextSlower(0.02f, text));
            else
                story.text = text;
            
            // Displays Image
            var image = _storyHolderModel.GetImage(_storyHolderModel.CurrentNode);
            if (!image.Equals(""))
            {
                imageHolder[0].SetActive(false);
                imageHolder[1].SetActive(true);
                imageHolder[1].GetComponent<Image>().sprite = Resources.Load <Sprite>("Images/StoryImages/" + image);
            }
            else
            {
                imageHolder[1].SetActive(false);
                imageHolder[0].SetActive(true);
            }
            
            // Displays Chapter Title
            gameObject.GetComponentsInChildren<Text>()[0].text = _chapterTitle;
        }

        /// <summary>
        /// Saves the actual node and their properties
        /// </summary>
        private void SaveGameState()
        {
            GameDataModel.SaveRunningGame(new GameDataModel
            {
                Title = _chapterTitle,
                ParentNode = _storyHolderModel.CurrentNode.name,
                IsStoryNode = _storyHolderModel.IsStoryNode,
                NodeIndex = _storyHolderModel.GetNodeIndex(),
                PastStoryNodes = _storyHolderModel.GetPastStoryNodes(),
                SelectedChoices = _storyHolderModel.GetSelectedChoices()
            });
            StartCoroutine(ShowImage());
        }

        /// <summary>
        /// Displays the text char by char, gives a visual effect
        /// </summary>
        /// <param name="time">duration of the time to wait till the next letter is printed</param>
        /// <param name="text">text to print</param>
        /// <returns></returns>
        private IEnumerator TextSlower(float time, string text)
        {
            var strArray = text.Split(' ');
            foreach (var t in strArray)
            {
                foreach (var c in t)
                {
                    story.text += c;
                    yield return new WaitForSeconds(time);
                }

                story.text += " ";
            }
            
            // Set Scroll view Position
            storyScrollbar.value = 1;
        }
        
        /// <summary>
        /// Displays the Image
        /// </summary>
        /// <returns></returns>
        private IEnumerator ShowImage()
        {
            SetSaveStatus(true);
            
            yield return new WaitForSeconds(2f);
            
            SetSaveStatus(false);
        }

        /// <summary>
        /// Enables or disables the save status image and text
        /// </summary>
        /// <param name="isEnabled">true to enable the objects, false to disable</param>
        private void SetSaveStatus(bool isEnabled)
        {
            _saveImage.enabled = isEnabled;
            _saveText.enabled = isEnabled;
        }

        /// <summary>
        /// Adds the item to the Inventory
        /// Maybe some flashy screen, showing the item up close
        /// </summary>
        private void AddItemToInventory()
        {
            var item = _storyHolderModel.GetItem();
            if (!item.Equals(""))
                InventoryController.Ic.AddItem(item);
        }
        
        #endregion

        #region Next Chapter / End
        
        /// <summary>
        /// Loads the next Chapter, when the End of Chapter node is reached
        /// Loads the next Story, when the EndOfStoryNode is reached
        /// Loads the GameOver Screen, when the GameOver node is reached
        /// Loads the End of Tale Screen (End Credits screen), when the Game is finished (Not yet developed)
        /// </summary>
        private void NextChapter()
        {
            if (_storyHolderModel.GetIsEndOfChapter())
            {
                _logger.LogEntry("UI log", "End of Chapter reached.", GameLogger.GetLineNumber());
                // If No more nodes then Button Text = "Next Chapter", and switch Listener
                nextButton.GetComponentInChildren<Text>().text = "Next Chapter";
                nextButton.onClick.RemoveAllListeners();

                GameManager.Gm.IsEndOfChapter = true;
                nextButton.onClick.AddListener(UIManager.Uim.NextChapter_Click);
            }
            else  if (_storyHolderModel.GetIsEndOfStory())
            {
                _logger.LogEntry("UI log", "End of Story reached.", GameLogger.GetLineNumber());
                
                nextButton.GetComponentInChildren<Text>().text = "Next Part";
                nextButton.onClick.RemoveAllListeners();

                GameManager.Gm.IsEndOfPart = true;
                nextButton.onClick.AddListener(UIManager.Uim.NextPart_Click);
            }
            else if (_storyHolderModel.GetIsGameOver())
            {
                _logger.LogEntry("UI log", "Game Over reached.", GameLogger.GetLineNumber());
                nextButton.gameObject.SetActive(false);

                GameManager.Gm.IsGameOver = true;
            }
            // else if (_storyHolder.GetIsEndOfTale())
            // {
            //      _logger.LogEntry("UI log", "End of Tale reached.", GameLogger.GetLineNumber());
            //
            //      GameManager.GM.IsEndOfTale = true;
            // }
        }
        
        #endregion
        
        #region Choices 

        /// <summary>
        /// Builds the choice list, depending on the count of the nodes
        /// Some choices are only visible for players with the required background
        /// </summary>
        /// <param name="isGameOver">if true, then only the choices which haven't been selected are visible</param>
        private void BuildChoiceList(bool isGameOver)
        {
            foreach (Transform item in choiceRoot)
                Destroy(item.gameObject);
            var choices = _storyHolderModel.GetChoices(_storyHolderModel.CurrentNode).ToList();

            var i = 0;
            if (!_storyHolderModel.CheckSelectedChoices(choices))
            {
                foreach (var choice in choices)
                {
                    _storyHolderModel.ChoiceNodes[i] = choice;
                    SetChoice(true, i);
                    i++;
                }
            }
            else
            {
                if (!isGameOver)
                {
                    SetChoice( false, i);
                }
                else
                {
                    var gameOverChoice = _storyHolderModel.SetSelectedChoice(i);
                    foreach (var choice in choices)
                    {
                        if (choice != gameOverChoice)
                        {
                            _storyHolderModel.ChoiceNodes[i] = choice;
                            SetChoice(true, i);
                            i++;
                        }
                    }
                    _storyHolderModel.SetChoiceIndex();
                }
            }
        }

        /// <summary>
        /// Sets the properties of the choice nodes
        /// </summary>
        /// <param name="isSave">true the game is saved, false then not</param>
        /// <param name="index">index of the choice node array</param>
        private void SetChoice(bool isSave, int index)
        {
            var choiceInstance = Instantiate(choicePrefab, choiceRoot);
            var background = _storyHolderModel.GetBackground(_storyHolderModel.ChoiceNodes[index]);

            // Check if this node can only be used by a certain player
            if (!background.Equals(""))
            {
                if (background.Equals(GameDataInfoModel.PlayerBackground))
                {
                    // Set Text
                    var choiceText = choiceInstance.GetComponentInChildren<Text>();
                    choiceText.text = _storyHolderModel.GetChoiceText(_storyHolderModel.ChoiceNodes[index]);
                }
                else return;
            }
            else
            {
                // Set Text
                var choiceText = choiceInstance.GetComponentInChildren<Text>();
                choiceText.text = _storyHolderModel.GetChoiceText(_storyHolderModel.ChoiceNodes[index]);
            }

            // Add listener
            var button = choiceInstance.GetComponentInChildren<Button>();
            button.onClick.AddListener(() =>
            {
                _storyHolderModel.SetNextNode(_storyHolderModel.ChoiceNodes[index]);
                UpdateUI(isSave, false);
            });
        }

        #endregion
    }
}

