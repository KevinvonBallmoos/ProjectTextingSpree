using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Code.Controller.FileControllers;
using Code.Dialogue.Story;
using Code.GameData;
using Code.Logger;

namespace Code
{
    /// <summary>
    /// Is in Control of the Game, Story and handles the Scenes
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">11.01.2023</para>
    public class GameManager : MonoBehaviour
    {
        // Logger
        private readonly GameLogger _logger = new GameLogger("GameManager");
        // GameManager
        public static GameManager Gm;
        // Story UI
        private static StoryUI _storyUI;
        // Menu, Message Box and Character Screen Objects
        [Header("Menu, Save and Message Box Screen Objects")]
        [SerializeField] private GameObject[] screenObjects;
        [SerializeField] private GameObject[] messageBox;
        // Message Box Game Over Screen Object
        [Header("Game over Message Box")]
        [SerializeField] private GameObject messageBoxGameOver;
        // Character
        [Header("Character")] 
        [SerializeField] private GameObject[] characterPages;
        [SerializeField] public GameObject[] characters;
        [SerializeField] private Text chosenCharacter;
        [SerializeField] private InputField playerName;
        // Buttons
        [Header("TopBar Buttons")] 
        [SerializeField] private Button[] buttons;
        // StoryUI Script
        [Header("Scripts")]
        [SerializeField] private StoryUI storyUIScript;
		// States of the Game
		[NonSerialized] public bool IsGameOver;
        [NonSerialized] public bool IsEndOfChapter;
        [NonSerialized] public bool IsEndOfStory;
        // Active Scene
        [NonSerialized] public static int ActiveScene;
        // Chapter, Part and Path
        private int _chapter;
        private int _part;
        private string _runPath;
        // Regex Pattern for InputField
        private const string RegexPattern = "^[A-Za-z0-9\\s]+$";

        #region Awake and Start

        /// <summary>
        /// Awake of the GameManager
        /// Assigns the GameManager so it's always the same Object and does not get destroyed when switching scenes
        /// </summary>
        private void Awake()
        {
            if (Gm == null)
                Gm = this;
        }

