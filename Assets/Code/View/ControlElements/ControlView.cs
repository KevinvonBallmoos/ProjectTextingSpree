using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Code.View.ControlElements
{
    public class ControlView : MonoBehaviour
    {
        /// <summary>
        /// Sets the properties of the MessageBox
        /// </summary>
        /// <param name="messageBox"></param>
        /// <param name="eventMethod">Listener to add to the Button</param>
        /// <param name="text">Message Box text</param>
        public void SetMessageBoxProperties(GameObject[] messageBox, UnityAction eventMethod, string text)
        {
            messageBox[0].GetComponent<Button>().onClick.RemoveAllListeners();
            messageBox[0].GetComponent<Button>().onClick.AddListener(eventMethod);
            messageBox[1].GetComponent<Text>().text = text;
        }
        
        #region Top Bar Buttons
        
        /// <summary>
        /// Displays the 2nd Character Page
        /// </summary>
        public void ScrollNextCharacterPage_CLick()
        {
            UIManager.Uim.characterPages[0].SetActive(false);
            UIManager.Uim.characterPages[1].SetActive(true);
            ChangeButtonProperties(UIManager.Uim.buttons, ScrollPreviousCharacterPage_CLick, "Go back", false);
        }

        /// <summary>
        /// Displays the 1st Character Page
        /// </summary>
        private void ScrollPreviousCharacterPage_CLick()
        {
            UIManager.Uim.characterPages[0].SetActive(true);
            UIManager.Uim.characterPages[1].SetActive(false);
            ChangeButtonProperties(UIManager.Uim.buttons, GameManager.Gm.BackToMainMenu_Click, "Back to Menu", true);
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