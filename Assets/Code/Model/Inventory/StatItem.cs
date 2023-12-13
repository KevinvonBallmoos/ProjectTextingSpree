using Code.Controller.GameController;
using UnityEngine;

namespace Code.Model.Inventory
{
    /// <summary>
    /// This class holds the Stat Items
    /// </summary>
    /// <para name="author">Kastriot Dulla</para>
    /// <para name="date">11.01.2023</para>
    [System.Serializable]
    [CreateAssetMenu(fileName = "StatItem", menuName = "Item/statItem")]
    public class StatItem : Item
    {
        public StatItemType itemType;
        public int amount;
        
        /// <summary>
        /// Use the actual item
        /// </summary>
        public override void Use()
        {
            base.Use();
            InventoryController.Ic.OnStatItemUse(itemType, amount);
            Model.Inventory.Inventory.Instance.RemoveItem(this);
        }
    }
    
    /// <summary>
    /// Enumeration of several item types, that can be created.
    /// </summary>
    public enum StatItemType
    {
        HealthItem,
        ThirstItem,
        FoodItem
    }
}
