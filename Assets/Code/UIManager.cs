using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using Code.Model.Files;
using Code.View.ControlElements;
using Code.View.GameData;

namespace Code
{
    public class UIManager : MonoBehaviour
    {
        // UI Manager instance
        public static UIManager Uim;
        // ControlView
        private ControlView _controlView;
        // TopBar Buttons
        [Header("Character Page TopBar Buttons")] 
        [SerializeField] public Button[] buttons;
        // Character pages
        [Header("Character Pages")]
        [SerializeField] public GameObject[] characterPages;
        // MessageBox, Button and Text
        [Header("Messagebox")]
        [SerializeField] private GameObject[] messageBox;        
        // Main Menu, Message Box and Character Screen Objects
        [Header("Main Menu, Message Box and Character Screens")]
        [SerializeField] private GameObject[] screenObjects;
        // Remove data button
        [Header("Remove Data Button")] [SerializeField]
        private Button removeData;
        
        // Path to the Save files
        private static string SaveDataPath;
        
        #region Awake

        /// <summary>
        /// Awake of the UIManager
        /// Sets a new Instance of the ControlView
        /// </summary>
        private void Awake()
        {
            if (Uim == null)
                Uim = this;
            _controlView = gameObject.AddComponent<ControlView>();
            SaveDataPath = Application.persistentDataPath + "/SaveData";
            
            if (GameManager.ActiveScene == 0)
                EnableRemoveDataButton();
        }
        
        #endregion
        
        #region Character Page Top Bar Buttons

        /// <summary>
        /// Calls the Control view to add the event method to the button
        /// </summary>
        /// <param name="button">Button to add the event</param>
        /// <param name="eventMethod">Event to add to the button</param>
        public void AddButtonListener(Button button, UnityAction eventMethod)
        {
            _controlView.AddButtonListener(button, eventMethod);
        }
        
        /// <summary>
        /// Displays the 2nd Character Page
        /// </summary>
        public void ScrollNextCharacterPage_CLick()
        {
            _controlView.ScrollNextCharacterPage_CLick(buttons);
        }

        /// <summary>
        /// Displays the 1st Character Page
        /// </summary>
        public void ScrollPreviousCharacterPage_CLick()
        {
            _controlView.ScrollPreviousCharacterPage_CLick(buttons);
        }
        
        #endregion

        #region MessageBox
        
        /// <summary>
        /// Calls the Control view to set the properties of the MessageBox
        /// </summary>
        /// <param name="eventMethod">Listener to add to the Button</param>
        /// <param name="buttonText">Button left text</param>
        /// <param name="text">Message Box text</param>
        public void SetMessageBoxProperties(UnityAction eventMethod, string buttonText, string text)
        {
            _controlView.SetMessageBoxProperties(messageBox, eventMethod, buttonText,text);
        }

        /// <summary>
        /// Action to continue the override of the savedata process
        /// </summary>
        public void Continue_Click()
        {
            _controlView.ContinueAction(screenObjects);
        }

        /// <summary>
        /// Action to cancel the Messagebox
        /// </summary>
        public void Cancel_CLick()
        {
            _controlView.CancelAction(screenObjects);
        }
        
        /// <summary>
        /// Action to set the Message box for removing data
        /// </summary>
        public void Remove_Click()
        {
            SetMessageBoxProperties(RemoveData_Click, "Remove Data", XmlModel.GetMessageBoxText(1));
            _controlView.RemoveDataAction(screenObjects[1]);
        }
        
        #endregion
        
        #region Remove Data
        
        /// <summary>
        /// Action to remove data
        /// </summary>
        private void RemoveData_Click()
        {
            _controlView.RemoveData(SaveDataPath, removeData);
        }

        /// <summary>
        /// Enables the Remove Button in theSave slot panel when there are any 
        /// </summary>
        private void EnableRemoveDataButton()
        {
            var files = Directory.GetFiles(SaveDataPath);   
            //removeData = GameObject.FindGameObjectWithTag("RemoveData").GetComponent<Button>();
            removeData.enabled = files.Any();
        }
        
        #endregion
    }
}