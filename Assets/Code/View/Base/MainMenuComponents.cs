using UnityEngine;

namespace Code.View.Base
{
    /// <summary>
    /// Abstract class provides fields for the Main Menu
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">23.12.2023</para>
    public abstract class MainMenuComponents : MonoBehaviour, IComponentBase    
    {
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
        
        #region Inherited from IComponentBase
        
        [field: Header("MESSAGE BOX")]
        // MessageBox
        [field: Header("MessageBox, Button Continue (left one), Message Box Text")]
        [SerializeField] 
        private GameObject messageBox;
        public GameObject MessageBox
        {
            get => messageBox; 
            set => messageBox = value;
        }
        [SerializeField] 
        private GameObject[] messageBoxGameObjects;
        public GameObject[] MessageBoxGameObjects
        {
            get => messageBoxGameObjects;
            set => messageBoxGameObjects = value;
        }
        
        [field: Header("MATERIALS")]
        // Materials
        [field: Header("Material")]
        [SerializeField] 
        private Material defaultMaterial;
        public Material DefaultMaterial
        {
            get => defaultMaterial;
            set => defaultMaterial = value;
        }
        
        #endregion
    }
}