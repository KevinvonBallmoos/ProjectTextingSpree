using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Code.UI
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "Item", menuName = "Item/baseItem")]
    public class Item : ScriptableObject
    {
        public Sprite _itemIcon = null;
        // I instanciate the name variable here because the ScriptibaleObject already has a variable called name
        new public string name = "Default Item";

        
        /// <summary>
        /// Virtual means I can overwrite this function in another class once instanciated.
        /// This function is called when the item is actually clicked on.
        /// </summary>
        public virtual void Use()
        {
            Debug.Log("Using: " + name);
        }
        
    }
    
}

