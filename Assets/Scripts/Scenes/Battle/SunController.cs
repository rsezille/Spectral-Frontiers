using DG.Tweening;
using UnityEngine;

namespace SF {
    public class SunController : MonoBehaviour {
        private Light light;

        [Header("Dependencies")]
        public FloatVariable sunIntensity;
        public SunSettings sunSettings;
        public MissionVariable missionToLoad;

        [Header("Events")]
        public GameEvent lightChange;

        private void Awake() {
            light = GetComponent<Light>();
        }

        private void Update() {
            if (light.intensity != sunIntensity.value) {
                light.intensity = sunIntensity.value;
            }
        }

        public void NewTurn() {
            if (sunSettings.turnType) {
                sunSettings.turnTime += 0.2f;

                if (Mathf.Approximately(sunSettings.turnTime, 10f) || sunSettings.turnTime > 10f) {
                    sunSettings.turnTime = 0f;
                    sunIntensity.value = sunSettings.dayIntensity;
                }

                if (sunSettings.turnTime <= 5 && sunSettings.turnTime >= 4) {
                    sunIntensity.value = Mathf.Lerp(sunSettings.dayIntensity, sunSettings.nightIntensity, sunSettings.turnTime - 4f);
                } else if (sunSettings.turnTime <= 10 && sunSettings.turnTime >= 9) {
                    sunIntensity.value = Mathf.Lerp(sunSettings.nightIntensity, sunSettings.dayIntensity, sunSettings.turnTime - 9f);
                }
            }
        }

        public void LoadMission() {
            sunSettings.turnType = false;

            switch (missionToLoad.value.lighting) {
                case SunSettings.LightingType.Day:
                    sunIntensity.value = sunSettings.dayIntensity;
                    break;
                case SunSettings.LightingType.Night:
                    sunIntensity.value = sunSettings.nightIntensity;
                    break;
                case SunSettings.LightingType.Turn:
                    sunIntensity.value = sunSettings.dayIntensity;
                    sunSettings.turnType = true;
                    break;
                case SunSettings.LightingType.Auto: // Mainly for testing purpose
                    sunIntensity.value = sunSettings.nightIntensity;

                    float speed = 1f;
                    DOTween
                        .Sequence()
                        .Append(DOTween.To(() => sunIntensity.value, x => sunIntensity.value = x, sunSettings.dayIntensity, speed).SetEase(Ease.Linear).OnUpdate(lightChange.Raise))
                        .AppendInterval(speed * 4f)
                        .Append(DOTween.To(() => sunIntensity.value, x => sunIntensity.value = x, sunSettings.nightIntensity, speed).SetEase(Ease.Linear).OnUpdate(lightChange.Raise))
                        .AppendInterval(speed * 4f)
                        .SetLoops(-1);
                    break;
            }

            lightChange.Raise();
        }
    }
}
