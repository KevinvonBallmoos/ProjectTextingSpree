using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Code.Dialogue.Story;
using Code.GameData;
using Code.Logger;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        // StoryHolder
        private static StoryHolder _selectedStory;
        // GameManager
        public static GameManager Gm;
        // Ending Screen
        [SerializeField] private GameObject endingScreen;
        
        // Menu and Save Screens
        [SerializeField] private GameObject mainMenuScreen;
        [SerializeField] private GameObject saveGameScreen;
        [SerializeField] private GameObject overrideSaveGameScreen;
        [SerializeField] private GameObject characterPropertiesScreen;
        
        [NonSerialized] public bool IsGameOver;
        [NonSerialized] public bool IsEndOfChapter;
        [NonSerialized] public bool IsEndOfStory;

        private int _chapter;
        private int _part;
        private string _runPath;
        private string _storyPath;
        
        private void Start()
        {
            var t = new Thread(StoryAsset.ReloadStoryProperties);
            t.Start();
            
            try
            {
                _runPath = $"{Application.dataPath}/Resources/";
                _storyUI = GameObject.FindGameObjectWithTag("Story").GetComponent<StoryUI>();
                _selectedStory = GameObject.FindGameObjectWithTag("Story").GetComponent<StoryHolder>();

                _chapter = 1;
            }
            catch (Exception ex)
            {
                _logger.LogEntry("Exception Log", ex.Message, new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
            }
        }

        /// <summary>
        /// Starts a new Game
        /// </summary>
        public void NewGame_Click()
        {
            if (GameData.GameDataController.NewGame())
            {
                LoadSavedScene(1);
            }
            else
            {
                overrideSaveGameScreen.SetActive(true);
            }
            // Enter name and choose character
            // GameData controller checks if a open slot is ready
            // returns yes after saving a new file and insert name and character
            // returns no: overridesavegamescreen is activated, savegamescreen
            //SceneManager.LoadScene(1);
        }

        /// <summary>
        /// Loads a saved Game
        /// </summary>
        public void LoadGame_Click()
        {
            // 
        }

        /// <summary>
        /// Loads the saved Scene
        /// </summary>
        /// <param name="scene"></param>
        public static void LoadSavedScene(int scene)
        {
            SceneManager.LoadScene(scene);
        }

        /// <summary>
        /// Checks if its Game Over or end of Chapter
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
            _storyPath = $@"Story/Part{_part}/Story{_part}Chapter{_chapter}.asset";
            
            if (!File.Exists($@"{_runPath}{_storyPath}")) return;
            _selectedStory.selectedChapter = Resources.Load<Story>(_storyPath.Replace(".asset", ""));
            _selectedStory.Start();
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
        /// When the next Chapter Button is clicked
        /// </summary>
        public void NextChapter_Click()
        {
            _storyUI.Start();
        }

        /// <summary>
        /// When the next Chapter Button is clicked
        /// </summary>
        public void NextStory_Click()
        {
            SceneManager.LoadScene(_part);
        }

        private static int GetPath()
        {
            var path = _selectedStory.selectedChapter.name;
            foreach (var t in path)
            {
                if (char.IsDigit(t))
                    return int.Parse(t.ToString());
            }
            return 0;
        }
    }
}
