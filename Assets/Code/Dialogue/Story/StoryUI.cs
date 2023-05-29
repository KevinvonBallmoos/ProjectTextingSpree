using System.Collections;
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
        [SerializeField] private GameObject[] imageHolder = new GameObject[2];
        [SerializeField] private GameObject saveStatus;
        // Story Chapter
        public StoryAsset currentChapter;
        // Coroutine
        private Coroutine _textCoroutine;
        // Image, Text of save status
        private Image _saveImage;
        private Text _saveText;

        /// <summary>
        /// When the Game starts, gets the story, adds the Next button click Event and Updates the UI
        /// </summary>
        public void Start()
        {
            _storyHolder = GameObject.FindGameObjectWithTag("Story").GetComponent<StoryHolder>();
            _storyHolder.LoadChapterProperties(currentChapter);

            if (currentChapter == null)
                currentChapter = _storyHolder.CurrentChapter;
            
            _saveImage = saveStatus.GetComponentInChildren<Image>();
            _saveText = saveStatus.GetComponentInChildren<Text>();
            nextButton.gameObject.SetActive(false);
            nextButton.onClick.AddListener(Next_Click);
            UpdateUI();
        }

        /// <summary>
        /// When the next button is clicked, it loads the next part of the story
        /// </summary>
        private void Next_Click()
        {
            StopCoroutine(_textCoroutine);
            _storyHolder.Next();
            UpdateUI();
        }
        
        #region Update UI
        
        /// <summary>
        /// Updates the Story, loads the next part of story and the choices nodes
        /// </summary>
        private void UpdateUI()
        {
            if (!_storyHolder.GetIsNull())
            {
                // if Story Node
                if (_storyHolder.GetIsStoryNode())
                {
                    if (_storyHolder.HasNext())
                    {
                        nextButton.gameObject.SetActive(true);
                        choiceRoot.gameObject.SetActive(false);
                    }
                    else
                    {
                        NextChapter();
                    }
                }
                // if Choice Node
                else if (!_storyHolder.GetIsStoryNode())
                {
                    nextButton.gameObject.SetActive(false);
                    choiceRoot.gameObject.SetActive(true);
                    BuildChoiceList();
                }
            }
            else
            {
                // When no more Nodes are available, continue with the next Chapter or Next Story or Ending
                nextButton.gameObject.SetActive(true);
                choiceRoot.gameObject.SetActive(false);
                NextChapter();
            }
            
            DisplayNodeProperties();
            
            SaveGameState();

            AddItemToInventory();
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
            if (!_storyHolder.GetImage().Equals(""))
            {
                imageHolder[0].SetActive(false);
                imageHolder[1].SetActive(true);
                imageHolder[1].GetComponent<Image>().sprite = Resources.Load <Sprite>("StoryImage/" + _storyHolder.GetImage());
            }
            else
            {
                imageHolder[1].SetActive(false);
                imageHolder[0].SetActive(true);
            }
            
            // Displays Chapter Title
            story.GetComponentInChildren<Text>().text = GetTitleText();
        }
        
        /// <summary>
        /// Saves the actual node and their properties
        /// </summary>
        private void SaveGameState()
        {
            GameDataController.Gdc.SaveGame(new SaveData
            {
                Title = GetTitleText(),
                ParentNode = _storyHolder.ParentNode.name,
                IsStoryNode = _storyHolder.IsStoryNode,
            });
            StartCoroutine(ShowImage());
        }

        /// <summary>
        /// Sets the Title Text
        /// </summary>
        private string GetTitleText()
        {
            var xmlDoc = new XmlDocument();
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
            var text = _storyHolder.GetParentNodeText().Replace("{Name}", GameDataController.Gdc.GetPlayerName());
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
        /// Or the GameOver Screen when the GameOver node is reached
        /// </summary>
        private void NextChapter()
        {
            if (_storyHolder.GetIsEndOfChapter())
            {
                _logger.LogEntry("UI log", "End of Chapter reached.", GameLogger.GetLineNumber());
                // If No more nodes then Button Text = "Next Chapter", and switch Listener
                nextButton.GetComponentInChildren<Text>().text = "Next Chapter";
                nextButton.onClick.RemoveListener(Next_Click);

                GameManager.Gm.IsEndOfChapter = true;
                nextButton.onClick.AddListener(GameManager.Gm.NextChapter_Click);
            }
            else  if (_storyHolder.GetIsEndOfStory())
            {
                _logger.LogEntry("UI log", "End of Story reached.", GameLogger.GetLineNumber());
                
                nextButton.GetComponentInChildren<Text>().text = "Next Part";
                nextButton.onClick.RemoveListener(Next_Click);

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
        /// Some choices are only for a different Player visible
        /// </summary>
        private void BuildChoiceList()
        {
            foreach (Transform item in choiceRoot)
                Destroy(item.gameObject);

            foreach (var choice in _storyHolder.GetChoices())
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
                    _storyHolder.Next(choice);
                    UpdateUI();
                });
            }
        }
        
        #endregion
    }
}

