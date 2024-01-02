using System.Collections.Generic;
using System.Text.RegularExpressions;
using Code.Controller.GameController;
using Code.Controller.LocalizationController;
using Code.Model.GameData;
using Code.View.SceneUIManager;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Code.View.SceneUIViews
{
    /// <summary>
    /// This class handles the logic of ui elements
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">02.01.2024</para>
    public class ComponentView : MonoBehaviour
    {
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
        
        #region GameBook Story Image

        public void SwitchToStoryImage(GameObject[] menuGroupObjects)
        {
            menuGroupObjects[0].GetComponent<Image>().enabled = true;
        }
        
        #endregion
    }
}