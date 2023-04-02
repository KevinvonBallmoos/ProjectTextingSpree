using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Inventory
{
    public class ItemSlot : MonoBehaviour
    {
        public Image _icon;
        private Item _item;

        /// <summary>
        /// Adds new item to the itemSlot
        /// </summary>
        /// <param name="newItem"></param>
        public void AddItem(Item newItem)
        {
            _item = newItem;
            _icon.sprite = newItem._icon;
        }

        /// <summary>
        /// Use Item when it's clicked on.
        /// </summary>
        public void UseItem()
        {
            if (_item != null)
            {
                _item.Use();
            }
        }

        /// <summary>
        /// Distroy item slot, so that other items can be created.
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
            {
                Inventory._instance.RemoveItem(_item);
            }
        }

        /// <summary>
        /// Call item description when moused over an item.
        /// </summary>
        public void OnCourserEnter()
        {
            // Call the data from the GameManager script
            GameManager.instance.DisplayItemInfo(_item.name, _item.GetItemDescription(), transform.position);
        }
        
        /// <summary>
        /// Close item description when mouse leave the item.
        /// </summary>
        public void OnCourserExit()
        {
            GameManager.instance.DestroyItemInfo();
        }
    }
}
