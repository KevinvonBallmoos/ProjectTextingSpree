using Code.View.Base;
using Code.View.ControlElements;

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
        private ControlView _controlView;
        
        #region Awake and Start

        /// <summary>
        /// Awake of the CharacterPageUIManager instance
        /// Sets a new Instance of the ControlView
        /// </summary>
        private void Awake()
        {
            if (SUim == null)
                SUim = this;
            _controlView = UIManager.Uim.controlView; 
        }
        
        #endregion
        
        #region Initialize UI
        
        /// <summary>
        /// Initializes UI components, depending on the scene that is loaded.
        /// </summary>
        public void InitializeUI()
        {

        }
        
        #endregion
        
        #region Story Image

        public void SwitchToStoryImage_OnClick()
        {
            _controlView.SwitchToStoryImage(menuGroupObjects);
        }
        
        #endregion
        
        #region Story Pages
        
        /// <summary>
        /// When the next button is clicked, it loads the next part of the story
        /// </summary>
        private void Next_Click()
        {
        }
        
        /// <summary>
        /// Scrolls back one page
        /// </summary>
        public void ScrollBack_Click()
        {
        }

        /// <summary>
        /// Scrolls back to the last choice, but the already selected, cannot be chosen again
        /// </summary>
        public void ScrollBackGameOver_Click()
        {
            
        }
        
        #endregion
        
        #region Story Chapter
        
        #endregion
        
        #region Story Part
        
        #endregion
    }
}