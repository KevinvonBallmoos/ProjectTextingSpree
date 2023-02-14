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

      public void AddItem(Item newItem)
      {
         _item = newItem;
         _icon.sprite = newItem._itemIcon;
      }

      public void UseItem()
      {
         if (_item != null)
         {
            _item.Use();
         }
      }

      public void DestroySlot()
      {
         Destroy(gameObject);
      }
   }
}

