using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Inventory
{
    public class InventoryController : MonoBehaviour
    {
        // GameManager
        private static InventoryController inventoryController;
        // Inventory items
        public List<Item> _itemList = new List<Item>();
        public Transform _inventoryPanel;
        public GameObject _itemInfoPrefab;
        private GameObject _currentItemInfo = null;
        
        // Singleton region to destroy items as well as item description
        #region singleton

        // Create InventoryController instance to be able to use Awake as well as all other methods in static content.
        public static InventoryController instance;

        private void Awake()
        {
            if (inventoryController == null)
                inventoryController = this;
            else
                Destroy(gameObject);

            if (instance == null)
                instance = this;
        }
        
        #endregion

        /// <summary>
        /// Adds the Item,
        /// TODO Put into InventoryController
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(string item)
        {
            foreach (var i in _itemList)
            {
                if (i._name.Equals(item))
                {
                    Inventory._instance.AddItem(i);
                }
            }
        }
        
        /// <summary>
        /// This function is called when we use an item that can grant stats, such as health.
        /// </summary>
        /// <param name="itemType"></param>
        /// <param name="amount"></param>
        public void OnStatItemUse(StatItemType itemType, int amount)
        {
            Debug.Log("Consuming: "+ itemType + " Added amount: "+amount);
        }
        
        /// <summary>
        /// Displays item information when the mouse is hovered over an item.
        /// </summary>
        /// <param name="itemName"></param>
        /// <param name="itemDescription"></param>
        /// <param name="buttonPos"></param>
        public void DisplayItemInfo(string itemName, string itemDescription, Vector2 buttonPos)
        {
            if (_currentItemInfo != null)
            {
                Destroy(_currentItemInfo.gameObject);
            }
        
            // Create variables so that the tooltip window is moved to a different direction and not simply closed
            buttonPos.x -= 100;  
            buttonPos.y += 75;
            
            _currentItemInfo = Instantiate(_itemInfoPrefab, buttonPos, Quaternion.identity, _inventoryPanel);
            _currentItemInfo.GetComponent<ItemInfo>().SetUp(itemName, itemDescription);
        }
        
        /// <summary>
        /// Destroy Item info so it's not staying behind when the mouse is leaving.
        /// </summary>
        public void DestroyItemInfo()
        {
            if (_currentItemInfo != null)
            {
                float delayInSeconds = 0.5f; // Set the delay in seconds
        
                // Delay the destruction of the _currentItemInfo game object
                StartCoroutine(DelayedDestroy(_currentItemInfo.gameObject, delayInSeconds));
        
                _currentItemInfo = null; // Clear the _currentItemInfo variable
            }
        }

        IEnumerator DelayedDestroy(GameObject gameObjectToDestroy, float delayInSeconds)
        {
            // Wait for the specified delay
            yield return new WaitForSeconds(delayInSeconds);
            
            // Destroy the game object
            Destroy(gameObjectToDestroy);
        }

    }

}
