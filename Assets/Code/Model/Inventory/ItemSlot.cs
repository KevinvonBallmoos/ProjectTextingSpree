using Code.Controller.GameController;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Model.Inventory
{
    public class ItemSlot : MonoBehaviour
    {
        // Image icon
        public Image icon;
        // Item
        private Item _item;

        /// <summary>
        /// Adds new item to the itemSlot
        /// </summary>
        /// <param name="newItem"></param>
        public void AddItem(Item newItem)
        {
            _item = newItem;
            icon.sprite = newItem._icon;
        }

        /// <summary>
        /// Use Item when it's clicked on.
        /// </summary>
        public void UseItem()
        {
            if (_item != null)
                _item.Use();
        }

        /// <summary>
        /// Destroy item slot, so that other items can be created.
        /// </summary>
        public void DestroySlot()
        {
            Destroy(gameObject);
        }

        /// <summary>
        /// Remove an item from the inventory.
        /// </summary>
        public void OnRemoveButtonClicked()
        {
            if (_item != null)
                Model.Inventory.Inventory.Instance.RemoveItem(_item);
        }

        /// <summary>
        /// Call item description when moused over an item.
        /// </summary>
        public void OnCourserEnter()
        {
            // Call the data from the GameManager script
            InventoryController.Ic.DisplayItemInfo(_item.name, _item.GetItemDescription(), transform.position);
        }
        
        /// <summary>
        /// Close item description when mouse leave the item.
        /// </summary>
        public void OnCourserExit()
        {
            InventoryController.Ic.DestroyItemInfo();
        }
    }
}
