using UnityEngine;
using UnityEngine.UI;

namespace Code.View.Base
{
    /// <summary>
    /// Abstract class provides fields for the Character Page
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">23.12.2023</para>
    public abstract class CharacterPageComponents : MonoBehaviour
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
    }
}