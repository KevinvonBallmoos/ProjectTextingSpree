using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Controller
{
    /// <summary>
    /// When a Character is clicked
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">29.04.23</para>
    public class CharacterSelect : MonoBehaviour
    {
        // Character chosen
        [SerializeField] private Text chosenCharacter;
        // Title of Character
        [SerializeField] private TextMeshProUGUI characterTitle;
        // Slot view
        private GameObject[] _characters;

        /// <summary>
        /// Loads the Character name in the invisible label
        /// So the GameDataController knows which character to save
        /// </summary>
        public void Character_Click()
        {
            chosenCharacter.text = characterTitle.text;
            //chosenCharacter.enabled = false;
            
            SetImage();
        }

        /// <summary>
        /// Sets the select Image, when a character is selected
        /// </summary>
        private void SetImage()
        {
            _characters = GameManager.Gm.characters;
            foreach (var c in _characters)
            {
                var image = c.GetComponentsInChildren<Image>()[2];
                image.enabled = false;
            }
            gameObject.GetComponentsInChildren<Image>()[2].enabled = true;
        }
    }
}