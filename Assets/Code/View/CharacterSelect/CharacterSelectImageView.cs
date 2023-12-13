using UnityEngine;

namespace Code.View.CharacterSelect
{
    /// <summary>
    /// Enables the Image on the clicked Character
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">29.04.23</para>
    public class CharacterSelectImageView : MonoBehaviour
    {
        /// <summary>
        /// Sets the character Field, with the title of the selected Character
        /// </summary>
        public void Character_Click()
        {
            UIManager.Uim.Character_Click(gameObject);
        }
    }
}