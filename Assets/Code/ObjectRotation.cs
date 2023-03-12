using UnityEngine;

namespace Code
{
    /// <summary>
    /// Rotates an Element
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">30.01.2023</para>
    public class ObjectRotation : MonoBehaviour
    {
        [SerializeField] private float zRotation;
        private void Update()
        {
            transform.Rotate(0, 0 * Time.deltaTime, zRotation, Space.Self);
        }
    }
}
