using UnityEngine;

namespace Code.SaveManager
{
    public class SaveImageRotation : MonoBehaviour
    {
        private void Update()
        {
            transform.Rotate(0, 0 * Time.deltaTime, -0.5f, Space.Self);
        }
    }
}
