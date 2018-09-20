using UnityEngine;

namespace SF {
    public class SunController : MonoBehaviour {
        private Light light;

        [Header("Dependencies")]
        public FloatVariable lightIntensity;
        public SunSettings sunSettings;

        private void Awake() {
            light = GetComponent<Light>();
        }

        private void Update() {
            if (light.intensity != lightIntensity.value) {
                light.intensity = lightIntensity.value;
            }
        }
    }
}
