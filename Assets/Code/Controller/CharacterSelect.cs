using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Code
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
        [SerializeField] private TextMeshProUGUI title;
        // Slot view
        [SerializeField] private GameObject characters;
        
        /// <summary>
        /// Loads the Character name in the invisible label
        /// So the GameDataController knows which character to save
        /// </summary>
        public void Character_Click()
        {
            chosenCharacter.text = title.text;
            chosenCharacter.enabled = false;
            
            SetImage();
        }

        private void SetImage()
        {
            var slots = characters.GetComponentsInChildren<Image>();
            for (var i = 0; i < slots.Length; i++)
            {
                if (i is 2 or 5 or 8)
                    slots[i].enabled = false;
            }
            gameObject.GetComponentsInChildren<Image>()[2].enabled = true;
        }
    }
}