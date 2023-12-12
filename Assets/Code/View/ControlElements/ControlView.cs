using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Code.Controller.GameController;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using Code.Model.Files;
using Code.Model.GameData;
using Code.View.Base;
using TMPro;

namespace Code.View.ControlElements
{
    public class ControlView : ComponentBase
    {
        // ControlView instance
        public static ControlView Cv;
        
        #region Start method
        
        /// <summary>
        /// Awake of the UIManager
        /// Sets a new Instance of the ControlView
        /// </summary>
        private void Awake()
        {
            if (Cv == null)
                Cv = this;
        }
        
        private void Start()
        {
            if (GameManager.Gm.ActiveScene != 0) return;
            InitializeSaveDataPanel("LOAD", 0, true);
        }
        
        #endregion
        
        #region Game States

        /// <summary>
        /// Opens the character select window and disables the select Images
        /// </summary>
        public void NewGame()
        {
            foreach (var c in characters)
            {
                var image = c.GetComponentsInChildren<Image>()[2];
                image.enabled = false;
                // Sets the scrollbar to the top
                var scrollbar = c.GetComponentInChildren<Scrollbar>();
                scrollbar.value = 1;
            }
        }

        /// <summary>
        /// Checks if a character was selected and a Name was given
        /// Starts a new game and checks if a save placeholder is empty, else asks to override another placeholder
        /// </summary>
        public void BookButtonStartNewGame()
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
                GameManager.Gm.ActiveScene = 1;
                GameManager.Gm.LoadScene();
            }
            else
            {
                InitializeSaveDataPanel("Override", 1, false);
                screenObjects[2].SetActive(false);
                UIManager.Uim.SetMessageBoxProperties(UIManager.Uim.Continue_Click, "Continue", XmlModel.GetMessageBoxText(0));
                screenObjects[1].SetActive(true);
            }
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
        
        #region Load Game Data onto save file

        /// <summary>
        /// Initializes the Savedata panel
        /// Disables the check Images of all placeholders
        /// Sets the Button text and the overview text
        /// </summary>
        /// <param name="text">Text for the Button, either Load Game or New Game</param>
        /// <param name="index">Identifies which text from the xml file should be displayed</param>
        /// <param name="loadData">True => loads data into placeholders, False => only initializes the save data panel</param>
        public void InitializeSaveDataPanel(string text, int index, bool loadData)
        {
            var holders = placeholderView.GetComponentsInChildren<Image>();
            for (var i = 0; i < holders.Length; i++)
            {
                if (i is 1 or 3 or 5)
                    holders[i].enabled = false;
            }
            buttonLoadGameText.text = text;
            placeholderView.GetComponentsInChildren<Text>()[0].text = XmlModel.GetInformationText(index);
            
            if (loadData)
                LoadDataIntoPlaceholders();
        }
        

        /// <summary>
        /// Gets all game data files and stores the data in the placeholders
        /// </summary>
        public void LoadDataIntoPlaceholders()
        {
            var loadedGameData= GameDataModel.GetLoadedData();
            
            for (var i = 0; i < 3; i++)
            {
                UpdatePlaceholderView(i, i < loadedGameData.Count ? loadedGameData : null);
            }
        }

        /// <summary>
        /// Updates the Placeholder view with data.
        /// If loaded data is null, then the placeholder is empty
        /// </summary>
        /// <param name="placeholderNum">Placeholder number where the data has to be placed</param>
        /// <param name="loadedData">List of all loaded data to display</param>
        private void UpdatePlaceholderView(int placeholderNum, IReadOnlyList<GameDataModel> loadedData)
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
        
        #region Load or override Save

