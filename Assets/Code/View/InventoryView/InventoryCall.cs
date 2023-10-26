using System.Collections.Generic;
using Code.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace Code.View.InventoryView
{
    public class InventoryCall : MonoBehaviour
    {
        [SerializeField] private Button inventoryButton;
        [SerializeField] private Button mapButton;
        [SerializeField] private Button menuButton;
        [SerializeField] private GameObject inventoryCanvas;
        [SerializeField] private GameObject mapCanvas;
        [SerializeField] private GameObject menuCanvas;

        private readonly List<ItemSlot> _itemSlotList = new ();
        public GameObject itemSlotPrefab;
        public Transform inventoryItemTransform;

        private bool _isInventoryOpen;
        private bool _isMapOpen;
        private bool _isMenuOpen;
        
        private void Start()
        {
            Inventory.Inventory.Instance.OnItemChangeDelegate += UpdateInventoryUI;
            UpdateInventoryUI();
            
            inventoryButton.onClick.AddListener(Inventory_Click);
            mapButton.onClick.AddListener(Map_Click);
            menuButton.onClick.AddListener(Menu_Click);
        }

        /// <summary>
        /// Updates the inventory UI
        /// </summary>
        private void UpdateInventoryUI()
        {
            // First we check the count of the items in our inventory.
            int currentItemCount = Inventory.Inventory.Instance.inventoryItemList.Count;
            
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
                    _itemSlotList[i].AddItem(Inventory.Inventory.Instance.inventoryItemList[i]);
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
                var gameObject = Instantiate(itemSlotPrefab, inventoryItemTransform);
                var newSlot = gameObject.GetComponent<ItemSlot>();
                _itemSlotList.Add(newSlot);
            }
        }

        /// <summary>
        /// Opens the inventory and disables the map.
        /// </summary>
        private void Inventory_Click()
        {
            _isInventoryOpen = true;
            _isMapOpen = false;
            
            SetScreens();
        }

        /// <summary>
        /// Opens the map and disables the inventory.
        /// </summary>
        private void Map_Click()
        {
            _isMapOpen = true;
            _isInventoryOpen = false;
            
           SetScreens();
        }
    
        /// <summary>
        /// Opens the menu above the map and the inventory
        /// </summary>
        private void Menu_Click()
        {
            if (_isMenuOpen)
            {
                _isMenuOpen = false;
                menuCanvas.gameObject.SetActive(false);
            }
            else
            {
                _isMenuOpen = true;
                menuCanvas.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Sets the screen, according to the map and inventory 
        /// </summary>
        private void SetScreens()
        {
            if (_isInventoryOpen)
            {
                inventoryCanvas.gameObject.SetActive(true);
                mapCanvas.gameObject.SetActive(false);
            }
            if (_isMapOpen)
            {
                mapCanvas.gameObject.SetActive(true);
                inventoryCanvas.gameObject.SetActive(false);
            } 

            menuCanvas.gameObject.SetActive(false);
        }
    }
}
