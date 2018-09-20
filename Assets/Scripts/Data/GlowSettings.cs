using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SF {
    [CreateAssetMenu(menuName = "SF/Settings/Glow")]
    public class GlowSettings : ScriptableObject {
        public float minBrightness = 1f;
        public float maxBrightness = 4.5f;
        [Tooltip("Time to do a 1/2 cycle")]
        public float time = 1f;
    }
}
