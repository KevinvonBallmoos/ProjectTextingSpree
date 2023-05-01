using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code
{
    public class CharacterSelect : MonoBehaviour
    {
        // Character chosen
        [SerializeField] private Text chosenCharacter;
        // Title of Character
        [SerializeField] private TextMeshProUGUI title;

        public void Character_Click()
        {
            chosenCharacter.text = title.text;
        }
    }
}