using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
   public class ItemSlot : MonoBehaviour
   {
      public Image _icon;
      private Item _item;

      /// <summary>
      /// Asignes the new item that is given as a parameter ot _item.
      /// As well as get the icon of said item.
      /// </summary>
      /// <param name="newItem"></param>
      public void AddItem(Item newItem)
      {
         _item = newItem;
         _icon.sprite = newItem._itemIcon;
      }

      /// <summary>
      /// Checks if item is not null and calls the Use() function defined in the item class.
      /// </summary>
      public void UseItem()
      {
         if (_item != null)
         {
            _item.Use();
         }
      }

      /// <summary>
      /// Obvious. Deletes an item slot and the item attachet to it.
      /// </summary>
      public void DestroySlot()
      {
         Destroy(gameObject);
      }

      public void OnRemoveButtonClick()
      {
         if (_item != null)
         {
            Inventory.inventoryInstance.RemoveItem(_item);
         }
      }

      public void OnCursorEnter()
      {
         GameManager.instance.DisplayItemInfo(_item.name, _item.GetItemDescription(), transform.position);
      }
      
      public void OnCursorExit()
      {
         GameManager.instance.DestroyItemInfo();
      }
   }
}

