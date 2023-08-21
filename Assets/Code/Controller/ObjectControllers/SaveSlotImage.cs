using UnityEngine;
using UnityEngine.UI;

namespace Code.Controller.ObjectControllers
{
    /// <summary>
    /// Enables the Image on the clicked Save slot
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">05.05.2023</para>
    public class SaveSlotImage : MonoBehaviour
    {
        // Save slot view
        public GameObject slotView;

        /// <summary>
        /// Disables the select Image on all saves, when a save is selected
        /// Enables the select Image on the current save slot game object
        /// </summary>
        public void GameObject_Click()
        { 
            // Disable all Images
            var slots = slotView.GetComponentsInChildren<Image>();
            for (var i = 0; i < slots.Length; i++)
            {
                if (i is 1 or 3 or 5)
                   slots[i].enabled = false;
            }
            // Enable Image of current game object 
            gameObject.GetComponentsInChildren<Image>()[1].enabled = true;
        }
    }
}