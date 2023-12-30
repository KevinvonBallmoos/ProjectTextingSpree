using System;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

using Code.View.Base;
using Code.View.ControlElements;
using Code.View.Dialogue.StoryView;
using Code.View.SceneUIManager;
using UnityEngine;
using UnityEngine.UI;

namespace Code
{
    /// <summary>
    /// This class handles all UI Events
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">23.12.2023</para>
    public class UIManager : ComponentBase
    {
        // UI Manager instance
        public static UIManager Uim;
        // ControlView
        [NonSerialized] public ControlView ControlView;
        // StoryUIView
        [NonSerialized] public StoryUIView StoryUIView;
        
        #region Awake and Start

        /// <summary>
        /// Awake of the UIManager
        /// Sets a new Instance of the ControlView
        /// </summary>
        private void Awake()
        {
            if (Uim == null)
                Uim = this;
            ControlView = gameObject.AddComponent<ControlView>();
        }

        /// <summary>
        /// When the Game is started (buildIndex = 0) -> loads the Save Files and displays them on the Paper Object
        /// </summary>
        private void Start()
        {
            GameManager.Gm.SetActiveScene(null, false);
            InitializeUI();
        }

        /// <summary>
        /// Initializes UI components, depending on the scene that is loaded.
        /// </summary>
        private void InitializeUI()
        {
            switch (GameManager.Gm.ActiveScene)
            {
                case 0:
                    MainMenuUIManager.MmUim.InitializeUI();
                    break;
                case 1:
                    CharacterPageUIManager.CpUim.InitializeUI();
                    break;
                case 2 or 3:
                    StoryUIView = gameObject.AddComponent<StoryUIView>();
                    StoryUIManager.SUim.InitializeUI(StoryUIView);
                    break;
            }
        }

        #endregion
        
        #region GameBook Open Book

        /// <summary>
        /// Opens the character select window and disables the select Images
        /// </summary>
        public void ButtonOpenBook_Click()
        {
            // TODO: Animation Turns to page 2
            // Display Character on pages 2 - 3,4 - 5
            GameManager.Gm.SetActiveScene(1, true);
        }

        #endregion
        
        #region Character Page Start New Game

        /// <summary>
        /// Action to check, to start a new game
        /// </summary>
        /// <param name="playerName">The player name input field</param>
        /// <param name="chosenCharacter">The chosen character text component</param>
        /// <param name="characterPage">The Character page game object</param>
        public void StartNewGame(InputField playerName, Text chosenCharacter, GameObject characterPage)
        {
            ControlView.BookButtonStartNewGame(playerName, chosenCharacter, characterPage, messageBox);
        }
        
        #endregion
        
        #region Next Chapter / Next Part Button Events
        
        /// <summary>
        /// Starts the new Chapter
        /// </summary>
        public void NextChapter_Click()
        {
            StoryUIManager.SUim.InitializeUI(StoryUIView);
        }

        /// <summary>
        /// Switching between scenes
        /// 1 => 2 NewGameScene to StoryScene1
        /// 2 => 3 StoryScene1 to StoryScene2
        /// 3 => 2 StoryScene2 to StoryScene1
        /// </summary>
        public void NextPart_Click()
        {
            GameManager.Gm.NextPart();
        }
        
        #endregion

        #region MessageBox
        
        /// <summary>
        /// Calls the Control view to set the properties of the MessageBox
        /// </summary>
        /// <param name="eventMethod">Listener to add to the Button</param>
        /// <param name="buttonText">Button left text</param>
        /// <param name="text">Message Box text</param>
        public void SetMessageBoxProperties(UnityAction eventMethod, string buttonText, string text)
        {
            ControlView.SetMessageBoxProperties(messageBoxGameObjects, eventMethod, buttonText, text);
        }

        /// <summary>
        /// Action to continue the override of the savedata process
        /// </summary>
        public void Continue_Click()
        {
            GameManager.Gm.SetActiveScene(0, false);
            ControlView.ContinueAction();
        }

        /// <summary>
        /// Action to cancel the Messagebox
        /// </summary>
        public void Cancel_CLick()
        {
            ControlView.CancelAction(messageBox);
        }

        /// <summary>
        /// Enables or disables the messagebox game over screen 
        /// </summary>
        /// <param name="enable">true enable, false disable</param>
        public void EnableOrDisableMessageBoxGameOver(bool enable)
        {
            messageBox.SetActive(enable);
        }
        
        #endregion
        
        #region Quit Game

        /// <summary>
        /// Return back to the main menu
        /// </summary>
        public void BackToMainMenu_Click()
        {
            GameManager.Gm.SetActiveScene(0, true);
        }

        /// <summary>
        /// Quits the game, and returns to the desktop
        /// </summary>
        public void QuitGame_Click()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
        
        #endregion
    }
}