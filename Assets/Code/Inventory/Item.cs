using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

namespace Code.Inventory
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "Item", menuName = "Item/baseItem")]
    
    public class Item : ScriptableObject
    {
        public string _name = "Default Item";
        public Sprite _icon = null;
        public string _itemDescription = "Used to progress the story";
    
        /// <summary>
        /// Virtual means I can overwrite this function in another class once instanciated.
        /// This function is called when the item is actually clicked on. Momentarely its only logging stuff.
        /// </summary>
        public virtual void Use()
        {
            Debug.Log("Hello World" + _name);
        }
    
        /// <summary>
        /// Simply returns the item description that is doing to be created on the item class itself.
        /// </summary>
        /// <returns></returns>
        public virtual string GetItemDescription()
        {
            return _itemDescription;
        }
    }
}
