using System.Collections;
using System.Linq;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using Code.Logger;
using Code.GameData;
using Code.Inventory;

namespace Code.Dialogue.Story
{
    /// <summary>
    /// Displays the Story in the GUI
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">12.12.2022</para>
    public class  StoryUI : MonoBehaviour
    {
        // Logger
        private readonly GameLogger _logger = new GameLogger("GameManager");
        // Story Holder
        private StoryHolder _storyHolder;
        // SerializedFields
        [SerializeField] private TextMeshProUGUI story;
        [SerializeField] private Transform choiceRoot;
        [SerializeField] private GameObject choicePrefab;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button pageBackButton;
        [SerializeField] private GameObject[] imageHolder = new GameObject[2];
        [SerializeField] private GameObject saveStatus;
        // Ending Screen
        [SerializeField] private GameObject messageBoxEndScreen;
        // Story Chapter
        public StoryAsset currentChapter;
        //StoryNode
        private StoryNode _nodeToDisplay;
        // Coroutine
        private Coroutine _textCoroutine;
        // Image, Text of save status
        private Image _saveImage;
        private Text _saveText;

		#region Start

		/// <summary>
		/// When the Game starts, gets the story, adds the next button click Event and updates the UI
		/// </summary>
		public void Start()
        {
            _storyHolder = GameObject.FindGameObjectWithTag("Story").GetComponent<StoryHolder>();
            var isSave = _storyHolder.LoadChapterProperties(currentChapter);

            //if (currentChapter == null)
            currentChapter = _storyHolder.CurrentChapter;
            
            _saveImage = saveStatus.GetComponentInChildren<Image>();
            _saveText = saveStatus.GetComponentInChildren<Text>();
            
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(Next_Click);
            nextButton.GetComponentInChildren<Text>().text = "Next";
            nextButton.gameObject.SetActive(false);

            _nodeToDisplay = _storyHolder.GetCurrentNode();
            
            UpdateUI(isSave, false);
        }

		#endregion

		#region Button Events

		/// <summary>
		/// When the next button is clicked, it loads the next part of the story
		/// </summary>
		private void Next_Click()
        {
            StopCoroutine(_textCoroutine);
            _nodeToDisplay = _storyHolder.GetNextNode(_nodeToDisplay);
            UpdateUI(true, false);
        }
        
        /// <summary>
        /// Scrolls back one page
        /// </summary>
        public void ScrollBack_Click()
        {
            StopCoroutine(_textCoroutine);
            _nodeToDisplay = _storyHolder.GetNodeBefore();
            UpdateUI(false, false);
        }

        /// <summary>
        /// Scrolls back to the last choice, but the already selected, cannot be chosen again
        /// </summary>
        public void ScrollBackGameOver_Click()
        {
            messageBoxEndScreen.SetActive(false);
            StopCoroutine(_textCoroutine);
            _nodeToDisplay = _storyHolder.GetNodeBefore();
            UpdateUI(false, true);
        }

        #endregion

        #region Update UI

        /// <summary>
        /// Updates the UI, loads the next story or choice nodes and their properties
        /// Saves the node state
        /// </summary>
        /// <param name="isSave"></param>
        /// <param name="isGameOver"></param>
        private void UpdateUI(bool isSave, bool isGameOver)
        {
            pageBackButton.gameObject.SetActive(!_nodeToDisplay.IsRootNode());

            DisplayNodeProperties(); 
            UpdateNodeChoice(isGameOver);

            if (!isSave) return;
            SaveGameState();
            AddItemToInventory();
        }

