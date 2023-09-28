using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Inventory
{
    /// <summary>
    /// Adds and Removes Items from the Inventory list
    /// </summary>
    /// <para name="author">Kastriot Dulla</para>
    /// <para name="date">11.01.2023</para>
    public class Inventory : MonoBehaviour
    {
        // Inventory instance
        public static Inventory Instance;
        // Delegate
        public delegate void OnItemChange();
        public OnItemChange OnItemChangeDelegate = delegate {};
        // Holds all items currently present in the inventory
        public List<Item> inventoryItemList = new List<Item>();

        #region Awake
        
        /// <summary>
        /// Awake of the Inventory
        /// Assigns the Inventory so it's always the same Object and does not get destroyed when switching scenes
        /// </summary>
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        #endregion
        
        /// <summary>
        /// This function is used to add items to the _inventoryItemList.
        /// </summary>
        /// <param name="item">item to add to the list</param>
        public void AddItem(Item item)
        {
            inventoryItemList.Add(item);
            // We invoke the actual function. And since we set it up to be a delegate, we don't need to null check.
            OnItemChangeDelegate.Invoke();
        }

        /// <summary>
        /// Removes Item from the ItemList.
        /// </summary>
        /// <param name="item"></param>
        public void RemoveItem(Item item)
        {
            inventoryItemList.Remove(item);
            OnItemChangeDelegate.Invoke();
        }
        
        // The singleton is a C# programming paradigm or pattern. It allows the creation of one of itself as well as
        // easy access to said created instance. To learn more see the following link:
        // https://csharpindepth.com/Articles/Singleton
        
        // Delegates are used to pass methods as arguments to other methods. In other words, it is methods that references
        // other methods with a particular parameter list and return type. To learn more about it see the following link:
        // https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/delegates/

    }
}

