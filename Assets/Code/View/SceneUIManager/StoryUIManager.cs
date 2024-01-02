using Code.View.Base;
using Code.View.ControlElements;
using Code.View.SceneUIViews;
using UnityEngine;
using UnityEngine.UI;

namespace Code.View.SceneUIManager
{
    /// <summary>
    /// This class handles the UI Events of the Main Menu
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">23.12.2023</para>
    public class StoryUIManager : StoryComponents
    {
        // Story UI Manager instance
        public static StoryUIManager SUim;
        // ControlView
        private ComponentView _componentView;
        // StoryUIView
        private StoryUIView _storyUIView;

        public int Chapter { get; set; }
        public int Part { get; set; }

        #region Awake and Start

        /// <summary>
        /// Awake of the CharacterPageUIManager instance
        /// Sets a new Instance of the ControlView
        /// </summary>
        private void Awake()
        {
            if (SUim == null)
                SUim = this;
            _componentView = UIManager.Uim.ComponentView;
            _storyUIView = UIManager.Uim.StoryUIView;
        }
        
        #endregion
        
        #region Initialize UI
        
        /// <summary>
        /// Initializes UI components, depending on the scene that is loaded.
        /// </summary>
        public void InitializeUI(StoryUIView storyUIView)
        {
            if (GameManager.Gm.ActiveScene is 2 or 3)
            {
                Chapter = 1;
                _storyUIView = storyUIView;
                _storyUIView.InitializeStoryUI();
            }
        }
        
        #endregion
        
        #region Story Image

        public void SwitchToStoryImage_OnClick()
        {
            _componentView.SwitchToStoryImage(menuGroupObjects);
        }
        
        #endregion
        
        #region Story Pages
        
        /// <summary>
        /// When the next button is clicked, it loads the next part of the story
        /// </summary>
        private void Next_Click()
        {
            _storyUIView.ButtonNext();
        }
        
        /// <summary>
        /// Scrolls back one page
        /// </summary>
        public void ScrollBack_Click()
        {
            _storyUIView.ScrollBack();
        }

        /// <summary>
        /// Scrolls back to the last choice, but the already selected, cannot be chosen again
        /// </summary>
        public void ScrollBackGameOver_Click()
        {
            _storyUIView.ScrollBackGameOver(UIManager.Uim.messageBox);
        }
        
        #endregion
        
        #region Story Chapter
        
        #endregion
        
        #region Story Part
        
        #endregion
        
        #region Component Getters

        /// <summary>
        /// Gets the next button game object
        /// </summary>
        /// <returns>next button</returns>
        public GameObject GetButtonNext()
        {
            return storyButtonObjects[0];
        }
        
        /// <summary>
        /// Gets the page back button game object
        /// </summary>
        /// <returns>page back button</returns>
        public GameObject GetButtonPageBack()
        {
            return storyButtonObjects[1];
        }

        /// <summary>
        /// Gets the Status game object
        /// </summary>
        /// <returns>status game object</returns>
        public GameObject GetSaveStatusObject()
        {
            return saveStatus;
        }

        /// <summary>
        /// Gets the choice root transform
        /// </summary>
        /// <returns>choice root transform</returns>
        public GameObject GetChoiceRoot()
        {
            return choiceRoot;
        }
        
        /// <summary>
        /// Gets the choice button prefab game object
        /// </summary>
        /// <returns>choice button prefab game object</returns>
        public GameObject GetChoiceButtonPrefab()
        {
            return choicePrefab;
        }

        /// <summary>
        /// Gets the story objects
        /// </summary>
        /// <returns>story and scrollbar game objects</returns>
        public GameObject[] GetStoryObjects()
        {
            return storyObjects;
        }

        /// <summary>
        /// Gets the map and story image game object
        /// </summary>
        /// <returns>map and story image game object</returns>
        public GameObject[] GetMenuGroupObjects()
        {
            return menuGroupObjects;
        }
        
        #endregion
    }
}