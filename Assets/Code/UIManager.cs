using Code.View.ControlElements;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Code
{
    public class UIManager : MonoBehaviour
    {
        // UI Manager instance
        public static UIManager Uim;
        // ControlView
        private ControlView _view;
        // Buttons
        [Header("TopBar Buttons")] 
        [SerializeField] public Button[] buttons;
        // Character pages
        [Header("Character Pages")]
        [SerializeField] public GameObject[] characterPages;
        // MessageBox
        [Header("Messagebox")]
        [SerializeField] private GameObject[] messageBox;
        
        #region Awake

        /// <summary>
        /// Awake of the UIManager
        /// Sets a new Instance of the ControlView
        /// </summary>
        private void Awake()
        {
            if (Uim == null)
                Uim = this;
            _view = new ControlView();
        }
        
        #endregion
        
        #region TopBar Buttons

        /// <summary>
        /// Calls the Control view to add the event method to the button
        /// </summary>
        /// <param name="button">Button to add the event</param>
        /// <param name="eventMethod">Event to add to the button</param>
        public void AddButtonListener(Button button, UnityAction eventMethod)
        {
            _view.AddButtonListener(button, eventMethod);
        }
        
        #endregion

        #region MessageBox
        
        /// <summary>
        /// Calls the Control view to set the properties of the MessageBox
        /// </summary>
        /// <param name="eventMethod">Listener to add to the Button</param>
        /// <param name="text">Message Box text</param>
        public void SetMessageBoxProperties(UnityAction eventMethod, string text)
        {
            _view.SetMessageBoxProperties(messageBox, eventMethod, text);
        }
        
        #endregion
    }
}