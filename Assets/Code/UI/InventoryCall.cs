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
    
        // Start is called before the first frame update
        private void Start()
        {
            inventoryButton.onClick.AddListener(CallInventory);
            mapButton.onClick.AddListener(CallMap);
            menuButton.onClick.AddListener(CallMenu);
        }

        /// <summary>
        /// Calls the inventory and disables the map.
        /// THis is just for pushing.
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
