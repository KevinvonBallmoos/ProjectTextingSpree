using TMPro;
using UnityEngine;
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
        
        /// <summary>
        /// Loads the Character name in the invisible label
        /// So the GameDataController knows which character to save
        /// </summary>
        public void Character_Click()
        {
            chosenCharacter.text = title.text;
            chosenCharacter.enabled = false;
        }
    }
}