using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Code.Dialogue.Story;
using Code.GameData;
using Code.Logger;
using UnityEngine.Serialization;

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
        // Story UI
        private static StoryUI _storyUI;
        // GameManager
        public static GameManager Gm;
        // Ending Screen
        [SerializeField] private GameObject endingScreen;
        // Menu Save and Properties Screens
        [SerializeField] private GameObject mainMenuScreen;
        [SerializeField] private GameObject messageBoxScreen;
        [SerializeField] private GameObject characterPropertiesScreen;
        [SerializeField] private GameObject characters;
        [SerializeField] private Text character;
        [SerializeField] private InputField playerName;
				[SerializeField] private GameObject[] messageBoxScreenObjects;
				// States of the Game
				[NonSerialized] public bool IsGameOver;
        [NonSerialized] public bool IsEndOfChapter;
        [NonSerialized] public bool IsEndOfStory;
        // Various variables
        private int _chapter;
        private int _part; 
        public static int ActiveScene = 0;
        private string _runPath;
        private string _storyPath;

        #region Awake and Start

        /// <summary>
        /// Awake of the GameManager
        /// </summary>
        private void Awake()
        {
            if (Gm == null)
                Gm = this;
        }

        /// <summary>
        /// Start of the GameManager
        /// </summary>
        private void Start()
        {
            try
            {
                _runPath = $"{Application.dataPath}/Resources/";
                _storyUI = GameObject.FindGameObjectWithTag("Story").GetComponent<StoryUI>();

                _chapter = 1;
            }
            catch (Exception ex)
            {
                _logger.LogEntry("Exception Log", ex.Message, new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
            }
        }

        #endregion

        #region Game State Button Events
        
        /// <summary>
        /// Opens the character select window and sets the Image to false
        /// </summary>
        public void NewGame_Click()
        {
            mainMenuScreen.SetActive(false);
            
            var slots = characters.GetComponentsInChildren<Image>();
            for (var i = 0; i < slots.Length; i++)
            {
                if (i is 2 or 5 or 8)
                    slots[i].enabled = false;
            }
            
            characterPropertiesScreen.SetActive(true);
        }

        /// <summary>
        /// Starts a new game and checks if a save slot is empty, else asks to override another slot
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
                characterPropertiesScreen.GetComponentsInChildren<Text>()[0].color = Color.red;
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
                characterPropertiesScreen.SetActive(false);
                SetMessageBoxProperties(GameDataController.Gdc.Continue_Click, XmlController.GetMessageBoxText(0));
                messageBoxScreen.SetActive(true);
            }
        }

        /// <summary>
        /// Loads a saved Game
        /// </summary>
        public void LoadGame_Click()
        {
            GameDataController.Gdc.LoadGame();
        }

        #endregion
        
        #region Next Chapter / Story or End
        
        /// <summary>
        /// Checks if its Game Over, end of Chapter or end of story
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
        /// Loads next chapter
        /// </summary>
        private void LoadNextChapter()
        {
            IsEndOfChapter = false;
            _part = GetPath();
            _chapter++;
            _storyPath = $@"Story/Story{_part}Chapter{_chapter}.asset";
            
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
            endingScreen.SetActive(true);
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
            messageBoxScreenObjects[0].GetComponent<Button>().onClick.RemoveAllListeners();
            messageBoxScreenObjects[0].GetComponent<Button>().onClick.AddListener(eventMethod);
            messageBoxScreenObjects[1].GetComponent<Text>().text = text;
        }
        
        #endregion
        
        #region Main Menu

        public void BackToMainMenu() // TODO Rename to click
        {
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
		}
}
