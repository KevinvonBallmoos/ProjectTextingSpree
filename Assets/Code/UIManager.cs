using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using Code.Controller.GameController;
using Code.Controller.LocalizationController;
using Code.Model.Files;
using Code.Model.GameData;
using Code.View.Base;
using Code.View.ControlElements;

namespace Code
{
    public class UIManager : ComponentBase
    {
        // UI Manager instance
        public static UIManager Uim;
        // Path to the Save files
        private static string _saveDataPath;
        
        #region Awake and Start

        /// <summary>
        /// Awake of the UIManager
        /// Sets a new Instance of the ControlView
        /// </summary>
        private void Awake()
        {
            if (Uim == null)
                Uim = this;
            _saveDataPath = Application.persistentDataPath + "/SaveData";
        }

        /// <summary>
        /// When the Game is started (buildIndex = 0) -> loads the Save Files and displays them on the Paper Object
        /// </summary>
        private void Start()
        {
            if (GameManager.Gm.ActiveScene != 0) return;
            EnableRemoveDataButton();
        }

        #endregion
        
        #region Start New Game

        /// <summary>
        /// Opens the character select window and disables the select Images
        /// </summary>
        public void BookButtonNewGame_Click()
        {
            // TODO: Animation Turns to page 2
            // Display Character on pages 2 - 3,4 - 5
            screenObjects[0].SetActive(false);
            screenObjects[2].SetActive(true);
            
            ControlView.Cv.NewGame();
            // Adds Listener,to go back to the menu
            ControlView.Cv.AddButtonListener(buttons[0], GameManager.Gm.BackToMainMenu_Click);        
        }

        /// <summary>
        /// Checks if a character was selected and a Name was given
        /// Starts a new game and checks if a save placeholder is empty, else asks to override another placeholder
        /// </summary>
        public void BookButtonStartNewGame_Click()
        {
            ControlView.Cv.BookButtonStartNewGame();
        }

        #endregion
        
        #region LoadOrOverrideSave

        /// <summary>
        /// Action to load a game
        /// </summary>
        public void LoadOrOverrideSave_Click()
        {
            var holders = placeholderView.GetComponentsInChildren<Image>();
            GameDataInfoModel.SetPlaceholderNum(holders);

            if (GameDataInfoModel.Placeholder == -1)
            {
                errorLabel.enabled = true;
                var key = buttonLoadGameText.text.Equals("LOAD")? LocalizationKeyController.SaveFileErrorLabelLoadCaptionKey : LocalizationKeyController.SaveFileErrorLabelOverrideCaptionKey;
                errorLabel.text = LocalizationManager.GetLocalizedValue(key);
                return;
            }
            ControlView.Cv.LoadOrOverrideSave(buttonLoadGameText.text);
            
            if (GameManager.Gm.ActiveScene == 2)
                GameDataController.Gdc.LoadSelectedGame();
            
            errorLabel.enabled = false;
        }

        #endregion
        
        #region Character Page Top Bar Buttons
        
        /// <summary>
        /// Displays the 2nd Character Page
        /// </summary>
        public void ScrollNextCharacterPage_CLick()
        {
            ControlView.Cv.ScrollNextCharacterPage();
        }

        /// <summary>
        /// Displays the 1st Character Page
        /// </summary>
        public void ScrollPreviousCharacterPage_CLick()
        {
            ControlView.Cv.ScrollPreviousCharacterPage();
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
            ControlView.Cv.SetMessageBoxProperties(eventMethod, buttonText,text);
        }

        /// <summary>
        /// Action to continue the override of the savedata process
        /// </summary>
        public void Continue_Click()
        {
            ControlView.Cv.ContinueAction();
        }

        /// <summary>
        /// Action to cancel the Messagebox
        /// </summary>
        public void Cancel_CLick()
        {
            ControlView.Cv.CancelAction();
        }
        
        /// <summary>
        /// Action to set the Message box for removing data
        /// </summary>
        public void Remove_Click()
        {
            SetMessageBoxProperties(RemoveData_Click, "Remove Data", XmlModel.GetMessageBoxText(1));
            var holders = placeholderView.GetComponentsInChildren<Image>();
            ControlView.Cv.RemoveDataAction(screenObjects[1], holders);
        }
        
        #endregion
        
        #region Remove Data
        
        /// <summary>
        /// Action to remove data
        /// </summary>
        private void RemoveData_Click()
        {
            ControlView.Cv.RemoveData(_saveDataPath);
        }

        /// <summary>
        /// Enables the Remove Button in theSave slot panel when there are any 
        /// </summary>
        private void EnableRemoveDataButton()
        {
            var files = Directory.GetFiles(_saveDataPath);   
            removeData.enabled = files.Any();
        }
        
        #endregion
    }
}