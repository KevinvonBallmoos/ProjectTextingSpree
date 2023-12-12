using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.View.Base
{
    public class ComponentBase : MonoBehaviour
    {
        // TopBar Buttons
        [Header("Character Page TopBar Buttons")] 
        [SerializeField] protected  Button[] buttons;
        // Character pages
        [Header("Character Pages")]
        [SerializeField] protected  GameObject[] characterPages;
        // Character
        [Header("Character")] 
        [SerializeField] protected  GameObject[] characters;
        [SerializeField] protected  Text chosenCharacter;
        [SerializeField] protected  InputField playerName;
        // MessageBox, Button and Text
        [Header("Messagebox")]
        [SerializeField] protected  GameObject[] messageBox;       
        // Message Box Game Over Screen Object
        [Header("Game over Message Box")]
        [SerializeField] protected  GameObject messageBoxGameOver;
        // Main Menu, Message Box, Character Screen object
        [Header("Main Menu, Message Box and Character Screens")]
        [SerializeField] protected  GameObject[] screenObjects;
        // Remove data button
        [Header("Remove Data Button")] 
        [SerializeField] protected  Button removeData;
        // Load save
        [Header("Load Game Text")]
        [SerializeField] protected  Text buttonLoadGameText;
        // Error Label
        [Header("Error Label")] 
        [SerializeField] protected  TextMeshProUGUI errorLabel;
        // Savedata Placeholders
        [Header("Savedata Placeholders")]
        [SerializeField] protected  GameObject[] placeholders;
        // Placeholder view
        [Header("Placeholder view")]
        [SerializeField] protected  GameObject placeholderView;
    }
}