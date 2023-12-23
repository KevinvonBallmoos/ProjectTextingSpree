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
        
        #region GameDataPaper Focus

        /// <summary>
        /// Up scales the GameDataPaper so its interactable and readable
        /// Sets the game data information text, according to the override state 
        /// </summary>
        /// <param name="mainMenuGameObjects">game data paper and game book</param>
        /// <param name="placeholderView">Game object that holds all placeholders</param>
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
        
        #region GameBook Character Page
        
        /// <summary>
        /// Checks if a character was selected and a Name was given
        /// Starts a new game and checks if a save placeholder is empty, else asks to override another placeholder
        /// </summary>
        /// <param name="playerName">InputField component with the player name</param>
        /// <param name="chosenCharacter">The chosen character text component</param>
        /// <param name="characterSelect">Character Select game object</param>
        /// <param name="messageBox">message box game object</param>
        public void BookButtonStartNewGame(InputField playerName, Text chosenCharacter, GameObject characterSelect, GameObject messageBox)
        {
            if (!SubmitInputField(playerName)) return;
            if (chosenCharacter.text.Equals(""))
            {
                characterSelect.GetComponentsInChildren<Text>()[0].color = Color.red;
                return;
            }
            chosenCharacter.color = Color.white;
            
            if (GameDataController.Gdc.NewGame(playerName.text, chosenCharacter.text))
            {
                GameManager.Gm.ActiveScene = 2;
                GameManager.Gm.LoadScene();
            }
            else
            {
                UIManager.Uim.SetMessageBoxProperties(UIManager.Uim.Continue_Click, "Continue", LocalizationManager.GetLocalizedValue(LocalizationKeyController.MessageBoxText1CaptionKey));
                messageBox.SetActive(true);
            }
        }
        
        /// <summary>
        /// Disables the select Image on every character, when a character is selected
        /// Enables the select Image on the current selected Character game object
        /// </summary>
        /// <param name="characters">All character objects</param>
        /// <param name="chosenCharacter">The chosen character text component</param>
        /// <param name="characterGameObject">game object of the selected character</param>
        public void SetImage(GameObject[] characters, Text chosenCharacter, GameObject characterGameObject)
        {
            chosenCharacter.text = characterGameObject.GetComponentsInChildren<Text>()[0].text;
            DisableImages(characters);
            // Enable Image of current game object 
            characterGameObject.GetComponentsInChildren<Image>()[2].enabled = true;
        }

        /// <summary>
        /// Disable all check images and scrollbars on each character
        /// </summary>
        public void DisableImages(GameObject[] characters)
        {
            foreach (var c in characters)
            {
                c.GetComponentsInChildren<Image>()[2].enabled = false;
            }
        }

        /// <summary>
        /// Sets the scrollbar value to the top
        /// </summary>
        /// <param name="characters"></param>
        public void SetScrollbarValue(GameObject[] characters)
        {
            foreach (var c in characters)
            {
                // Sets the scrollbar to the top
                var scrollbar = c.GetComponentInChildren<Scrollbar>();
                scrollbar.value = 1;
            }
        }
        
        #endregion
        
        #region Character Page Input Field

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
        /// </summary>
        public void ContinueAction()
        {
            GameDataInfoModel.IsOverride = true;
            GameManager.Gm.LoadScene();
        }
        
        /// <summary>
        /// Action to disable the Messagebox
        /// </summary>
        /// <param name="messageBox">Message box</param>
        public void CancelAction(GameObject messageBox)
        {
            messageBox.SetActive(false);
        }

        /// <summary>
        /// Enables the messagebox with the according text, to remove a selected save
        /// </summary>
        /// <param name="messageBox">Message box</param>
        /// <param name="holders">Controls, that hold images</param>
        /// <param name="errorLabel">The error label game object</param>
        public void RemoveDataAction(GameObject messageBox, Image[] holders, GameObject errorLabel)
        {
            GameDataInfoModel.SetPlaceholderNum(holders);
            var placeholder = GameDataInfoModel.Placeholder;
            if (placeholder == -1)
            {
                errorLabel.SetActive(true);
                errorLabel.GetComponentInChildren<TextMeshProUGUI>().text = LocalizationManager.GetLocalizedValue(LocalizationKeyController.SaveFileErrorLabelRemoveCaptionKey);
                return;
            }

            errorLabel.SetActive(false);
            messageBox.SetActive(true);
        }
        
        #endregion
        
        #region Remove Data

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
        
        #region Story Image

        public void SwitchToStoryImage(GameObject[] menuGroupObjects)
        {
            menuGroupObjects[0].GetComponent<Image>().enabled = true;
        }
        
        #endregion
    }
}