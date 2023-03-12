using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Code.UI
{
    public class Inventory : MonoBehaviour
    {
    
        /// <summary>
        /// The singleton is a C# programming paradigm or pattern. It allows the creation of one of itself as well as
        /// easy access to said created instance. To learn more see the following link:
        /// https://csharpindepth.com/Articles/Singleton
        /// </summary>
        #region singleton
        
        public static Inventory inventoryInstance;
    
        private void Awake()
        {
            if (inventoryInstance == null)
            {
                inventoryInstance = this;
            }
        }
        
        #endregion
    
        /// <summary>
        /// Delegates are used to pass methods as arguments to other methods. In other words, it is methods that references
        /// other methods with a particular parameter list and return type. To learn more about it see the following link:
        /// https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/delegates/
        /// </summary>
        public delegate void OnItemChange();
        public OnItemChange _onItemChange = delegate {};
        // Holds all items currently present in the inventory
        public List<Item> _inventoryItemList = new List<Item>();

        /// <summary>
        /// This function is used to add items to the _inventoryItemList.
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(Item item)
        {
            _inventoryItemList.Add(item);
            // We invoke the actuall function. And since we set it up to be a delegate, we don't need to null check.
            _onItemChange.Invoke();
        }
        
        public void RemoveItem(Item item)
        {
            _inventoryItemList.Remove(item);
            // We invoke the actuall function. And since we set it up to be a delegate, we don't need to null check.
            _onItemChange.Invoke();
        }
    }
}

