using System.Collections;
using System.Collections.Generic;
using Code;
using UnityEngine;

namespace Code.Inventory
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "StatItem", menuName = "Item/statItem")]
    public class StatItem : Item
    {
        public StatItemType _itemType;
        public int _amount;
        
        /// <summary>
        /// Use the actual item. Similar to the baseItem.
        /// </summary>
        public override void Use()
        {
            base.Use();
            InventoryController.instance.OnStatItemUse(_itemType, _amount);
            Inventory._instance.RemoveItem(this);
        }
    }
    
    /// <summary>
    /// The item types, that can be created.
    /// </summary>
    public enum StatItemType
    {
        HealtItem,
        ThirstItem,
        FoodItem,
    }
}
