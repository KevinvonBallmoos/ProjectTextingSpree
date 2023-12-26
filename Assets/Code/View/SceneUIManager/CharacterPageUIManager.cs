using UnityEngine;

using Code.View.Base;
using Code.View.ControlElements;

namespace Code.View.SceneUIManager
{
    /// <summary>
    /// This class handles the UI Events of the Main Menu
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">23.12.2023</para>
    public class CharacterPageUIManager : CharacterPageComponents
    {
        // MainMenu UI Manager instance
        public static CharacterPageUIManager CpUim;
        // ControlView
        private ControlView _controlView;
        
        #region Awake and Start

        /// <summary>
        /// Awake of the CharacterPageUIManager instance
        /// Sets a new Instance of the ControlView
        /// </summary>
        private void Awake()
        {
            if (CpUim == null)
                CpUim = this;
            _controlView = UIManager.Uim.controlView; 
        }
        
        #endregion
        
        #region Initialize UI
        
        /// <summary>
        /// Initializes UI components, depending on the scene that is loaded.
        /// </summary>
        public void InitializeUI()
        {
            _controlView.DisableImages(characters);
            _controlView.AddButtonListener(topBarButtons[0], BackToMainMenu_Click);
            _controlView.SetScrollbarValue(characters);
        }
        
        #endregion
        
        #region Character Page Top Bar Buttons
        
        /// <summary>
        /// Hides the Message Box
        /// Loads the MainMenu Scene
        /// </summary>
        public void BackToMainMenu_Click()
        {
            UIManager.Uim.EnableOrDisableMessageBoxGameOver(false);
            UIManager.Uim.SetActiveScene(0);
            GameManager.Gm.LoadScene();
        }
        
        /// <summary>
        /// Displays the 2nd Character Page
        /// </summary>
        public void ScrollNextCharacterPage_CLick()
        {
            _controlView.ScrollNextCharacterPage(topBarButtons, characterPages);
            _controlView.SetScrollbarValue(characters);
        }

        /// <summary>
        /// Displays the 1st Character Page
        /// </summary>
        public void ScrollPreviousCharacterPage_CLick()
        {
            _controlView.ScrollPreviousCharacterPage(topBarButtons, characterPages);
            _controlView.SetScrollbarValue(characters);
        }
        
        #endregion

        #region Character Click

        /// <summary>
        /// Sets the character Field, with the title of the selected Character
        /// </summary>
        public void Character_Click(GameObject characterGameObject)
        {
            _controlView.SetImage(characters, chosenCharacter, characterGameObject);
        }

        #endregion
        
        #region Character Page Input Field

        /// <summary>
        /// Is triggered, when the value of the input field changes
        /// Compares the last entered char of the input with the regex string
        /// if the input does not match, the last entered char is removed
        /// </summary>
        public void InputField_OnValueChanged()
        {
            _controlView.ValidateInputField(playerName);
        }

        /// <summary>
        /// Is triggered when the User submits the Username
        /// It checks if the input is empty or not
        /// </summary>
        public bool InputField_OnSubmit()
        {
            return _controlView.SubmitInputField(playerName);
        }
        
        #endregion
        
        #region Character Page New Game
        
        /// <summary>
        /// Checks if a character was selected and a Name was given
        /// Starts a new game and checks if a save placeholder is empty, else asks to override another placeholder
        /// </summary>
        public void BookButtonStartNewGame_Click()
        {
            _controlView.BookButtonStartNewGame(playerName, chosenCharacter, characterPage, MessageBox);
        }
        
        #endregion
    }
}