        /// <summary>
        /// Updates the bottom UI with, either the choices or the next button
        /// </summary>
        private void UpdateNodeChoice(bool isGameOver)
        {
            // Checks if the current node has children
            if (_storyHolder.HasMoreNodes(_nodeToDisplay).Any())
            {
                // if the children are choice nodes
                if (_storyHolder.HasMoreNodes(_nodeToDisplay)[0].IsChoiceNode())
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
            // Displays Story Text
            story.text = "";
            _textCoroutine = StartCoroutine(TextSlower(0.02f));
            
            // Displays Image
            var image = _storyHolder.GetImage(_nodeToDisplay);
            if (!image.Equals(""))
            {
                imageHolder[0].SetActive(false);
                imageHolder[1].SetActive(true);
                imageHolder[1].GetComponent<Image>().sprite = Resources.Load <Sprite>("StoryImage/" + image);
            }
            else
            {
                imageHolder[1].SetActive(false);
                imageHolder[0].SetActive(true);
            }
            
            // Displays Chapter Title
            gameObject.GetComponentsInChildren<Text>()[0].text = GetTitleText();

            //Displays Page Back Button, when the last node was a choice
        }

        /// <summary>
        /// Saves the actual node and their properties
        /// </summary>
        private void SaveGameState()
        {
            GameDataController.Gdc.SaveGame(new SaveData
            {
                Title = GetTitleText(),
                ParentNode = _storyHolder.GetCurrentNode().name,
                IsStoryNode = _storyHolder.IsStoryNode,
                NodeIndex = _storyHolder.GetNodeIndex(),
                PastStoryNodes = _storyHolder.GetPastStoryNodes(),
                SelectedChoices = _storyHolder.GetSelectedChoices()
            });
            StartCoroutine(ShowImage());
        }

        /// <summary>
        /// Sets the Title Text
        /// </summary>
        private string GetTitleText()
        {
            var xmlDoc = new XmlDocument();  // Maybe Xml Reader - better performance
            xmlDoc.Load($@"{Application.dataPath}/Resources/StoryFiles/{currentChapter.name}.xml"); 
            var rootNode = xmlDoc.SelectSingleNode($"//{currentChapter.name}");
            return rootNode?.FirstChild.InnerText;
        }

        /// <summary>
        /// Displays the text char by char, gives a visual effect
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private IEnumerator TextSlower(float time)
        {
            var text = _nodeToDisplay.GetText().Replace("{Name}", GameDataController.Gdc.GetPlayerName());
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
            var item = _storyHolder.GetItem();
            if (!item.Equals(""))
                InventoryController.instance.AddItem(item);
        }
        
        #endregion

        #region Next Chapter / End
        
        /// <summary>
        /// Loads the next Chapter when the End of Chapter node is reached
        /// or the GameOver Screen when the GameOver node is reached
        /// </summary>
        private void NextChapter()
        {
            if (_storyHolder.GetIsEndOfChapter())
            {
                _logger.LogEntry("UI log", "End of Chapter reached.", GameLogger.GetLineNumber());
                // If No more nodes then Button Text = "Next Chapter", and switch Listener
                nextButton.GetComponentInChildren<Text>().text = "Next Chapter";
                nextButton.onClick.RemoveAllListeners();

                GameManager.Gm.IsEndOfChapter = true;
                nextButton.onClick.AddListener(GameManager.Gm.NextChapter_Click);
            }
            else  if (_storyHolder.GetIsEndOfStory())
            {
                _logger.LogEntry("UI log", "End of Story reached.", GameLogger.GetLineNumber());
                
                nextButton.GetComponentInChildren<Text>().text = "Next Part";
                nextButton.onClick.RemoveAllListeners();

                GameManager.Gm.IsEndOfStory = true;
                nextButton.onClick.AddListener(GameManager.Gm.NextStory_Click);
            }
            else if (_storyHolder.GetIsGameOver())
            {
                _logger.LogEntry("UI log", "Game Over reached.", GameLogger.GetLineNumber());
                nextButton.gameObject.SetActive(false);

                GameManager.Gm.IsGameOver = true;
            }
        }
        
        #endregion
        
        #region Choices 

        /// <summary>
        /// Builds the choice list, depending on the count of the nodes
        /// Some choices are only visible for Player with the needed background
        /// </summary>
        /// <param name="isGameOver"></param>
        private void BuildChoiceList(bool isGameOver)
        {
            foreach (Transform item in choiceRoot)
                Destroy(item.gameObject);
            var choices = _storyHolder.GetChoices(_nodeToDisplay).ToList();
            
            if (!_storyHolder.CheckSelectedChoices(choices))
            {
                foreach (var choice in choices)
                    SetChoice(choice, true);
            }
            else
            {
                if (!isGameOver)
                {
                    // only show choices, that is in the list
                    var choice = _storyHolder.GetSelectedChoice();
                    SetChoice(choice, false);
                }
                else
                {
                    var gameOverChoice = _storyHolder.GetSelectedChoice();
                    foreach (var choice in choices)
                    {
                        if (choice != gameOverChoice)
                            SetChoice(choice, true);
                    }
                    _storyHolder.SetChoiceIndex();
                }
            }
        }

        /// <summary>
        /// Sets the choice node properties
        /// </summary>
        /// <param name="choice"></param>
        /// <param name="isSave"></param>
        private void SetChoice(StoryNode choice, bool isSave)
        {
            var choiceInstance = Instantiate(choicePrefab, choiceRoot);
            var background = choice.GetBackground();

            // Check if this node can only be used by a certain player
            if (!background.Equals(""))
            {
                if (background.Equals(GameDataController.Gdc.GetPlayerBackground()))
                {
                    // Set Text
                    var choiceText = choiceInstance.GetComponentInChildren<Text>();
                    choiceText.text = choice.GetText();
                }
                else return;
            }
            else
            {
                // Set Text
                var choiceText = choiceInstance.GetComponentInChildren<Text>();
                choiceText.text = choice.GetText();
            }

            // Add listener
            var button = choiceInstance.GetComponentInChildren<Button>();
            button.onClick.AddListener(() =>
            {
                _nodeToDisplay = _storyHolder.GetNextNode(choice);
                UpdateUI(isSave, false);
            });
        }

        #endregion
    }
}

