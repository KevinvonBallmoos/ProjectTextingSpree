using System.IO;
using System.Linq;
using Code.Controller.FileController;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using Code.Controller.GameController;
using Code.Controller.LocalizationController;
using Code.Model.GameData;
using Code.View.Base;
using Code.View.ControlElements;

namespace Code
{
    public class UIManager : ComponentBase
    {
        // UI Manager instance
        public static UIManager Uim;
        // ControlView
        private ControlView _controlView;
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
            _controlView = gameObject.AddComponent<ControlView>();
            _saveDataPath = Application.persistentDataPath + "/SaveData";
        }

        /// <summary>
        /// When the Game is started (buildIndex = 0) -> loads the Save Files and displays them on the Paper Object
        /// </summary>
        private void Start()
        {
            if (GameManager.Gm.ActiveScene != 0) return;
            EnableRemoveDataButton();
            _controlView.InitializeSaveDataPanel("LOAD", 0, true, placeholderView, buttonLoadGameText, placeholders);
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
            
            _controlView.NewGame(characters);
            // Adds Listener,to go back to the menu
            _controlView.AddButtonListener(buttons[0], BackToMainMenu_Click);        
        }

        /// <summary>
        /// Checks if a character was selected and a Name was given
        /// Starts a new game and checks if a save placeholder is empty, else asks to override another placeholder
        /// </summary>
        public void BookButtonStartNewGame_Click()
        {
            _controlView.BookButtonStartNewGame(playerName, chosenCharacter, screenObjects, placeholderView, buttonLoadGameText, placeholders);
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
            
            _controlView.LoadOrOverrideSave(buttonLoadGameText);
            
            if (GameManager.Gm.ActiveScene == 2)
                GameDataController.Gdc.LoadSelectedGame();
            
            errorLabel.enabled = false;
        }

        #endregion
        
        #region Character Page Top Bar Buttons
        
        /// <summary>
        /// Hides the Message Box
        /// Loads the MainMenu Scene
        /// </summary>
        public void BackToMainMenu_Click()
        {
            EnableOrDisableMessageBoxGameOver(false);
            GameManager.Gm.ActiveScene = 0;
            GameManager.Gm.LoadScene();
        }
        
        /// <summary>
        /// Displays the 2nd Character Page
        /// </summary>
        public void ScrollNextCharacterPage_CLick()
        {
            _controlView.ScrollNextCharacterPage(buttons, characterPages);
        }

        /// <summary>
        /// Displays the 1st Character Page
        /// </summary>
        public void ScrollPreviousCharacterPage_CLick()
        {
            _controlView.ScrollPreviousCharacterPage(buttons, characterPages);
        }
        
        #endregion

        #region Characterselect

        /// <summary>
        /// Sets the character Field, with the title of the selected Character
        /// </summary>
        public void Character_Click(GameObject characterGameObject)
        {
            _controlView.SetImage(characters, chosenCharacter, characterGameObject);
        }

        #endregion
        
        #region Characterselect Input Field

        /// <summary>
        /// Is triggered, when the value of the input field changes
        /// Compares the last entered char of the input with the regex string
        /// if the input does not match, the last entered char is removed
        /// </summary>
        public void InputField_OnValueChanged()
        {
            _controlView.ValidateInputField(playerName);
        }

        /// <summary>
        /// Is triggered when the User submits the Username
        /// It checks if the input is empty or not
        /// </summary>
        private bool InputField_OnSubmit()
        {
            return _controlView.SubmitInputField(playerName);
        }
        
        #endregion
        
        #region Next Chapter / Next Part Button Events
        
        /// <summary>
        /// Starts the new Chapter
        /// </summary>
        public void NextChapter_Click()
        {
            GameManager.Gm.NextChapter();
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
            _controlView.SetMessageBoxProperties(messageBox, eventMethod, buttonText, text);
        }

        /// <summary>
        /// Action to continue the override of the savedata process
        /// </summary>
        public void Continue_Click()
        {
            _controlView.ContinueAction(screenObjects);
        }

        /// <summary>
        /// Action to cancel the Messagebox
        /// </summary>
        public void Cancel_CLick()
        {
            _controlView.CancelAction(screenObjects);
        }
        
        /// <summary>
        /// Action to set the Message box for removing data
        /// </summary>
        public void Remove_Click()
        {
            SetMessageBoxProperties(RemoveData_Click, "Remove Data", LocalizationManager.GetLocalizedValue(LocalizationKeyController.MessageBoxText2CaptionKey));
            var holders = placeholderView.GetComponentsInChildren<Image>();
            _controlView.RemoveDataAction(screenObjects[1], holders, errorLabel);
        }

        /// <summary>
        /// Enables or disables the messagebox game over screen 
        /// </summary>
        /// <param name="enable">true enable, false disable</param>
        public void EnableOrDisableMessageBoxGameOver(bool enable)
        {
            messageBoxGameOver.SetActive(enable);
        }
        
        #endregion
        
        #region Remove Data
        
        /// <summary>
        /// Action to remove data
        /// </summary>
        private void RemoveData_Click()
        {
            _controlView.RemoveData(_saveDataPath, removeData, placeholders, screenObjects[1]);
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