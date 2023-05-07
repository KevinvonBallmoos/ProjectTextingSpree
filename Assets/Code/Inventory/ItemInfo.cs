using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Inventory
{
    public class ItemInfo : MonoBehaviour
    {
        public Text _itemName;
        public Text _itemDescription;

        
        public void SetUp(string name, string description)
        {
            _itemName.text = name;
            _itemDescription.text = description;    
        }
    }
}