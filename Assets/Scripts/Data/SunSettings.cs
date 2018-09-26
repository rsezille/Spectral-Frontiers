using UnityEngine;

namespace SF {
    [CreateAssetMenu(menuName = "SF/Settings/Sun")]
    public class SunSettings : ScriptableObject {
        [SerializeField, Tooltip("When the sun hurts really hard, in the desert during summer")]
        private float _hardCapIntensity = 1.1f;
        public float hardCapIntensity { private set { _hardCapIntensity = value; } get { return _hardCapIntensity; } }

        [SerializeField]
        private float _dayIntensity = 0.85f;
        public float dayIntensity { private set { _dayIntensity = value; } get { return _dayIntensity; } }

        [SerializeField]
        private float _nightIntensity = 0.1f;
        public float nightIntensity { private set { _nightIntensity = value; } get { return _nightIntensity; } }

        [HideInInspector]
        public bool turnType = false;

        public float turnTime = 0f;

        /**
         * Normalize the given intensity to 0-1 range, depending on dayIntensity and nightIntensity
         * Can be useful for Lerp values
         */
        public float GetNormalizedIntensity(float lightIntensity) {
            return Mathf.Clamp((lightIntensity - nightIntensity) / (dayIntensity - nightIntensity), nightIntensity, dayIntensity);
        }
    }
}
