using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Code.View.Base
{
    public class ComponentBase : MonoBehaviour
    {
        #region Serialized Fiels Menu
        
        [Header("MAIN MENU Scene")]
        
        [Header("MENU")]
        // Menu game objects
        [Header("Button Settings, Button Quit Game, Hover (info) label")] 
        [SerializeField] protected GameObject[] menuGameObjects;
        // Menu main game objects
        [Header("GameDataPaper and GameBook")] 
        [SerializeField] protected GameObject[] mainMenuGameObjects;
        
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
        
        [Header("MESSAGE BOX")]
        // MessageBox
        [Header("MessageBox, Button Continue (left one), Message Box Text")]
        [SerializeField] protected GameObject messageBox;
        [SerializeField] protected GameObject[] messageBoxGameObjects;       
        
        [Header("MATERIALS")]
        // Materials
        [Header("Material")]
        [SerializeField] protected Material defaultMaterial;
        
        #endregion
        
        #region Serialized Fields GameBook Character Page
        
        [Header("CHARACTER Page Scene")]
        // Character select
        [Header("GameObject Character Page")]
        [SerializeField] protected GameObject characterPage;
        // Character Page Top bar buttons
        [Header("Character Page TopBar Buttons")] 
        [SerializeField] protected Button[] topBarButtons;
        // Game book character game objects
        [Header("GameObjects Character Pages and Characters, Text chosenCharacter, InputField PlayerName")]
        [SerializeField] protected GameObject[] characterPages;
        [SerializeField] protected GameObject[] characters;
        [SerializeField] protected Text chosenCharacter;
        [SerializeField] protected InputField playerName;
        
        #endregion
        
        #region Serialized Fields GameBook Story

        [Header("NewStory Scene / StoryScene 1 + 2")]
        [SerializeField] protected GameObject[] menuGroupObjects;

        #endregion
    }
}