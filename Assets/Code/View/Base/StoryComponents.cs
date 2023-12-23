using UnityEngine;

namespace Code.View.Base
{
    /// <summary>
    /// Abstract class provides fields for the Story
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">23.12.2023</para>
    public abstract class StoryComponents : MonoBehaviour, IComponentBase
    {
        [Header("NewStory Scene / StoryScene 1 + 2")]
        [SerializeField] protected GameObject[] menuGroupObjects;
        
        #region Inherited from IComponentBase
        
        [field: Header("MESSAGE BOX")]
        // MessageBox
        [field: Header("MessageBox, Button Continue (left one), Message Box Text")]
        public GameObject MessageBox { get; set; }
        public GameObject[] MessageBoxGameObjects { get; set; }
        [field: Header("MATERIALS")]
        // Materials
        [field: Header("Material")]
        public Material DefaultMaterial { get; set; }
        
        #endregion
    }
}