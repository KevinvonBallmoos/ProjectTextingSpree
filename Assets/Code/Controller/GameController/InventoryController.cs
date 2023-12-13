using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Code.Model.Inventory;

namespace Code.Controller.GameController
{
    /// <summary>
    /// Is in Control of the Inventory
    /// </summary>
    /// <para name="author">Kastriot Dulla</para>
    /// <para name="date">11.01.2023</para>
    public class InventoryController : MonoBehaviour
    {
        // Create InventoryController instance to be able to use Awake as well as all other methods in static content.
        public static InventoryController Ic;
        
        // Inventory item list
        public List<Item> itemList = new ();
        // Inventory Panel and Prefab
        public Transform inventoryPanel;
        public GameObject itemInfoPrefab;
        // Item info, (Tooltip - info?)
        private GameObject _currentItemInfo = null;
        
        private const float DelayInSeconds = 0.5f; // Set the delay in seconds
        
        #region Awake
        
        /// <summary>
        /// Awake of the Inventory
        /// Assigns the Inventory so it's always the same Object and does not get destroyed when switching scenes
        /// </summary>
        private void Awake()
        {
            if (Ic == null)
                Ic = this;
        }
        
        #endregion

        #region Add Item
        
        /// <summary>
        /// Adds the Item,
        /// </summary>
        /// <param name="item">item to add to the Inventory</param>
        public void AddItem(string item)
        {
            foreach (var i in itemList)
            {
                if (i._name.Equals(item))
                {
                    Model.Inventory.Inventory.Instance.AddItem(i);
                }
            }
        }
        
        #endregion
        
        #region Item use and info
        
        /// <summary>
        /// This function is called when we use an item that can grant stats, such as health.
        /// </summary>
        /// <param name="itemType">type of the item</param>
        /// <param name="amount">amount of items</param>
        public void OnStatItemUse(StatItemType itemType, int amount)
        {
            Debug.Log("Consuming: "+ itemType + " Added amount: "+amount);
        }
        
        /// <summary>
        /// Displays item information when the mouse is hovered over an item.
        /// </summary>
        /// <param name="itemName">name of the item</param>
        /// <param name="itemDescription">description of the item</param>
        /// <param name="buttonPos">button position</param>
        public void DisplayItemInfo(string itemName, string itemDescription, Vector2 buttonPos)
        {
            if (_currentItemInfo != null)
            {
                Destroy(_currentItemInfo.gameObject);
            }
        
            // Create variables so that the tooltip window is moved to a different direction and not simply closed
            buttonPos.x -= 100;  
            buttonPos.y += 75;
            
            _currentItemInfo = Instantiate(itemInfoPrefab, buttonPos, Quaternion.identity, inventoryPanel);
            _currentItemInfo.GetComponent<ItemInfo>().SetUp(itemName, itemDescription);
        }
        
        #endregion
        
        #region Destroy Item and GameObject
        
        /// <summary>
        /// Destroy Item info so it's not staying behind when the mouse is leaving.
        /// </summary>
        public void DestroyItemInfo()
        {
            if (_currentItemInfo == null) return;
            
            // Delay the destruction of the _currentItemInfo game object
            StartCoroutine(DelayedDestroy(_currentItemInfo.gameObject, DelayInSeconds));
            _currentItemInfo = null;
        }

        /// <summary>
        /// Destroys a specified game object
        /// </summary>
        /// <param name="gameObjectToDestroy"> game object to destroy</param>
        /// <param name="delayInSeconds">how many seconds to delay, till the destroy</param>
        /// <returns></returns>
        IEnumerator DelayedDestroy(GameObject gameObjectToDestroy, float delayInSeconds)
        {
            // Wait for the specified delay
            yield return new WaitForSeconds(delayInSeconds);
            
            // Destroy the game object
            Destroy(gameObjectToDestroy);
        }
        
        #endregion
    }
}
