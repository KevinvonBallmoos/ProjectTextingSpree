using UnityEngine;

namespace Code.View.Base
{
    public class ComponentBase : MonoBehaviour
    {
        // Settings prefabs
        [Header("Settings Properties Root, Bool Prefab, Input Prefab")] 
        [SerializeField] protected GameObject settingsPropertiesRoot;
        [SerializeField] protected GameObject[] settingsPrefabs;
        
        [Header("MESSAGE BOX")]
        // MessageBox
        [Header("MessageBox, Button Continue (left one), Message Box Text")]
        [SerializeField] public GameObject messageBox;
        [SerializeField] public GameObject[] messageBoxGameObjects;       
        
        [Header("MATERIALS")]
        // Materials
        [Header("Material")]
        [SerializeField] public Material defaultMaterial;
    }
}