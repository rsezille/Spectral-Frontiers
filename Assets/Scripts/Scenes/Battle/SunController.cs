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

        public void NewTurn() {
            if (sunSettings.turnType) {
                sunSettings.turnTime += 0.2f;

                if (Mathf.Approximately(sunSettings.turnTime, 10f) || sunSettings.turnTime > 10f) {
                    sunSettings.turnTime = 0f;
                    lightIntensity.value = sunSettings.dayIntensity;
                }

                if (sunSettings.turnTime <= 5 && sunSettings.turnTime >= 4) {
                    lightIntensity.value = Mathf.Lerp(sunSettings.dayIntensity, sunSettings.nightIntensity, sunSettings.turnTime - 4f);
                } else if (sunSettings.turnTime <= 10 && sunSettings.turnTime >= 9) {
                    lightIntensity.value = Mathf.Lerp(sunSettings.nightIntensity, sunSettings.dayIntensity, sunSettings.turnTime - 9f);
                }
            }
        }
    }
}
