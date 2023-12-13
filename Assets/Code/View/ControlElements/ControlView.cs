using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Code.Controller.FileController;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

using Code.Controller.GameController;
using Code.Controller.LocalizationController;
using Code.Model.GameData;

namespace Code.View.ControlElements
{
    public class ControlView : MonoBehaviour
    {
        // Regex Pattern for InputField
        private const string RegexPattern = "^[A-Za-z0-9\\s]+$";
        
        #region Game States

        /// <summary>
        /// Opens the character select window and disables the select Images
        /// </summary>
        /// <param name="characters">All character objects</param>
        public void NewGame(GameObject[] characters)
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
        /// <param name="playerName">InputField component with the player name</param>
        /// <param name="chosenCharacter">The chosen character text component</param>
        /// <param name="screenObjects">Main Menu, Message Box and Character Screen Objects</param>
        /// <param name="placeholderView">Game object that holds all placeholders"</param>
        /// <param name="buttonLoadGameText">Text control that holds the button text"</param>
        /// <param name="placeholders">placeholders for the game data</param>
        public void BookButtonStartNewGame(InputField playerName, Text chosenCharacter, GameObject[] screenObjects, GameObject placeholderView, Text buttonLoadGameText, GameObject[] placeholders)
        {
            if (!InputField_OnSubmit(playerName)) return;
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
                InitializeSaveDataPanel("Override", 1, false, placeholderView, buttonLoadGameText, placeholders);
                screenObjects[2].SetActive(false);
                UIManager.Uim.SetMessageBoxProperties(UIManager.Uim.Continue_Click, "Continue", XmlController.GetMessageBoxText(0));
                screenObjects[1].SetActive(true);
            }
        }
        
