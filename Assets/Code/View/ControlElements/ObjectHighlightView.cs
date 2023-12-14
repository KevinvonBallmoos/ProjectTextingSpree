using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code.View.ControlElements
{
    /// <summary>
    /// Highlights an object when the mouse hovers over it
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">30.01.2023</para>
    public class ObjectHighlightView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // Hover label text component
        [SerializeField] private Text hoverLabel;
        // Text for the label
        [SerializeField] private string labelText;

        /// <summary>
        /// Mouse entered the object
        /// </summary>
        /// <param name="eventData">event data of the mouse enter</param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            hoverLabel.text = labelText;
        }

        /// <summary>
        /// Mouse exited the object
        /// </summary>
        /// <param name="eventData">event data of the mouse exit</param>
        public void OnPointerExit(PointerEventData eventData)
        {
            hoverLabel.text = "...";
        }
    }
}