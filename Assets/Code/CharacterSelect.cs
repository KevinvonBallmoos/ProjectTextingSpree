using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Code
{
    public class CharacterSelect : MonoBehaviour
    {
        // Character Properties Screen
        [SerializeField] private GameObject characterPropertiesScreen;
        
        private void Character_Click()
        {
            characterPropertiesScreen.GetComponent<Text>().text = this.gameObject.GetComponent<Text>().text;
        }
    }
}