        /// <summary>
        /// Is triggered when the User submits the Username
        /// It checks if the input is empty or not
        /// </summary>
        /// <param name="playerName">InputField component with the player name</param>
        private bool InputField_OnSubmit(InputField playerName)
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
        /// <param name="placeholderView">Game object that holds all placeholders"</param>
        /// <param name="buttonLoadGameText">Text control that holds the button text"</param>
        /// <param name="placeholders">placeholders for the game data</param>
        public void InitializeSaveDataPanel(string text, int index, bool loadData, GameObject placeholderView, Text buttonLoadGameText, GameObject[] placeholders)
        {
            var holders = placeholderView.GetComponentsInChildren<Image>();
            for (var i = 0; i < holders.Length; i++)
            {
                if (i is 1 or 3 or 5)
                    holders[i].enabled = false;
            }
            buttonLoadGameText.text = text;
            placeholderView.GetComponentsInChildren<Text>()[0].text = XmlController.GetInformationText(index);
            
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
        
        #region Load or override Save

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
        
        #region Characterselect

        /// <summary>
        /// Disables the select Image on every character, when a character is selected
        /// Enables the select Image on the current selected Character game object
        /// </summary>
        /// <param name="characters">All character objects</param>
        /// <param name="chosenCharacter">The chosen character text component</param>
        /// <param name="characterGameObject">game object of the selected character</param>
        public void SetImage(GameObject[] characters, Text chosenCharacter, GameObject characterGameObject)
        {
            chosenCharacter.text = characterGameObject.GetComponentsInChildren<TextMeshProUGUI>()[0].text;
            // Disable all Images
            foreach (var c in characters)
            {
                var image = c.GetComponentsInChildren<Image>()[2];
                image.enabled = false;
            }
            // Enable Image of current game object 
            characterGameObject.GetComponentsInChildren<Image>()[2].enabled = true;
        }
        
        #endregion
        
        #region Characterselect Input Field

        /// <summary>
        /// Validates the input, matches with a regex string
        /// </summary>
        /// <param name="playerName">InputField component with the player name</param>
        public void ValidateInputField(InputField playerName)
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
        /// Submits the input
        /// </summary>
        /// <param name="playerName">InputField component with the player name</param>
        /// <returns>true when the text is not empty</returns>
        public bool SubmitInputField(InputField playerName)
        {
            if (playerName.text.Equals(""))
                playerName.GetComponentsInChildren<Text>()[0].color = Color.red;
            return !playerName.text.Equals("");
        }

        #endregion
        
        #region Messagebox
        
        /// <summary>
        /// Sets the properties of the MessageBox
        /// [0] Message box button left
        /// [1] Messagebox text
        /// </summary>
        /// <param name="messageBox">Message box component</param>
        /// <param name="eventMethod">Listener to add to the Button</param>
        /// <param name="buttonText">Button left text</param>
        /// <param name="text">Message Box text</param>
        public void SetMessageBoxProperties(GameObject[] messageBox, UnityAction eventMethod, string buttonText, string text)
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
        /// <param name="screenObjects">Main Menu, Message Box and Character Screen Objects</param>
        public void ContinueAction(GameObject[] screenObjects)
        {
            screenObjects[0].SetActive(true);
            screenObjects[1].SetActive(false);
        }
        
        /// <summary>
        /// Action to cancel the Messagebox
        /// [0]: Enables the title screen
        /// [1]: Disables the messagebox
        /// </summary>
        /// <param name="screenObjects">Main Menu, Message Box and Character Screen Objects</param>
        public void CancelAction(GameObject[] screenObjects)
        {
            screenObjects[0].SetActive(true);
            screenObjects[1].SetActive(false);
        }

        /// <summary>
        /// Display the messagebox with the according text, to remove a selected save
        /// [1]: Enables the messagebox
        /// </summary>
        /// <param name="screenObject">Message Box</param>
        /// <param name="holders">Controls, that hold images</param>
        /// <param name="errorLabel">The error label text component</param>
        public void RemoveDataAction(GameObject screenObject, Image[] holders, TextMeshProUGUI errorLabel)
        {
            GameDataInfoModel.SetPlaceholderNum(holders);
            var placeholder = GameDataInfoModel.Placeholder;
            if (placeholder == -1)
            {
                errorLabel.enabled = true;
                errorLabel.text = LocalizationManager.GetLocalizedValue(LocalizationKeyController.SaveFileErrorLabelRemoveCaptionKey);
                return;
            }

            errorLabel.enabled = false;
            screenObject.SetActive(true);
        }
        
        #endregion
        
        #region Character Page Top Bar Buttons

        /// <summary>
        /// Displays the 2nd Character Page
        /// [0]: First character Page
        /// [1]: Second character Page
        /// </summary>
        /// <param name="buttons">Character page top bar buttons</param>
        /// <param name="characterPages">Both character page components</param>
        public void ScrollNextCharacterPage(Button[] buttons, GameObject[] characterPages)
        {
            characterPages[0].SetActive(false);
            characterPages[1].SetActive(true);
            ChangeButtonProperties(buttons, UIManager.Uim.ScrollPreviousCharacterPage_CLick, "Go back", false);
        }

        /// <summary>
        /// Displays the 1st Character Page
        /// [0]: First character Page
        /// [1]: Second character Page
        /// </summary>
        /// <param name="buttons">Character page top bar buttons</param>
        /// <param name="characterPages">Both character page components</param>
        public void ScrollPreviousCharacterPage(Button[] buttons, GameObject[] characterPages)
        {
            characterPages[0].SetActive(true);
            characterPages[1].SetActive(false);
            ChangeButtonProperties(buttons, UIManager.Uim.BackToMainMenu_Click, "Back to Menu", true);
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
        /// <param name="buttons">TopBar buttons</param>
        /// <param name="eventMethod">Listener Method to add to the Button</param>
        /// <param name="text">For the Button caption</param>
        /// <param name="isEnabled">If character page 2 is active, the Button in the top right corner is disabled</param>
        private void ChangeButtonProperties(IReadOnlyList<Button> buttons, UnityAction eventMethod, string text, bool isEnabled)
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
        /// <param name="removeData">Remove Data Button</param>
        /// <param name="placeholders">Placeholders for the game data</param>
        /// <param name="screenObject">Message Box</param>
        public void RemoveData(string saveDataPath, Button removeData, GameObject[] placeholders, GameObject screenObject)
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

            screenObject.SetActive(false);
            removeData.enabled = Directory.GetFiles(saveDataPath).Any();
        }

        #endregion
    }
}