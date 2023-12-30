using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

using Code.Controller.FileController;
using Code.Logger;
using Code.Model.Dialogue.StoryModel;

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
        // Game Manager instance
        public static GameManager Gm;
        // Framerate and sync count
        [Header("Framerate")]
        [SerializeField] private int targetFrameRate;        
        [SerializeField] private int vSyncCount;

        #region Properties
        
        // Active Scene
        public int ActiveScene { get; set; } 
        
        // Run Path
        public string RunPath { get; set; }
        
		// Is Game over
		public bool IsGameOver { get; set; }
        
        // Menu Option Property :
        // TODO: Create Settings Model, also include FPS Settings (recommendation, with explanation) and Size
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
        /// Sets the run path and active scene
        /// Loads all localizable strings
        /// Creates necessary Folders
        /// </summary>
        private void Start()
        {
            try
            {
                Application.targetFrameRate = targetFrameRate;
                QualitySettings.vSyncCount = vSyncCount;
                
                RunPath = $"{Application.dataPath}/Resources/";
                SetActiveScene(null, false);
                StoryAssetModel.GetAllStoryFiles();
                LocalizationManager.LoadLocalizableValues();
                FileIOController.CreateFolders();
            }
            catch (Exception ex)
            {
                _logger.LogEntry("Exception Log", ex.Message, new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
            }
        }

        #endregion
        
        #region Update 
        
        /// <summary>
        /// Checks the status of the Game every frame
        /// 1. Game over
        /// 2. The Game is finished
        /// </summary>
        private void Update()
        {
            if (IsGameOver)
            {
            }
            // if (IsEndOfTale)
            //     LoadEndCreditScene();
        }
        
        #endregion
        
        #region Load Scene / Active Scene
        
        /// <summary>
        /// Sets the current active scene index
        /// <param name="index">null or index of the current active scene</param>
        /// <param name="loadScene">true when scene needs to be loaded after setting the current active scen</param>
        /// </summary>
        public void SetActiveScene(int? index, bool loadScene)
        {
            ActiveScene = index ?? SceneManager.GetActiveScene().buildIndex;

            if (loadScene)
                LoadScene();
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
                2 => 3,
                3 => 2,
                _ => ActiveScene
            };

            LoadScene();
        }

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