        /// <summary>
        /// Starts either a new game or loads a selected one 
        /// </summary>
        /// <param name="loadGameText">text that is on the button left on the save file</param>
        public void LoadOrOverrideSave(string loadGameText)
        {
            switch (loadGameText)
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
        
        #region Messagebox
        
        /// <summary>
        /// Sets the properties of the MessageBox
        /// </summary>
        /// <param name="eventMethod">Listener to add to the Button</param>
        /// <param name="buttonText">Button left text</param>
        /// <param name="text">Message Box text</param>
        public void SetMessageBoxProperties(UnityAction eventMethod, string buttonText, string text)
        {
            messageBox[0].GetComponent<Button>().onClick.RemoveAllListeners();
            messageBox[0].GetComponent<Button>().onClick.AddListener(eventMethod);
            messageBox[0].GetComponentInChildren<Text>().text = buttonText;
            messageBox[1].GetComponent<Text>().text = text;
        }
        
        /// <summary>
        /// When continue is clicked, the User can choose a save to override the old data with the new Game
        /// [0]: Enables the title screen
        /// [1]: Disables the messagebox
        /// </summary>
        public void ContinueAction()
        {
            screenObjects[0].SetActive(true);
            screenObjects[1].SetActive(false);
        }
        
        /// <summary>
        /// Action to cancel the Messagebox
        /// [0]: Enables the title screen
        /// [1]: Disables the messagebox
        /// </summary>
        public void CancelAction()
        {
            screenObjects[0].SetActive(true);
            screenObjects[1].SetActive(false);
        }

        /// <summary>
        /// Display the messagebox with the according text, to remove a selected save
        /// [1]: Enables the messagebox
        /// </summary>
        /// <param name="screenObject">Main Menu, Message Box and Character Screen Objects</param>
        /// <param name="holders">controls that hold images</param>
        public void RemoveDataAction(GameObject screenObject, Image[] holders)
        {
            GameDataInfoModel.SetPlaceholderNum(holders);
            var placeholder = GameDataInfoModel.Placeholder;
            if (placeholder == -1)
                return;
            screenObject.SetActive(true);
            // TODO Kevin: Check if empty placeholder, true => return
        }
        
        #endregion
        
        #region Character Page Top Bar Buttons
        
        /// <summary>
        /// Displays the 2nd Character Page
        /// [0]: First character Page
        /// [1]: Second character Page
        /// </summary>
        public void ScrollNextCharacterPage()
        {
            characterPages[0].SetActive(false);
            characterPages[1].SetActive(true);
            ChangeButtonProperties(UIManager.Uim.ScrollPreviousCharacterPage_CLick, "Go back", false);
        }

        /// <summary>
        /// Displays the 1st Character Page
        /// [0]: First character Page
        /// [1]: Second character Page
        /// </summary>
        public void ScrollPreviousCharacterPage()
        {
            characterPages[0].SetActive(true);
            characterPages[1].SetActive(false);
            ChangeButtonProperties(GameManager.Gm.BackToMainMenu_Click, "Back to Menu", true);
        }

        /// <summary>
        /// Adds a listener to a specific button
        /// </summary>
        /// <param name="button">Button to add the event</param>
        /// <param name="eventMethod">Event to add to the button</param>
        public void AddButtonListener(Button button, UnityAction eventMethod)
        {
            button.onClick.AddListener(eventMethod);
        }

        /// <summary>
        /// Removes all Listeners on the Button
        /// Adds a new Listener
        /// Sets the Button Text
        /// </summary>
        /// <param name="eventMethod">Listener Method to add to the Button</param>
        /// <param name="text">For the Button caption</param>
        /// <param name="isEnabled">If character page 2 is active, the Button in the top right corner is disabled</param>
        private void ChangeButtonProperties(UnityAction eventMethod, string text, bool isEnabled)
        {
            buttons[0].onClick.RemoveAllListeners();
            buttons[0].onClick.AddListener(eventMethod);
            buttons[0].GetComponentInChildren<Text>().text = text;
            // On Character Page 2 this Button is disabled
            buttons[1].gameObject.SetActive(isEnabled);
        }
        
        #endregion
        
        #region Remove Data

        /// <summary>
        /// Searches the selected Data and deletes the according File
        /// </summary>
        /// <param name="saveDataPath">Path where the save data files are</param>
        public void RemoveData(string saveDataPath)
        {
            var placeholder = GameDataInfoModel.Placeholder;
            var files = Directory.GetFiles(saveDataPath);
            
            if (placeholder >= files.Length)
                placeholder = files.Length - 1;
            
            // Deletes the file
            FileIOModel.DeleteFile(files[placeholder]);
            // Updates the placeholder view
            LoadDataIntoPlaceholders();
            // Sorts the other save files
            FileIOModel.SortSaveFiles();

            removeData.enabled = Directory.GetFiles(saveDataPath).Any();
        }

        #endregion
    }
}