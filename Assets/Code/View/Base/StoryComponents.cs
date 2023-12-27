using UnityEngine;
using UnityEngine.UI;

namespace Code.View.Base
{
    /// <summary>
    /// Abstract class provides fields for the Story
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">23.12.2023</para>
    public abstract class StoryComponents : MonoBehaviour, IComponentBase
    {
        [Header("NewStory Scene / StoryScene 1 + 2")]
        [Header("Image Title, SwitchImages Button, Map, StoryImage, Inventory")]
        // Menu group objects
        [SerializeField] protected GameObject[] menuGroupObjects;
        // Text Control that holds the story text
        [Header("Story Text, Scrollbar")]
        [SerializeField] protected GameObject[] storyObjects;
        // Choice objects
        [Header("Choice Root and Choice Button Prefab")]
        [SerializeField] protected Transform choiceRoot;
        [SerializeField] protected GameObject choicePrefab;
        // Buttons
        [Header("Next Button, Page Back Button")] 
        [SerializeField] protected GameObject[] storyButtonObjects;
        // Object for the save animation
        [Header("Save Status")]
        [SerializeField] protected GameObject saveStatus;
        
        #region Inherited from IComponentBase
        
        [field: Header("MESSAGE BOX")]
        // MessageBox
        [field: Header("MessageBox, Button Continue (left one), Message Box Text")]
        public GameObject MessageBox { get; set; }
        public GameObject[] MessageBoxGameObjects { get; set; }
        [field: Header("MATERIALS")]
        // Materials
        [field: Header("Material")]
        public Material DefaultMaterial { get; set; }
        
        #endregion
    }
}