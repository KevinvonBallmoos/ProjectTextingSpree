using UnityEngine;

namespace Code.Controller
{
    /// <summary>
    /// Rotates an Element around the Z Axis
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">30.01.2023</para>
    public class ObjectRotation : MonoBehaviour
    {
        // Z Rotation
        [SerializeField] private float zRotation;
        
        /// <summary>
        /// Every frame the GameObject gets rotated
        /// </summary>
        private void Update()
        {
            transform.Rotate(0, 0 * Time.deltaTime, zRotation, Space.Self);
        }
    }
}
