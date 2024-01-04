using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using Code.Controller.GameController;
using Code.Controller.LocalizationController;
using Code.Model.GameData;
using Code.Model.Settings;
using Code.View.Base;
using Code.View.SceneUIViews;

namespace Code.View.SceneUIManager
{
    /// <summary>
    /// This class handles the UI Events of the Main Menu
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">23.12.2023</para>
    public class MainMenuUIManager : MainMenuComponents
    {
        // MainMenu UI Manager instance
        public static MainMenuUIManager MmUim;
        // ComponentView
        private ComponentView _componentView;
        // MainMenuUIView
        private MainMenuUIView _mainMenuUIView;
        // Path to the Save files
        private static string _saveDataPath;
        
        #region Awake and Start

        /// <summary>
        /// Awake of the MainMenuUIManager instance
        /// Sets a new Instance of the ControlView
        /// </summary>
        private void Awake()
        {
            if (MmUim == null)
                MmUim = this;
            _componentView = UIManager.Uim.ComponentView;
            _saveDataPath = Application.persistentDataPath + "/SaveData";
        }
        
        #endregion

        #region Initialize UI

        /// <summary>
        /// Initializes the UI components for the main menu
        /// </summary>
        public void InitializeUI()
        {
            EnableRemoveDataButton();
            _mainMenuUIView = UIManager.Uim.MainMenuUIView;
            if (GameDataInfoModel.IsOverride)
            {
                _mainMenuUIView.InitializeSaveDataPanel("NEW GAME", true, placeholderView, gameDataGameObjects[0],
                    placeholders);
                DisplayLoadingPaper_Click();
            }
            else
            {
                _mainMenuUIView.InitializeSaveDataPanel("LOAD", true, placeholderView, gameDataGameObjects[0],
                    placeholders);
            }
        }

        #endregion
        
        #region GameData Paper

        /// <summary>
        /// Action to upscale the GameDataPaper
        /// </summary>
        public void DisplayLoadingPaper_Click()
        { 
            _mainMenuUIView.DisplayLoadingPaper(mainMenuGameObjects, placeholderView);
            mainMenuGameObjects[0].GetComponentsInChildren<Image>()[0].material = UIManager.Uim.defaultMaterial;
            _mainMenuUIView.SetGameObjectsBehavior(mainMenuGameObjects, menuGameObjects, gameDataGameObjects, placeholders, false);
        }

        /// <summary>
        /// Action to go back to the main menu view
        /// </summary>
        public void BackToTable_Click()
        {
            _mainMenuUIView.BackToTable(mainMenuGameObjects, placeholderView);
            _mainMenuUIView.SetGameObjectsBehavior(mainMenuGameObjects, menuGameObjects, gameDataGameObjects, placeholders, true);
        }
        
        
        #endregion
        
        #region GameDataPaper LoadOrOverrideSave

        /// <summary>
        /// Action to load a game
        /// </summary>
        public void LoadOrOverrideSave_Click()
        {
            var holders = placeholderView.GetComponentsInChildren<Image>();
            var buttonLoadGameText = gameDataGameObjects[0].GetComponentInChildren<Text>();
            GameDataInfoModel.SetPlaceholderNum(holders);

            if (GameDataInfoModel.Placeholder == -1)
            {
                var key = buttonLoadGameText.text.Equals("LOAD")? LocalizationKeyController.SaveFileErrorLabelLoadCaptionKey : LocalizationKeyController.SaveFileErrorLabelOverrideCaptionKey;
                errorLabel.GetComponentInChildren<TextMeshProUGUI>().text = LocalizationManager.GetLocalizedValue(key);
                errorLabel.SetActive(true);
                return;
            }
            
            _mainMenuUIView.LoadOrOverrideSave(buttonLoadGameText);
            
            if (GameManager.Gm.ActiveScene == 2)
                GameDataController.Gdc.LoadSelectedGame();
            
            errorLabel.SetActive(false);
        }

        #endregion
        
        #region GameDataPaper Remove Data
        
        /// <summary>
        /// Action to set the Message box for removing data
        /// </summary>
        public void Remove_Click()
        {
            UIManager.Uim.SetMessageBoxProperties(RemoveData_Click, "Remove Data", LocalizationManager.GetLocalizedValue(LocalizationKeyController.MessageBoxText2CaptionKey));
            var holders = placeholderView.GetComponentsInChildren<Image>();
            _componentView.RemoveDataAction(UIManager.Uim.messageBox, holders, errorLabel);
        }
        
        /// <summary>
        /// Action to remove data
        /// </summary>
        private void RemoveData_Click()
        {
            _mainMenuUIView.RemoveData(_saveDataPath, gameDataGameObjects[1], placeholders, UIManager.Uim.messageBox);
        }

        /// <summary>
        /// Enables the Remove Button in theSave slot panel when there are any 
        /// </summary>
        private void EnableRemoveDataButton()
        {
            var files = Directory.GetFiles(_saveDataPath);   
            gameDataGameObjects[1].GetComponent<Button>().enabled = files.Any();
        }
        
        #endregion
        
        #region Settings

        /// <summary>
        /// Action to open the settings panel
        /// </summary>
        public void OpenSettings_Click()
        {
            _mainMenuUIView.OpenSettings(settingsPanel, mainMenuGameObjects, menuGameObjects);
            SettingsModel.LoadSettings();
            UIManager.Uim.DisplayInGameSettings_Click();
        }
        
        /// <summary>
        /// Action to close the settings panel
        /// </summary>
        public void CloseSettings_Click()
        {
            _mainMenuUIView.CloseSettings(settingsPanel, mainMenuGameObjects, menuGameObjects);
        }
        
        #endregion
    }
}