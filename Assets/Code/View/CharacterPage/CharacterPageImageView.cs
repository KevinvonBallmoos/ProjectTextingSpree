using Code.View.SceneUIManager;
using UnityEngine;

namespace Code.View.CharacterPage
{
    /// <summary>
    /// Enables the Image on the clicked Character
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">29.04.23</para>
    public class CharacterPageImageView : MonoBehaviour
    {
        /// <summary>
        /// Sets the character Field, with the title of the selected Character
        /// </summary>
        public void Character_Click()
        {
            CharacterPageUIManager.CpUim.Character_Click(gameObject);
        }
    }
}