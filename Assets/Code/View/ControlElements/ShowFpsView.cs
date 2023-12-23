using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace Code.View.ControlElements
{
    /// <summary>
    /// Displays the fps on a text object.
    /// </summary>
    public class ShowFpsView : MonoBehaviour {
        
        // Fps text
        [SerializeField] private Text fpsText;
        // DeltaTime
        private float deltaTime = 1;

        /// <summary>
        /// Updates the frames
        /// </summary>
        private void Update () {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            var fps = 1.0f / deltaTime;
            fpsText.text = Mathf.Ceil (fps).ToString (CultureInfo.InvariantCulture);
        }
    }
}
