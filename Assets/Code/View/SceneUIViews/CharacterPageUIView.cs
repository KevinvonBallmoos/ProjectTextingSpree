using System.Collections.Generic;
using System.Text.RegularExpressions;
using Code.Controller.GameController;
using Code.Controller.LocalizationController;
using Code.View.SceneUIManager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Code.View.SceneUIViews
{
    public class CharacterPageUIView : MonoBehaviour
    {
                // Regex Pattern for InputField
        private const string RegexPattern = "^[A-Za-z0-9\\s]+$";
        
        #region GameBook Character Page
        
        /// <summary>
        /// Checks if a character was selected and a Name was given
        /// Starts a new game and checks if a save placeholder is empty, else asks to override another placeholder
        /// </summary>
        /// <param name="playerName">InputField component with the player name</param>
        /// <param name="chosenCharacter">The chosen character text component</param>
        /// <param name="characterPage">The Character page game object</param>
        /// <param name="messageBox">Message box game object</param>
        public void BookButtonStartNewGame(InputField playerName, Text chosenCharacter, GameObject characterPage, GameObject messageBox)
        {
            if (!SubmitInputField(playerName)) return;
            if (chosenCharacter.text.Equals(""))
            {
                characterPage.GetComponentsInChildren<Text>()[0].color = Color.red;
                return;
            }
            chosenCharacter.color = Color.white;
            
            if (GameDataController.Gdc.NewGame(playerName.text, chosenCharacter.text))
            {
                GameManager.Gm.SetActiveScene(2, true);
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
                ChangeButtonProperties(buttons, CharacterPageUIManager.CpUim.ScrollPreviousCharacterPage_CLick, "Go back", false);
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
                ChangeButtonProperties(buttons, CharacterPageUIManager.CpUim.BackToMainMenu_Click, "Back to Menu", true);
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
    }
}