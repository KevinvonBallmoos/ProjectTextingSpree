using UnityEngine;

namespace Code.GameDataManager
{
    /// <summary>
    /// Rotates an Element
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">30.01.2023</para>
    public class SaveImageRotation : MonoBehaviour
    {
        private void Update()
        {
            transform.Rotate(0, 0 * Time.deltaTime, -0.5f, Space.Self);
        }
    }
}
