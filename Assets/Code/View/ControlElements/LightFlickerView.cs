using UnityEngine;

namespace Code.View.ControlElements
{

    public class LightFlickerView : MonoBehaviour
    {
        
        [SerializeField] private UnityEngine.Rendering.Universal.Light2D yellowLight;

        private int frames;

        [SerializeField] private int framesPerRandomize;

        [SerializeField] private float minValue;
        [SerializeField] private float maxValue;

        // Update is called once per frame
        private void Update()
        {
            frames++;
            if (frames % framesPerRandomize == 0)
            { 
                RandomizeIntensity();
            }
        }

        private void RandomizeIntensity()
        {
            // Create an instance of the Random class
            var random = new System.Random();
            var randomValue = (float)(random.NextDouble() * (maxValue - minValue) + minValue);
            yellowLight.intensity = randomValue;
        }
    }
}
