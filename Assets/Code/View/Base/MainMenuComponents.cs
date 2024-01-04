using UnityEngine;
using UnityEngine.Serialization;

namespace Code.View.Base
{
    /// <summary>
    /// Abstract class provides fields for the Main Menu
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">23.12.2023</para>
    public abstract class MainMenuComponents : MonoBehaviour
    {
        [Header("MENU")]
        // Menu game objects
        [Header("Button Settings, Button Quit Game, Hover (info) label")]
        [SerializeField] protected GameObject[] menuGameObjects;

        // Menu main game objects
        [Header("GameDataPaper and GameBook")]
        [SerializeField] protected internal GameObject[] mainMenuGameObjects;

        [Header("GAME DATA")]
        // Game data game objects
        [Header("Button Load, Button Remove, Button Back")]
        [SerializeField] protected GameObject[] gameDataGameObjects;

        // Game data error label
        [Header("Error Label")]
        [SerializeField] protected GameObject errorLabel;

        // Game data Placeholders
        [Header("PlaceholderView and Placeholders")]
        [SerializeField] protected GameObject placeholderView;
        [SerializeField] protected GameObject[] placeholders;
        
        [Header("Settings Panel")]
        // Settings panel
        [SerializeField] protected GameObject settingsPanel;
    }
}