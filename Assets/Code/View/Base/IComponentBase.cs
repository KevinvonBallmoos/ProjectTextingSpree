using UnityEngine;

namespace Code.View.Base
{
    /// <summary>
    /// Interface for the base components, that each UI class needs.
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">23.12.2023</para>
    public interface IComponentBase
    {
        // MessageBox
        GameObject MessageBox { get; set; }
        // MessageBox Objects
        GameObject[] MessageBoxGameObjects { get; set; } 
        
        // Materials
        Material DefaultMaterial { get; set; }
    }
}