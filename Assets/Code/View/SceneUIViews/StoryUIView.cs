using System.Collections;
using System.IO;
using System.Linq;
using Code.Controller.DialogueController.StoryDialogueController;
using Code.Controller.FileController;
using Code.Controller.GameController;
using Code.Logger;
using Code.Model.Dialogue.StoryModel;
using Code.Model.GameData;
using Code.Model.Settings;
using Code.View.SceneUIManager;
using UnityEngine;
using UnityEngine.UI;
using GameObject = UnityEngine.GameObject;

namespace Code.View.SceneUIViews 
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
        // StoryUIManager 
        private StoryUIManager _storyUIManager;
        // Current story chapter
        public StoryAssetController currentChapter;
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
		public void InitializeStoryUI()
        {
            SettingsModel.LoadSettings();
            
            _storyUIManager = StoryUIManager.SUim;
            _storyHolderModel = GameObject.FindGameObjectWithTag("Story").GetComponent<StoryHolderModel>();
            if (currentChapter == null)
            {
                currentChapter = StoryAssetModel.StoryAssets[0].Asset;
                StoryAssetModel.CurrentAsset = currentChapter;
            }

            var isSave = _storyHolderModel.LoadChapterProperties(currentChapter);
            
            currentChapter = _storyHolderModel.CurrentChapter;

            var saveStatus = _storyUIManager.GetSaveStatusObject();
            _saveImage = saveStatus.GetComponentInChildren<Image>();
            _saveText = saveStatus.GetComponentInChildren<Text>();
            
            var nextButton = _storyUIManager.GetButtonNext();
            nextButton.GetComponent<Button>().onClick.RemoveAllListeners();
            nextButton.GetComponent<Button>().onClick.AddListener(ButtonNext);
            nextButton.GetComponentInChildren<Text>().text = "Next";
            //nextButton.gameObject.SetActive(false);
            nextButton.SetActive(false);
            
            _chapterTitle = XmlController.GetChapterTitle(currentChapter);
            
            UpdateUI(isSave, false);
        }

		#endregion

		#region Button Events

		/// <summary>
		/// When the next button is clicked, it loads the next part of the story
		/// </summary>
		public void ButtonNext()
        {
            if (_textCoroutine != null)
                StopCoroutine(_textCoroutine);
            _storyHolderModel.SetNextNode(_storyHolderModel.CurrentNode);
            UpdateUI(true, false);
        }
        
        /// <summary>
        /// Scrolls back one page
        /// </summary>
        public void ScrollBack()
        {
            if (_textCoroutine != null)
                StopCoroutine(_textCoroutine);
            _storyHolderModel.SetNodeBefore();
            UpdateUI(false, false);
            
            var nextButton = _storyUIManager.GetButtonNext();
            nextButton.GetComponentInChildren<Text>().text = "Next";
            nextButton.GetComponent<Button>().onClick.RemoveAllListeners();
            nextButton.GetComponent<Button>().onClick.AddListener(ButtonNext);
        }

        /// <summary>
        /// Scrolls back to the last choice, but the already selected, cannot be chosen again
        /// </summary>
        /// <param name="messageBox">the message box object</param>
        public void ScrollBackGameOver(GameObject messageBox)
        {
            messageBox.SetActive(false);
            if (_textCoroutine != null)
                StopCoroutine(_textCoroutine);
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
            var pageBackButton = _storyUIManager.GetButtonPageBack();
            pageBackButton.SetActive(!_storyHolderModel.GetRootNode());
            
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
            var nextButton = _storyUIManager.GetButtonNext();
            var choiceRoot = _storyUIManager.GetChoiceRoot();
            // Checks if the current node has children
            if (_storyHolderModel.HasMoreNodes(_storyHolderModel.CurrentNode).Any())
            {
                // if the children are choice nodes
                if (_storyHolderModel.IsChoiceNode(_storyHolderModel.CurrentNode))
                {
                    nextButton.SetActive(false);
                    choiceRoot.gameObject.SetActive(true);
                    BuildChoiceList(isGameOver);
                }
                else
                {
                    nextButton.SetActive(true);
                    choiceRoot.gameObject.SetActive(false);
                }
            }
            else
            {
                // When no more Nodes are available, continue with the next Chapter or Next Story or Ending
                nextButton.SetActive(true);
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
            var story = _storyUIManager.GetStoryObjects()[0].GetComponent<Text>();
            var scrollbar = _storyUIManager.GetStoryObjects()[1].GetComponent<Scrollbar>();
            var imageObjects = _storyUIManager.GetMenuGroupObjects();
            story.text = "";
            var text = _storyHolderModel.GetCurrentNodeText().Replace("{Name}", GameDataInfoModel.PlayerName);
            if (SettingsInfoModel.IsTextSlowed.SettingValue){
                _textCoroutine = StartCoroutine(TextSlower(0.02f, text, story, scrollbar));
                // Set scrollbar handle to the top
                scrollbar.value = 1;
            }
            else
                story.text = text;
            
            // Displays Image
            var image = _storyHolderModel.GetImage();
            if (!image.Equals(""))
            {
                imageObjects[0].GetComponent<Text>().text = _storyHolderModel.GetImageTitle();
                imageObjects[1].SetActive(true);
                imageObjects[2].SetActive(false);
                imageObjects[3].SetActive(true);
                imageObjects[3].GetComponent<Image>().sprite = Resources.Load <Sprite>("Images/StoryImages/" + image);
            }
            else
            {
                imageObjects[1].SetActive(false);
                imageObjects[2].SetActive(true);
                imageObjects[3].SetActive(false);
            }
            
            // Displays Chapter Title
            _storyUIManager.GetStoryObjects()[2].GetComponent<Text>().text = _chapterTitle;
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
        /// <param name="story">story text component</param>
        /// <param name="storyScrollbar">story scrollbar component</param>
        /// <returns></returns>
        private IEnumerator TextSlower(float time, string text, Text story, Scrollbar storyScrollbar)
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
            var nextButton = _storyUIManager.GetButtonNext();
            if (_storyHolderModel.GetIsEndOfChapter())
            {
                _logger.LogEntry("UI log", "End of Chapter reached.", GameLogger.GetLineNumber());
                // If No more nodes then Button Text = "Next Chapter", and switch Listener
                nextButton.GetComponentInChildren<Text>().text = "Next Chapter";
                nextButton.GetComponent<Button>().onClick.RemoveAllListeners();

                LoadNextChapter();
                nextButton.GetComponent<Button>().onClick.AddListener(UIManager.Uim.NextChapter_Click);
            }
            else  if (_storyHolderModel.GetIsEndOfStory())
            {
                _logger.LogEntry("UI log", "End of Story reached.", GameLogger.GetLineNumber());
                
                nextButton.GetComponentInChildren<Text>().text = "Next Part";
                nextButton.GetComponent<Button>().onClick.RemoveAllListeners();

                LoadNextStoryPart();
                nextButton.GetComponent<Button>().onClick.AddListener(UIManager.Uim.NextPart_Click);
            }
            else if (_storyHolderModel.GetIsGameOver())
            {
                _logger.LogEntry("UI log", "Game Over reached.", GameLogger.GetLineNumber());
                nextButton.SetActive(false);

                GameManager.Gm.IsGameOver = true;
                EnableGameOverMessageBox();
            }
            // else if (_storyHolder.GetIsEndOfTale())
            // {
            //      _logger.LogEntry("UI log", "End of Tale reached.", GameLogger.GetLineNumber());
            //
            //      GameManager.GM.IsEndOfTale = true;
            // }
        }
        
        /// <summary>
        /// Sets the Path for the next Chapter
        /// Loads next chapter
        /// </summary>
        private void LoadNextChapter()
        {
            var chapter = StoryUIManager.SUim.Chapter++;
            var part = StoryUIManager.SUim.Part = GetPath();
            var storyPath = $@"StoryAssets/Story{part}Chapter{chapter}.asset";
            
            if (!File.Exists($@"{GameManager.Gm.RunPath}{storyPath}")) return;
            currentChapter = StoryAssetModel.GetAsset(storyPath);
        }
        
        /// <summary>
        /// Load next Story / next scene
        /// </summary>
        private void LoadNextStoryPart()
        {
            StoryUIManager.SUim.Part++;
        }
        
        /// <summary>
        /// Load GameOver Screen
        /// </summary>
        private void EnableGameOverMessageBox()
        {
            UIManager.Uim.EnableOrDisableMessageBoxGameOver(true);
        }
        
        /// <summary>
        /// Returns the Story Part
        /// </summary>
        /// <returns>the number of the story</returns>
        private int GetPath()
        {
            foreach (var t in currentChapter.name)
            {
                if (char.IsDigit(t))
                    return int.Parse(t.ToString());
            }
            return 0;
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
            var choiceRoot = _storyUIManager.GetChoiceRoot();
            foreach (Transform item in choiceRoot.transform)
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
            var choicePrefab = _storyUIManager.GetChoiceButtonPrefab();
            var choiceRoot = _storyUIManager.GetChoiceRoot().transform;
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
        
        #region Settings Panel Focus
        
        /// <summary>
        /// Opens the settings and disables the hover and click events of
        /// the game book, game data paper, quit and settings game object
        /// </summary>
        /// <param name="settingsPanel">settings panel</param>
        /// <param name="mainMenuGameObjects">game data paper and game book</param>
        /// <param name="menuGameObjects">button settings and quit, hover label</param>
        public void OpenSettings(GameObject settingsPanel, GameObject[] mainMenuGameObjects, GameObject[] menuGameObjects)
        {
            SetGameObjectsBehavior(settingsPanel, mainMenuGameObjects, menuGameObjects, false);
        }

        /// <summary>
        /// Down scales the GameDataPaper, back to origin state
        /// Main menu view is active
        /// </summary>
        /// <param name="settingsPanel">settings panel</param>
        /// <param name="mainMenuGameObjects">game data paper and game book</param>
        /// <param name="menuGameObjects">button settings and quit, hover label</param>
        public void CloseSettings(GameObject settingsPanel, GameObject[] mainMenuGameObjects, GameObject[] menuGameObjects)
        {
            SetGameObjectsBehavior(settingsPanel, mainMenuGameObjects, menuGameObjects, true);
        }

        /// <summary>
        /// Sets the behavior of the other controls
        /// </summary>
        /// <param name="settingsPanel">settings panel</param>
        /// <param name="mainMenuGameObjects">game data paper and game book</param>
        /// <param name="menuGameObjects">button settings and quit, hover label</param>
        /// <param name="isActive">true the settings panel will be visible, false it will be hidden</param>
        private void SetGameObjectsBehavior(GameObject settingsPanel, GameObject[] mainMenuGameObjects, GameObject[] menuGameObjects, bool isActive)
        {
            settingsPanel.SetActive(!isActive);
        }

        #endregion
    }
}

