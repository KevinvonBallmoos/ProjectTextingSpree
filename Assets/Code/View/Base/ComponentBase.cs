using UnityEngine;

namespace Code.View.Base
{
    public class ComponentBase : MonoBehaviour
    {
        [Header("MESSAGE BOX")]
        // MessageBox
        [Header("MessageBox, Button Continue (left one), Message Box Text")]
        [SerializeField] protected GameObject messageBox;
        [SerializeField] protected GameObject[] messageBoxGameObjects;       
        
        [Header("MATERIALS")]
        // Materials
        [Header("Material")]
        [SerializeField] protected Material defaultMaterial;
    }
}