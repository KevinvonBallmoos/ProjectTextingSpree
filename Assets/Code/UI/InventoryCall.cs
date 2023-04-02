using System;
using System.Collections.Generic;
using Code.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class InventoryCall : MonoBehaviour
    {
        [SerializeField] private Button inventoryButton;
        [SerializeField] private Button mapButton;
        [SerializeField] private Button menuButton;
        [SerializeField] private GameObject inventoryCanvas;
        [SerializeField] private GameObject mapCanvas;
        [SerializeField] private GameObject menuCanvas;

        private List<ItemSlot> _itemSlotList = new List<ItemSlot>();
        public GameObject _itemSlotPrefab;
        public Transform _inventoryItemTransform;
        
        private void Start()
        {
            Inventory.Inventory._instance._onItemChange += UpdateInventoryUI;
            UpdateInventoryUI();
        }

        // Gets Called once per frame
        private void Update()
        {
            inventoryButton.onClick.AddListener(CallInventory);
            mapButton.onClick.AddListener(CallMap);
            menuButton.onClick.AddListener(CallMenu);
        }

        /// <summary>
        /// Updates the inventory UI
        /// </summary>
        private void UpdateInventoryUI()
        {
            // First we check the count of the items in our inventory.
            int currentItemCount = Inventory.Inventory._instance._inventoryItemList.Count;
            
            // // Check if we have enough item slots.
            if (currentItemCount > _itemSlotList.Count)
            {
                // Add more item slots.
                AddItemSlots(currentItemCount);
            }
            // // Move through all item slots and check if item i is less or equal to the current item count.
            for (int i = 0; i < _itemSlotList.Count; i++)
            {
                if (i < currentItemCount)
                {
                    // Update the current item in the slot
                    _itemSlotList[i].AddItem(Inventory.Inventory._instance._inventoryItemList[i]);
                }
                else
                {
                    _itemSlotList[i].DestroySlot();
                    _itemSlotList.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Adds item slots to the inventory, so that we can add items. Makes sense?
        /// </summary>
        /// <param name="currentItemCount"></param>
        private void AddItemSlots(int currentItemCount)
        {
            // Calculate how many more slots we need. If we need any.
            int amount = currentItemCount - _itemSlotList.Count;

            // For loop to dynamically create slots in the inventory.
            for (int i = 0; i < amount; i++)
            {
                // _inventoryItemTransform is the Content of the InventoryPanel Scrollview.
                GameObject gameObject = Instantiate(_itemSlotPrefab, _inventoryItemTransform);
                ItemSlot newSlot = gameObject.GetComponent<ItemSlot>();
                _itemSlotList.Add(newSlot);
            }
        }

        /// <summary>
        /// Calls the inventory and disables the map.
        /// </summary>
        private void CallInventory()
        {
            inventoryCanvas.gameObject.SetActive(true);
            mapCanvas.gameObject.SetActive(false);
            menuCanvas.gameObject.SetActive(false);
        }

        /// <summary>
        /// Calls the map and disables the inventory.
        /// </summary>
        private void CallMap()
        {
            mapCanvas.gameObject.SetActive(true);
            inventoryCanvas.gameObject.SetActive(false);
            menuCanvas.gameObject.SetActive(false);
        }
    
        private void CallMenu()
        {
            menuCanvas.gameObject.SetActive(true);
            inventoryCanvas.gameObject.SetActive(false);
            mapCanvas.gameObject.SetActive(false);
        }
    
    }
}
