using UnityEngine;
using UnityEngine.UI;

namespace Code.View.Base
{
    /// <summary>
    /// Abstract class provides fields for the Story
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">23.12.2023</para>
    public abstract class StoryComponents : MonoBehaviour
    {
        [Header("NewStory Scene / StoryScene 1 + 2")]
        [Header("Image Title, SwitchImages Button, Map, StoryImage, Inventory")]
        // Menu group objects
        [SerializeField] protected GameObject[] menuGroupObjects;
        // Text Control that holds the story text
        [Header("Story Text, Scrollbar, Chapter Title")]
        [SerializeField] protected GameObject[] storyObjects;
        // Choice objects
        [Header("Choice Root GameObject and Choice Button Prefab")]
        [SerializeField] protected GameObject choiceRoot;
        [SerializeField] protected GameObject choicePrefab;
        // Buttons
        [Header("Next Button, Page Back Button")] 
        [SerializeField] protected GameObject[] storyButtonObjects;
        // Object for the save animation
        [Header("Save Status")]
        [SerializeField] protected GameObject saveStatus;
                
        [Header("Settings Panel")]
        // Settings panel
        [SerializeField] protected GameObject settingsPanel;
    }
}