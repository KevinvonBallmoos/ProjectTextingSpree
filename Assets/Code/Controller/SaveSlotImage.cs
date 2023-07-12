using UnityEngine;
using UnityEngine.UI;

namespace Code.Controller
{
    /// <summary>
    /// When a save slot is clicked
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">05.05.2023</para>
    public class SaveSlotImage : MonoBehaviour
    {
        // Slot view
        public GameObject slotView;

        /// <summary>
        /// Sets the select Image, when a save slot is selected.
        /// </summary>
        public void GameObject_Click()
        {
           var slots = slotView.GetComponentsInChildren<Image>();
           for (var i = 0; i < slots.Length; i++)
           {
               if (i is 1 or 3 or 5)
                   slots[i].enabled = false;
           }

           gameObject.GetComponentsInChildren<Image>()[1].enabled = true;
        }
    }
}