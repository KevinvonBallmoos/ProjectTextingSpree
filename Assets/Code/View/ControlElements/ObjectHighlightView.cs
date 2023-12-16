using System;
using Code.Controller.LocalizationController;
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
        // Image for the outline
        [SerializeField] private Image outlineImage;
        // Materials
        [SerializeField] private Material[] materials;
        // Localized key
        private string localizedKey;

        /// <summary>
        /// Gets the localized key, for the game object where this script is on
        /// </summary>
        private void Awake()
        {
            localizedKey = gameObject.name switch
            {
                "GameBook" => LocalizationKeyController.MenuInformationLabelNewGameKey,
                "GameDataPaperImage" => LocalizationKeyController.MenuInformationLabelLoadGameKey,
                "SettingsImage" => LocalizationKeyController.MenuInformationLabelSettingsKey,
                "Quit" => LocalizationKeyController.MenuInformationLabelQuitKey,
                _ => localizedKey
            };
        }

        /// <summary>
        /// Mouse entered the object
        /// </summary>
        /// <param name="eventData">event data of the mouse enter</param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            hoverLabel.text = LocalizationManager.GetLocalizedValue(localizedKey);
            outlineImage.material = materials[1];
        }

        /// <summary>
        /// Mouse exited the object
        /// </summary>
        /// <param name="eventData">event data of the mouse exit</param>
        public void OnPointerExit(PointerEventData eventData)
        {
            hoverLabel.text = "...";
            outlineImage.material = materials[0];
        }
    }
}