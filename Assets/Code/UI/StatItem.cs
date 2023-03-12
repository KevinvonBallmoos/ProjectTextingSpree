using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Code.UI
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "StatItem", menuName = "Item/statItem")]
    public class StatItem : Item
    {
        public StatItemType _itemType;
        public int _amount;
        
        public override void Use()
        {
            base.Use();
            GameManager.instance.OnStatItemUse(_itemType, _amount);
            Inventory.inventoryInstance.RemoveItem(this);
        }
        
    }
        public enum StatItemType
        {
            HealthItem,
            ThirstItem,
            FoodItem
        }
}
