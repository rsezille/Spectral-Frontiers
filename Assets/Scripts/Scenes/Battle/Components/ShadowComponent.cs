using UnityEngine;

namespace SF {
    public class ShadowComponent : MonoBehaviour {
        public ShadowController shadowPrefab;

        private void Awake() {
            Instantiate(shadowPrefab, transform);
        }
    }
}
