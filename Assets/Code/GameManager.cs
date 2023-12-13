using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Code.Controller.GameController;
using Code.Model.Dialogue.StoryDialogue;
using Code.Model.Files;
using Code.View.Dialogue.StoryView;

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
        //private readonly GameLogger _logger = new GameLogger("GameManager");
        // GameManager instance
        public static GameManager Gm;
        // Story UI
        private static StoryUIView _storyUIView;
        // StoryUI Script
        [Header("Scripts")]
        [SerializeField] private StoryUIView storyUIViewScript;
		// States of the Game
		[NonSerialized] public bool IsGameOver;
        [NonSerialized] public bool IsEndOfChapter;
        [NonSerialized] public bool IsEndOfPart;
        // Chapter, Part and Path
        private int _chapter;
        private int _part;
        private string _runPath;

        #region Properties
        
        // Active Scene
        public int ActiveScene { get; set; }    
        
        // Menu Option Property :
        // true  : the Story Text will appear letter by letter
        // false : the Story Text will appear directly
        public bool IsTextSlowed { get; set; }
        
        #endregion

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
                LocalizationManager.LoadLocalizableValues();
                FileIOModel.CreateFolders();
                
                if (SceneManager.GetActiveScene().buildIndex != 0)
                    _storyUIView = storyUIViewScript;
                    
            }
            catch (Exception ex)
            {
                //_logger.LogEntry("Exception Log", ex.Message, new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
            }
        }

        #endregion
        
        #region Next Chapter / Story or End
        
        /// <summary>
        /// Checks the status of the Game every frame
        /// 1. End of the current Chapter
        /// 2. End of the current Story Part
        /// 3. Game over
        /// 4. The Game is finished
        /// </summary>
        private void Update()
        {
            if (IsEndOfChapter)
                LoadNextChapter();
            if (IsEndOfPart)
                LoadNextStoryPart();
            if (IsGameOver)
                LoadGameOverScreen();
            // if (IsEndOfTale)
            //     LoadEndCreditScene();
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
            _storyUIView.currentChapter = Resources.Load<StoryAssetModel>(storyPath.Replace(".asset", ""));
            //_logger.LogEntry("GameManager Log", $"Next chapter: Story{_part}Chapter{_chapter}", GameLogger.GetLineNumber());
        }

        /// <summary>
        /// Load next Story / next scene
        /// </summary>
        private void LoadNextStoryPart()
        {
            IsEndOfPart = false;
            _part++;
            //_logger.LogEntry("GameManager Log", $"Next Story Part: Story{_part}Chapter{_chapter}",
                //GameLogger.GetLineNumber());
        }

        /// <summary>
        /// Load GameOver Screen
        /// </summary>
        private void LoadGameOverScreen()
        {
            IsGameOver = false;
            UIManager.Uim.EnableOrDisableMessageBoxGameOver(true);
            //_logger.LogEntry("GameManager Log", $"Game Over! ", GameLogger.GetLineNumber());
        }

        /// <summary>
        /// Returns the Story Part
        /// </summary>
        /// <returns>the number of the story</returns>
        private static int GetPath()
        {
            var path = _storyUIView.currentChapter.name;
            foreach (var t in path)
            {
                if (char.IsDigit(t))
                    return int.Parse(t.ToString());
            }
            return 0;
        }
        
        #endregion
        
        #region Next Chapter / Next Part Button Events
        
        /// <summary>
        /// Starts the new Chapter
        /// </summary>
        public void NextChapter()
        {
            _storyUIView.Start();
        }

        /// <summary>
        /// Switching between scenes
        /// 1 => 2 NewGameScene to StoryScene1
        /// 2 => 3 StoryScene1 to StoryScene2
        /// 3 => 2 StoryScene2 to StoryScene1
        /// </summary>
        public void NextPart()
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

        #region Load Scene

		/// <summary>
		/// Loads the Scene saved in the 'ActiveScene' variable
		/// </summary>
		public void LoadScene()
		{
            SceneManager.LoadScene(ActiveScene);
		}

		#endregion

    }
}
