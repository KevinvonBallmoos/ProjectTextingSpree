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
using Code.View.ControlElements;
using TMPro;
using UnityEngine.Serialization;

namespace Code
{
    public class UIManager : MonoBehaviour
    {
        // UI Manager instance
        public static UIManager Uim;
        // ControlView
        private ControlView _controlView;
        // TopBar Buttons
        [Header("Character Page TopBar Buttons")] 
        [SerializeField] public Button[] buttons;
        // Character pages
        [Header("Character Pages")]
        [SerializeField] public GameObject[] characterPages;
        // MessageBox, Button and Text
        [Header("Messagebox")]
        [SerializeField] private GameObject[] messageBox;        
        // Main Menu, Message Box and Character Screen Objects
        [Header("Main Menu, Message Box and Character Screens")]
        [SerializeField] private GameObject[] screenObjects;
        // Remove data button
        [Header("Remove Data Button")] 
        [SerializeField] private Button removeData;
        // Load save
        [Header("Load Game Text")]
        [SerializeField] private Text buttonLoadGameText;
        // Error Label
        [FormerlySerializedAs("ErrorLabel")]
        [Header("Error Label")] 
        [SerializeField] private TextMeshProUGUI errorLabel;
        // Savedata Placeholders
        [Header("Savedata Placeholders")]
        [SerializeField] private GameObject[] placeholders;
        // Placeholder view
        [Header("Placeholder view")]
        [SerializeField] private GameObject placeholderView;
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

            if (GameManager.ActiveScene != 0) return;
            EnableRemoveDataButton();
        }

        /// <summary>
        /// When the Game is started (buildIndex = 0) -> loads the Save Files and displays them on the Paper Object
        /// </summary>
        private void Start()
        {
            if (GameManager.ActiveScene != 0) return;
            LoadGameDataOntoSaveFile("LOAD", 0, true);
        }

        #endregion
        
        #region Start New Game

        /// <summary>
        /// Opens the character select window and disables the select Images
        /// </summary>
        public void NewGame_Click()
        {
        }

        /// <summary>
        /// Checks if a character was selected and a Name was given
        /// Starts a new game and checks if a save placeholder is empty, else asks to override another placeholder
        /// </summary>
        public void StartNewGame_Click()
        {

        }

        #endregion
        
        #region Load Game Data onto save file
        
        /// <summary>
        /// Action to initializes the Save panel and the placeholders
        /// </summary>
        /// <param name="text">Text for the Button, either Load Game or New Game</param>
        /// <param name="index">Identifies which text from the xml file should be displayed</param>
        /// <param name="loadData">True => loads data into placeholders, False => only initializes the save data panel</param>
        public void LoadGameDataOntoSaveFile(string text, int index, bool loadData)
        {
            _controlView.InitializeSaveDataPanel(text, index, placeholderView, buttonLoadGameText);
            
            if (loadData)
                _controlView.LoadDataIntoPlaceholders(placeholders);
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
            _controlView.LoadOrOverrideSave(buttonLoadGameText.text);
            
            if (GameManager.ActiveScene == 2)
                GameDataController.Gdc.LoadSelectedGame();
            
            errorLabel.enabled = false;
        }

        #endregion
        
        #region Character Page Top Bar Buttons

        /// <summary>
        /// Calls the Control view to add the event method to the button
        /// </summary>
        /// <param name="button">Button to add the event</param>
        /// <param name="eventMethod">Event to add to the button</param>
        public void AddButtonListener(Button button, UnityAction eventMethod)
        {
            _controlView.AddButtonListener(button, eventMethod);
        }
        
        /// <summary>
        /// Displays the 2nd Character Page
        /// </summary>
        public void ScrollNextCharacterPage_CLick()
        {
            _controlView.ScrollNextCharacterPage(buttons);
        }

        /// <summary>
        /// Displays the 1st Character Page
        /// </summary>
        public void ScrollPreviousCharacterPage_CLick()
        {
            _controlView.ScrollPreviousCharacterPage(buttons);
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
            _controlView.SetMessageBoxProperties(messageBox, eventMethod, buttonText,text);
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
            SetMessageBoxProperties(RemoveData_Click, "Remove Data", XmlModel.GetMessageBoxText(1));
            var holders = placeholderView.GetComponentsInChildren<Image>();
            _controlView.RemoveDataAction(screenObjects[1], holders);
        }
        
        #endregion
        
        #region Remove Data
        
        /// <summary>
        /// Action to remove data
        /// </summary>
        private void RemoveData_Click()
        {
            _controlView.RemoveData(_saveDataPath, removeData, placeholders);
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