using System.IO;
using System.Linq;
using Code.Controller.GameController;
using Code.Controller.LocalizationController;
using Code.Model.GameData;
using Code.View.Base;
using Code.View.ControlElements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        // ControlView
        private ControlView _controlView;
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
            _controlView = UIManager.Uim.controlView;
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
            _controlView.InitializeSaveDataPanel("LOAD", true, placeholderView, gameDataGameObjects[0],
                placeholders);
        }

        #endregion
        
        #region GameData Paper

        /// <summary>
        /// Action to upscale the GameDataPaper
        /// </summary>
        public void DisplayLoadingPaper_Click()
        { 
            _controlView.DisplayLoadingPaper(mainMenuGameObjects, placeholderView);
            mainMenuGameObjects[0].GetComponentsInChildren<Image>()[0].material = DefaultMaterial;
            _controlView.SetGameObjectsBehavior(mainMenuGameObjects, menuGameObjects, gameDataGameObjects, placeholders, false);
        }

        /// <summary>
        /// Action to go back to the main menu view
        /// </summary>
        public void BackToTable_Click()
        {
            _controlView.BackToTable(mainMenuGameObjects, placeholderView);
            _controlView.SetGameObjectsBehavior(mainMenuGameObjects, menuGameObjects, gameDataGameObjects, placeholders, true);
        }
        
        
        #endregion
        
        #region LoadOrOverrideSave

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
            
            _controlView.LoadOrOverrideSave(buttonLoadGameText);
            
            if (GameManager.Gm.ActiveScene == 2)
                GameDataController.Gdc.LoadSelectedGame();
            
            errorLabel.SetActive(false);
        }

        #endregion
        
        #region Remove Data
        
        /// <summary>
        /// Action to remove data
        /// </summary>
        public void RemoveData_Click()
        {
            _controlView.RemoveData(_saveDataPath, gameDataGameObjects[1], placeholders, MessageBox);
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
    }
}