using UnityEngine;
using UnityEngine.UI;

namespace Code.View.Base
{
    /// <summary>
    /// Abstract class provides fields for the Character Page
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">23.12.2023</para>
    public abstract class CharacterPageComponents : MonoBehaviour, IComponentBase
    {
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