        /// <summary>
        /// Start of the GameManager
        /// Sets the path, chapter and active scene
        /// Creates necessary Folders
        /// When the Game is started (buildIndex = 0) -> loads the Save Files and displays them on the Paper Object
        /// When another Scene was loaded (buildIndex = 1 - 3) -> instantiates the story script
        /// This is needed, because the GameManager is existing in all scenes,
        /// this determines from which scene the GameManager is started
        /// </summary>
        private void Start()
        {
            try
            {
                _runPath = $"{Application.dataPath}/Resources/";
                _chapter = 1;
                ActiveScene = 0;
                
                FileController.CreateFolders();
                
                if (SceneManager.GetActiveScene().buildIndex == 0)
                    GameDataController.Gdc.LoadGame();
                else if (SceneManager.GetActiveScene().buildIndex != 0)
                    _storyUI = storyUIScript;
                    
            }
            catch (Exception ex)
            {
                // TODO: Maybe quit Game??
                _logger.LogEntry("Exception Log", ex.Message, new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
            }
        }

        #endregion

        #region Game State Button Events
        
        /// <summary>
        /// Opens the character select window and disables the select Images
        /// Sets the scrollbar to the top
        /// </summary>
        public void NewGame_Click()
        {
            // TODO: Animation Turns to page 2
            // Display Character on pages 2 - 3,4 - 5
            screenObjects[0].SetActive(false);
            screenObjects[2].SetActive(true);
            foreach (var c in characters)
            {
                var image = c.GetComponentsInChildren<Image>()[2];
                image.enabled = false;
                var scrollbar = c.GetComponentInChildren<Scrollbar>();
                scrollbar.value = 1;
            }
            buttons[0].onClick.AddListener(BackToMainMenu_Click);
        }

        /// <summary>
        /// Checks if a character was selected and a Name was given
        /// Starts a new game and checks if a save slot is empty, else asks to override another slot
        /// </summary>
        public void StartNewGame_Click()
        {
            if (!InputField_OnSubmit()) return;
            if (chosenCharacter.text.Equals(""))
            {
                screenObjects[2].GetComponentsInChildren<Text>()[0].color = Color.red;
                return;
            }
            chosenCharacter.color = Color.white;

            if (GameDataController.Gdc.NewGame(playerName.text, chosenCharacter.text))
            {
                ActiveScene = 1;
                LoadScene();
            }
            else
            {
                GameDataController.Gdc.SetSaveScreen("NEW GAME", 1);
                screenObjects[2].SetActive(false);
                SetMessageBoxProperties(GameDataController.Gdc.Continue_Click, XmlController.GetMessageBoxText(0));
                screenObjects[1].SetActive(true);
            }
        }

        public void ScrollNextCharacterPage_CLick()
        {
            characterPages[0].SetActive(false);
            characterPages[1].SetActive(true);
            ChangeButtonProperties(ScrollPreviousCharacterPage_CLick, "Go back", false);

        }

        private void ScrollPreviousCharacterPage_CLick()
        {
            characterPages[0].SetActive(true);
            characterPages[1].SetActive(false);
            ChangeButtonProperties(BackToMainMenu_Click, "Back to Menu", true);
        }

        private void ChangeButtonProperties(UnityAction eventMethod, string text, bool isEnabled)
        {
            buttons[0].onClick.RemoveAllListeners();
            buttons[0].onClick.AddListener(eventMethod);
            buttons[0].GetComponentInChildren<Text>().text = text;
            
            buttons[1].gameObject.SetActive(isEnabled);
        }
        
        #endregion
        
        #region Next Chapter / Story or End
        
        /// <summary>
        /// Update Method
        /// Checks the status if its Game Over, end of Chapter or end of story
        /// </summary>
        private void Update()
        {
            if (IsEndOfChapter)
                LoadNextChapter();
            if (IsEndOfStory)
                LoadNextStoryPart();
            if (IsGameOver)
                LoadGameOverScreen();
        }

        /// <summary>
        /// Sets the Path for the next Chapter
        /// Loads next chapter
        /// </summary>
        private void LoadNextChapter()
        {
            IsEndOfChapter = false;
            _part = GetPath();
            _chapter++;
            var storyPath = $@"StoryAssets/Story{_part}Chapter{_chapter}.asset";
            
            if (!File.Exists($@"{_runPath}{storyPath}")) return;
            _storyUI.currentChapter = Resources.Load<StoryAsset>(storyPath.Replace(".asset", ""));
            _logger.LogEntry("GameManager Log", $"Next chapter: Story{_part}Chapter{_chapter}", GameLogger.GetLineNumber());
        }

        /// <summary>
        /// Load next Story / next scene
        /// </summary>
        private void LoadNextStoryPart()
        {
            IsEndOfStory = false;
            _part++;
            _logger.LogEntry("GameManager Log", $"Next Story Part: Story{_part}Chapter{_chapter}",
                GameLogger.GetLineNumber());
        }

        /// <summary>
        /// Load GameOver Screen
        /// </summary>
        private void LoadGameOverScreen()
        {
            IsGameOver = false;
            messageBoxGameOver.SetActive(true);
            _logger.LogEntry("GameManager Log", $"Game Over! ", GameLogger.GetLineNumber());
        }

        /// <summary>
        /// Returns the Story Part
        /// </summary>
        /// <returns></returns>
        private static int GetPath()
        {
            var path = _storyUI.currentChapter.name;
            foreach (var t in path)
            {
                if (char.IsDigit(t))
                    return int.Parse(t.ToString());
            }
            return 0;
        }
        
        #endregion
        
        #region Next Chapter / Story Button Events
        
        /// <summary>
        /// When the next chapter Button is clicked
        /// </summary>
        public void NextChapter_Click()
        {
            _storyUI.Start();
        }

        /// <summary>
        /// When the next story Button is clicked
        /// </summary>
        public void NextStory_Click()
        {
            ActiveScene = ActiveScene switch
            {
                1 => 2,
                2 => 3,
                3 => 2,
                _ => ActiveScene
            };

            SceneManager.LoadScene(ActiveScene);
        }
        
        #endregion
        
        #region Inputfield Events

        /// <summary>
        /// Handles the event when the user starts writing
        /// </summary>
        public void InputField_OnValueChanged()
        {
            var text = playerName.text;
            if (text.Equals(""))
            {
                playerName.GetComponentsInChildren<Text>()[0].color = Color.red;
                return;
            }

            var isMatch = Regex.IsMatch(text[^1].ToString(), RegexPattern);
            if (!isMatch)
            {
                playerName.text = text[..^1];
                return;
            }
            playerName.GetComponentsInChildren<Text>()[0].color = Color.white;
        }

        /// <summary>
        ///  When the 
        /// </summary>
        private bool InputField_OnSubmit()
        {
            if (playerName.text.Equals(""))
                playerName.GetComponentsInChildren<Text>()[0].color = Color.red;
            return !playerName.text.Equals("");
        }
        
        #endregion
        
        #region MessageBox

        /// <summary>
        /// Sets the properties of the MessageBox
        /// </summary>
        /// <param name="eventMethod"></param>
        /// <param name="text"></param>
        public void SetMessageBoxProperties(UnityAction eventMethod, string text)
        {
            messageBox[0].GetComponent<Button>().onClick.RemoveAllListeners();
            messageBox[0].GetComponent<Button>().onClick.AddListener(eventMethod);
            messageBox[1].GetComponent<Text>().text = text;
        }

        #endregion

        #region Main Menu

        public void BackToMainMenu_Click()
        {
            messageBoxGameOver.SetActive(false);
            ActiveScene = 0;
            LoadScene();
        }

		/// <summary>
		/// Loads the next Scene
		/// </summary>
		public static void LoadScene()
		{
            SceneManager.LoadScene(ActiveScene);
		}

		#endregion

        #region Menu Options
        
        public bool IsTextSlowed { get; set; }

        #endregion
    }
}
