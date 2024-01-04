using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Code.Controller.FileController;
using Code.Controller.LocalizationController;
using Code.Model.GameData;
using Code.Model.Settings;
using Code.View.ControlElements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.View.SceneUIViews
{
    /// <summary>
    /// Class handles UI methods for the Main menu Scene
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">23.12.2023</para>
    public class MainMenuUIView : MonoBehaviour
    {
        #region GameDataPaper Focus

        /// <summary>
        /// Up scales the GameDataPaper so its interactable and readable
        /// Sets the game data information text, according to the override state 
        /// </summary>
        /// <param name="mainMenuGameObjects">game data paper and game book</param>
        /// <param name="placeholderView">game object that holds all placeholders</param>
        public void DisplayLoadingPaper(GameObject[] mainMenuGameObjects, GameObject placeholderView)
        {
            var mainMenuTransform = mainMenuGameObjects[0].GetComponent<RectTransform>();
            mainMenuTransform.localRotation = new Quaternion(0, 0, 0, 0);
            mainMenuTransform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            mainMenuTransform.localPosition = new Vector3(-370, -42, 0);
            
            placeholderView.GetComponentsInChildren<Text>().Where(t => t.name.Equals("GameDataInformationText")).ToArray()[0].text =
                LocalizationManager.GetLocalizedValue(LocalizationKeyController.InformationKeys[!GameDataInfoModel.IsOverride? 0 : 1]);
        }

        /// <summary>
        /// Down scales the GameDataPaper, back to origin state
        /// Main menu view is active
        /// </summary>
        /// <param name="mainMenuGameObjects">game data paper and game book</param>
        /// <param name="placeholderView">Game object that holds all placeholders</param>
        public void BackToTable(GameObject[] mainMenuGameObjects, GameObject placeholderView)
        {
            var mainMenuTransform = mainMenuGameObjects[0].GetComponent<RectTransform>();
            mainMenuTransform.localRotation = new Quaternion(-2.86f, 0.43f, 25.22f, 112f);
            mainMenuTransform.localScale = new Vector3(0.35f, 0.41f, 0.5f);
            mainMenuTransform.localPosition = new Vector3(-634, -221, 0);
            
            placeholderView.GetComponentsInChildren<Text>().Where(t => t.name.Equals("GameDataInformationText")).ToArray()[0].text = "Game Data";
        }

        /// <summary>
        /// Sets the behavior of the other controls
        /// </summary>
        /// <param name="mainMenuGameObjects">game data paper and game book</param>
        /// <param name="menuGameObjects">button settings and quit, hover label</param>
        /// <param name="gameDataGameObjects">button load, remove and back</param>
        /// <param name="placeholders">placeholders for the game data</param>
        /// <param name="isMenuFocus">true when the menu is focused, false the the game data paper is focused</param>
        public void SetGameObjectsBehavior(GameObject[] mainMenuGameObjects, GameObject[] menuGameObjects, GameObject[] gameDataGameObjects, GameObject[] placeholders, bool isMenuFocus)
        {
            // Main menu game objects 
            mainMenuGameObjects[0].GetComponent<Button>().enabled = isMenuFocus;
            mainMenuGameObjects[0].GetComponentInChildren<ObjectHighlightView>().enabled = isMenuFocus;
            mainMenuGameObjects[1].GetComponent<ObjectHighlightView>().enabled = isMenuFocus;
            mainMenuGameObjects[1].GetComponentInChildren<Button>().enabled = isMenuFocus;
            
            // Menu game objects
            menuGameObjects[0].GetComponentInChildren<ObjectHighlightView>().enabled = isMenuFocus;
            menuGameObjects[1].SetActive(isMenuFocus);
            menuGameObjects[2].SetActive(isMenuFocus);
            
            // Game data game objects
            gameDataGameObjects[0].GetComponent<Image>().raycastTarget = !isMenuFocus;
            gameDataGameObjects[0].GetComponentInChildren<Text>().raycastTarget = !isMenuFocus;
            gameDataGameObjects[1].GetComponent<Image>().raycastTarget = !isMenuFocus;
            gameDataGameObjects[1].GetComponentInChildren<Text>().raycastTarget = !isMenuFocus;
            gameDataGameObjects[2].SetActive(!isMenuFocus);
            
            // Ray cast target, (to enable recognize mouse events)
            foreach (var p in placeholders)
                p.GetComponent<Image>().raycastTarget = !isMenuFocus;
        }
        
        #endregion
        
        #region GameDataPaper Load Game Data

        /// <summary>
        /// Initializes the Savedata panel
        /// Disables the check Images of all placeholders
        /// Sets the Button text and the overview text
        /// </summary>
        /// <param name="text">Text for the Button, either Load Game or New Game</param>
        /// <param name="loadData">True => loads data into placeholders, False => only initializes the save data panel</param>
        /// <param name="placeholderView">Game object that holds all placeholders"</param>
        /// <param name="buttonLoadGame">button load game object"</param>
        /// <param name="placeholders">placeholders for the game data</param>
        public void InitializeSaveDataPanel(string text, bool loadData, GameObject placeholderView, GameObject buttonLoadGame, GameObject[] placeholders)
        {
            var holders = placeholderView.GetComponentsInChildren<Image>()
                .Where(c => c.name.Equals("CheckImage")).ToList();
            foreach (var t in holders)
            {
                t.enabled = false;
            }
            buttonLoadGame.GetComponentInChildren<Text>().text = text;
            
            if (loadData)
                LoadDataIntoPlaceholders(placeholders);
        }
        
        /// <summary>
        /// Gets all game data files and stores the data in the placeholders
        /// </summary>
        /// <param name="placeholders">placeholders for the game data</param>
        public void LoadDataIntoPlaceholders(GameObject[] placeholders)
        {
            var loadedGameData= GameDataModel.GetLoadedData();
            
            for (var i = 0; i < 3; i++)
            {
                UpdatePlaceholderView(placeholders, i, i < loadedGameData.Count ? loadedGameData : null);
            }
        }

        /// <summary>
        /// Updates the Placeholder view with data.
        /// If loaded data is null, then the placeholder is empty
        /// </summary>
        /// <param name="placeholders">placeholders for the game data</param>
        /// <param name="placeholderNum">Placeholder number where the data has to be placed</param>
        /// <param name="loadedData">List of all loaded data to display</param>
        private void UpdatePlaceholderView(GameObject[] placeholders, int placeholderNum, IReadOnlyList<GameDataModel> loadedData)
        {
            var placeholderObject = placeholderNum switch
            {
                0 => placeholders[0],
                1 => placeholders[1],
                2 => placeholders[2],
                _ => null
            };

            for (var i = 0; i < 5; i++)
            {
                if (placeholderObject == null) continue;
                
                var time = DateTime.Now;
                if (loadedData?[placeholderNum].TimeOfSave != null) 
                    time = DateTime.ParseExact(loadedData[placeholderNum].TimeOfSave, "yyyy-dd-M--HH-mm-ss", CultureInfo.InvariantCulture);
                
                var obj = placeholderObject.transform.GetChild(i).gameObject;
                var text = i switch
                {
                    0 => loadedData != null ? $"Player: {loadedData[placeholderNum].PlayerName}" : "Player: No data",
                    1 => loadedData != null ? $"Chapter: {loadedData[placeholderNum].Title}" : "Chapter: No data saved",
                    2 => loadedData != null ? $"Completion: {loadedData[placeholderNum].ProgressPercentage} %" : "Completion: ... %",
                    3 => loadedData != null ? $"Time of last Save: \n{time}" : "Time of last Save: No data",
                    4 => loadedData != null ? $"Time spent in Game: {loadedData[placeholderNum].TimeSpent}" : "Time spent in Game: 00:00:00",
                    _ => obj.GetComponent<TextMeshProUGUI>().text
                };
                obj.GetComponent<TextMeshProUGUI>().text = text;
            }
        }
        
        #endregion
        
        #region GameDataPaper Load or Override Save

        /// <summary>
        /// Starts either a new game or loads a selected one 
        /// </summary>
        /// <param name="buttonLoadGameText">Text component that is on the button left on the save file</param>
        public void LoadOrOverrideSave(Text buttonLoadGameText)
        {
            switch (buttonLoadGameText.text)
            {
                case "LOAD":
                    GameManager.Gm.ActiveScene = 2;
                    break;
                case "Override":
                    GameManager.Gm.ActiveScene = 1;
                    GameManager.Gm.LoadScene();
                    break;
            }
        }
        
        #endregion
        
        #region GameDataPaper Remove Data

        /// <summary>
        /// Searches the selected Data and deletes the according File
        /// </summary>
        /// <param name="saveDataPath">Path where the save data files are</param>
        /// <param name="removeData">GameObject Remove Data Button</param>
        /// <param name="placeholders">Placeholders for the game data</param>
        /// <param name="messageBox">Message box</param>
        public void RemoveData(string saveDataPath, GameObject removeData, GameObject[] placeholders, GameObject messageBox)
        {
            var placeholder = GameDataInfoModel.Placeholder;
            var files = Directory.GetFiles(saveDataPath);
            
            if (placeholder >= files.Length)
                placeholder = files.Length - 1;
            
            // Deletes the file
            FileIOController.DeleteFile(files[placeholder]);
            // Updates the placeholder view
            LoadDataIntoPlaceholders(placeholders);
            // Sorts the other save files
            FileIOController.SortSaveFiles();

            messageBox.SetActive(false);
            removeData.GetComponent<Button>().enabled = Directory.GetFiles(saveDataPath).Any();
        }

        #endregion
        
        #region Settings Panel Focus
        
        /// <summary>
        /// Opens the settings and disables the hover and click events of
        /// the game book, game data paper, quit and settings game object
        /// </summary>
        /// <param name="settingsPanel">settings panel</param>
        /// <param name="mainMenuGameObjects">game data paper and game book</param>
        /// <param name="menuGameObjects">button settings and quit, hover label</param>
        public void OpenSettings(GameObject settingsPanel, GameObject[] mainMenuGameObjects, GameObject[] menuGameObjects)
        {
            SetGameObjectsBehavior(settingsPanel, mainMenuGameObjects, menuGameObjects, false);
        }

        /// <summary>
        /// Down scales the GameDataPaper, back to origin state
        /// Main menu view is active
        /// </summary>
        /// <param name="settingsPanel">settings panel</param>
        /// <param name="mainMenuGameObjects">game data paper and game book</param>
        /// <param name="menuGameObjects">button settings and quit, hover label</param>
        public void CloseSettings(GameObject settingsPanel, GameObject[] mainMenuGameObjects, GameObject[] menuGameObjects)
        {
            SetGameObjectsBehavior(settingsPanel, mainMenuGameObjects, menuGameObjects, true);
        }

        /// <summary>
        /// Sets the behavior of the other controls
        /// </summary>
        /// <param name="settingsPanel">settings panel</param>
        /// <param name="mainMenuGameObjects">game data paper and game book</param>
        /// <param name="menuGameObjects">button settings and quit, hover label</param>
        /// <param name="isActive">true the settings panel will be visible, false it will be hidden</param>
        private void SetGameObjectsBehavior(GameObject settingsPanel, GameObject[] mainMenuGameObjects, GameObject[] menuGameObjects, bool isActive)
        {
            settingsPanel.SetActive(!isActive);
            // Main menu game objects 
            mainMenuGameObjects[0].GetComponent<Button>().enabled = isActive;
            mainMenuGameObjects[0].GetComponentInChildren<ObjectHighlightView>().enabled = isActive;
            mainMenuGameObjects[1].GetComponent<ObjectHighlightView>().enabled = isActive;
            mainMenuGameObjects[1].GetComponentInChildren<Button>().enabled = isActive;
            
            // Menu game objects
            menuGameObjects[0].GetComponentInChildren<ObjectHighlightView>().enabled = isActive;
            menuGameObjects[1].GetComponent<ObjectHighlightView>().enabled = isActive;
            menuGameObjects[1].SetActive(isActive);
            menuGameObjects[2].SetActive(isActive);
            
            // Ray cast target, (to disable recognize mouse events)
            mainMenuGameObjects[0].GetComponent<Image>().raycastTarget = isActive;
            foreach (var p in menuGameObjects)
                p.GetComponent<Image>().raycastTarget = isActive;
        }

        #endregion
    }
}