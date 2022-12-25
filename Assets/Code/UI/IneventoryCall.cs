using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IneventoryCall : MonoBehaviour
{
    [SerializeField] private Button inventoryButton;
    [SerializeField] private Button mapButton;
    [SerializeField] private GameObject inventoryCanvas;
    [SerializeField] private GameObject mapCanvas;
    
    // Start is called before the first frame update
    void Start()
    {
        inventoryButton.onClick.AddListener(CallInventory);
        mapButton.onClick.AddListener(CallMap);
    }

    /// <summary>
    /// Calls the inventory and disables the map.
    /// THis is just for pushing.
    /// </summary>
    private void CallInventory()
    {
        inventoryCanvas.gameObject.SetActive(true);
        mapCanvas.gameObject.SetActive(false);
    }

    /// <summary>
    /// Calls the map and disables the inventory.
    /// </summary>
    private void CallMap()
    {
        mapCanvas.gameObject.SetActive(true);
        inventoryCanvas.gameObject.SetActive(false);
    }
    
}
