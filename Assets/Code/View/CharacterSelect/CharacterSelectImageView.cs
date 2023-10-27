using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.View.CharacterSelect
{
    /// <summary>
    /// Enables the Image on the clicked Character
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">29.04.23</para>
    public class CharacterSelectImageView : MonoBehaviour
    {
        // Character chosen
        [SerializeField] private Text character;
        // Title of Character
        [SerializeField] private TextMeshProUGUI characterName;
        // All characters
        private GameObject[] _characters;

        /// <summary>
        /// Sets the character Field, with the title of the selected Character
        /// </summary>
        public void Character_Click()
        {
            character.text = characterName.text;
            SetImage();
        }

        /// <summary>
        /// Disables the select Image on all character, when a character is selected
        /// Enables the select Image on the current Character game object
        /// </summary>
        private void SetImage()
        {
            // Disable all Images
            _characters = GameManager.Gm.characters;
            foreach (var c in _characters)
            {
                var image = c.GetComponentsInChildren<Image>()[2];
                image.enabled = false;
            }
            // Enable Image of current game object 
            gameObject.GetComponentsInChildren<Image>()[2].enabled = true;
        }
    }
}