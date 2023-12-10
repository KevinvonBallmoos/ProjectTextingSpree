using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using Code.Controller.GameController;
using Code.Model.Files;

namespace Code.View.ControlElements
{
    public class ControlView : MonoBehaviour
    {
        #region Messagebox
        
        /// <summary>
        /// Sets the properties of the MessageBox
        /// </summary>
        /// <param name="messageBox"></param>
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
        /// <param name="screenObject">Main Menu, Message Box and Character Screen Objects</param>
        public void RemoveDataAction(GameObject screenObject)
        {
            GameDataController.Gdc.SetPlaceholderNum();
            var placeholder = GameDataController.Gdc.GetPlaceholderNum();
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
        /// <param name="buttons">Character page top bar buttons</param>
        public void ScrollNextCharacterPage_CLick(Button[] buttons)
        {
            UIManager.Uim.characterPages[0].SetActive(false);
            UIManager.Uim.characterPages[1].SetActive(true);
            ChangeButtonProperties(buttons, UIManager.Uim.ScrollPreviousCharacterPage_CLick, "Go back", false);
        }

        /// <summary>
        /// Displays the 1st Character Page
        /// [0]: First character Page
        /// [1]: Second character Page
        /// </summary>
        /// <param name="buttons">Character page top bar buttons</param>
        public void ScrollPreviousCharacterPage_CLick(Button[] buttons)
        {
            UIManager.Uim.characterPages[0].SetActive(true);
            UIManager.Uim.characterPages[1].SetActive(false);
            ChangeButtonProperties(buttons, GameManager.Gm.BackToMainMenu_Click, "Back to Menu", true);
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
        public void RemoveData(string saveDataPath, Button removeData)
        {
            var placeholder = GameDataController.Gdc.GetPlaceholderNum();
            var files = Directory.GetFiles(saveDataPath);
            
            //if (files.Length <= 0) return;
            
            if (placeholder >= files.Length)
                placeholder = files.Length - 1;
            
            // Deletes the file
            FileIOModel.DeleteFile(files[placeholder]);
            // Updates the placeholder view
            GameDataController.Gdc.LoadDataIntoPlaceholders();
            // Sorts the other save files
            FileIOModel.SortSaveFiles();

            removeData.enabled = Directory.GetFiles(saveDataPath).Any();
        }

        #endregion
    }
}