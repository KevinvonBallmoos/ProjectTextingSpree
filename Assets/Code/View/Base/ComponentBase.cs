using UnityEngine;

namespace Code.View.Base
{
    public class ComponentBase : MonoBehaviour
    {
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