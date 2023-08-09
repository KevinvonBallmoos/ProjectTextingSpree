using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Code.Controller;
using Code.Dialogue.Story;
using Code.GameData;
using Code.Logger;
using Debug = UnityEngine.Debug;

namespace Code
{
    /// <summary>
    /// Is in Control of the Story
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
        [Header("Character Objects")]
        [SerializeField] private GameObject characters;
        [SerializeField] private Text character;
        [SerializeField] private InputField playerName;
        // StoryUI Script
        [Header("Scripts")]
        [SerializeField] private StoryUI storyUIScript;
		// States of the Game
		[NonSerialized] public bool IsGameOver;
        [NonSerialized] public bool IsEndOfChapter;
        [NonSerialized] public bool IsEndOfStory;
        // Menu Option TextSpeed
        private bool _isTextSlowed = true; 
        // Various variables
        private int _chapter;
        private int _part; 
        public static int ActiveScene;
        private string _runPath;
        private string _storyPath;

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
        /// When the Game is started (buildIndex = 0) -> loads the Save Files and displays them on the Paper Object
        /// When another Scene was loaded (buildIndex = 1 - 3) -> instantiates the story script
        /// </summary>
        private void Start()
        {
            try
            {
                _runPath = $"{Application.dataPath}/Resources/";
                _chapter = 1;
                ActiveScene = 0;
                
                if (SceneManager.GetActiveScene().buildIndex == 0)
                    GameDataController.Gdc.LoadGame();
                else if (SceneManager.GetActiveScene().buildIndex != 0)
                    _storyUI = storyUIScript;
                    
            }
            catch (Exception ex)
            {
                Debug.Log("Start Game Fail");
                _logger.LogEntry("Exception Log", ex.Message, new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
            }
        }

        #endregion

        #region Game State Button Events
        
        /// <summary>
        /// Opens the character select window and disables the select Images
        /// </summary>
        public void NewGame_Click()
        {
            // TODO: Turns to page 2
            // Display Character on pages 2 - 3,4 - 5
            // Remove line 109 + 118
            screenObjects[0].SetActive(false);
            
            var slots = characters.GetComponentsInChildren<Image>();
            for (var i = 0; i < slots.Length; i++)
            {
                if (i is 2 or 5 or 8)
                    slots[i].enabled = false;
            }
            
            screenObjects[2].SetActive(true);
        }

        /// <summary>
        /// Checks if a character was selected and a Name was given
        /// Starts a new game and checks if a save slot is empty, else asks to override another slot
        /// TODO: Always ask which slot should be taken?
        /// </summary>
        public void StartNewGame_Click()
        {
            if (playerName.text.Equals(""))
            {
                playerName.GetComponentsInChildren<Text>()[0].color = Color.red;
                return;
            }
            playerName.GetComponentsInChildren<Text>()[0].color = Color.white;
            if (character.text.Equals(""))
            {
                screenObjects[2].GetComponentsInChildren<Text>()[0].color = Color.red;
                return;
            }
            character.color = Color.white;

            if (GameDataController.Gdc.NewGame())
            {
                ActiveScene = 1;
                LoadScene();
            }
            else
            {
                GameDataController.Gdc.GetPlayer();
                GameDataController.Gdc.SetSaveScreen("NEW GAME", 1);
                screenObjects[2].SetActive(false);
                SetMessageBoxProperties(GameDataController.Gdc.Continue_Click, XmlController.GetMessageBoxText(0));
                screenObjects[1].SetActive(true);
            }
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
            _storyPath = $@"StoryAssets/Story{_part}Chapter{_chapter}.asset";
            
            if (!File.Exists($@"{_runPath}{_storyPath}")) return;
            _storyUI.currentChapter = Resources.Load<StoryAsset>(_storyPath.Replace(".asset", ""));
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

        /// <summary>
        /// Sets the isTextSlowed Property
        /// </summary>
        /// <param name="isSlowed"></param>
        public void SetIsTextSlowed(bool isSlowed)
        {
            _isTextSlowed = isSlowed;
        }

        /// <summary>
        /// Gets the isTextSlowed Property
        /// </summary>
        /// <returns></returns>
        public bool GetIsTextSlowed()
        {
            return _isTextSlowed;
        }

        #endregion
    }
}
