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
        // Main Menu and Save Objects
        [Header("Main Menu and Save Screens")]
        [SerializeField] private GameObject[] screenObjects;
        // Message Box Game Over Screen Object
        [Header("Game over Message Box")]
        [SerializeField] private GameObject messageBoxGameOver;
        // Character
        [Header("Character")] 
        [SerializeField] public GameObject[] characters;
        [SerializeField] private Text chosenCharacter;
        [SerializeField] private InputField playerName;
        // Buttons
        [Header("TopBar Buttons")] 
        [SerializeField] private Button[] buttons;
        // StoryUI Script
        [FormerlySerializedAs("storyUIScript")]
        [Header("Scripts")]
        [SerializeField] private StoryUIView storyUIViewScript;
		// States of the Game
		[NonSerialized] public bool IsGameOver;
        [NonSerialized] public bool IsEndOfChapter;
        [NonSerialized] public bool IsEndOfPart;
        // Active Scene
        public static int ActiveScene { get; set; }

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

        #region Game State Button Events
        
        #region Game States
        
        /// <summary>
        /// Opens the character select window and disables the select Images
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
                // Sets the scrollbar to the top
                var scrollbar = c.GetComponentInChildren<Scrollbar>();
                scrollbar.value = 1;
            }
            // Adds Listener,to go back to the menu
            UIManager.Uim.AddButtonListener(buttons[0], BackToMainMenu_Click);
        }

        /// <summary>
        /// Checks if a character was selected and a Name was given
        /// Starts a new game and checks if a save placeholder is empty, else asks to override another placeholder
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
                UIManager.Uim .LoadGameDataOntoSaveFile("Override", 1, false); //InitializeSaveDataPanel("Override", 1);
                screenObjects[2].SetActive(false);
                UIManager.Uim.SetMessageBoxProperties(UIManager.Uim.Continue_Click, "Continue", XmlModel.GetMessageBoxText(0));
                screenObjects[1].SetActive(true);
            }
        }
        
        #endregion
        
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
            messageBoxGameOver.SetActive(true);
            //_logger.LogEntry("GameManager Log", $"Game Over! ", GameLogger.GetLineNumber());
        }

        /// <summary>
        /// Returns the Story Part
        /// </summary>
        /// <returns></returns>
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
        public void NextChapter_Click()
        {
            _storyUIView.Start();
        }

        /// <summary>
        /// Switching between scenes
        /// 1 => 2 NewGameScene to StoryScene1
        /// 2 => 3 StoryScene1 to StoryScene2
        /// 3 => 2 StoryScene2 to StoryScene1
        /// </summary>
        public void NextPart_Click()
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
        /// Is triggered, when the value of the input field changes
        /// Compares the last entered char of the input with the regex string
        /// if the input does not match, the last entered char is removed
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
        /// Is triggered when the User submits the Username
        /// It checks if the input is empty or not
        /// </summary>
        private bool InputField_OnSubmit()
        {
            if (playerName.text.Equals(""))
                playerName.GetComponentsInChildren<Text>()[0].color = Color.red;
            return !playerName.text.Equals("");
        }
        
        #endregion

        #region Main Menu

        /// <summary>
        /// Hides the Message Box
        /// Loads the MainMenu Scene
        /// </summary>
        public void BackToMainMenu_Click()
        {
            messageBoxGameOver.SetActive(false);
            ActiveScene = 0;
            LoadScene();
        }

		/// <summary>
		/// Loads the Scene saved in the 'ActiveScene' variable
		/// </summary>
		public static void LoadScene()
		{
            SceneManager.LoadScene(ActiveScene);
		}

		#endregion

        #region Menu Options
        
        /// <summary>
        /// Menu Option Property :
        /// true  : the Story Text will appear letter by letter
        /// false : the Story Text will appear directly
        /// </summary>
        public bool IsTextSlowed { get; set; }

        #endregion
    }
}
