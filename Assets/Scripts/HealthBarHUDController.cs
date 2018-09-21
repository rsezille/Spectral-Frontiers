using UnityEngine;

namespace SF {
    public class HealthBarHUDController : MonoBehaviour {
        public ChangeColorByScale hpScale;

        [Header("Dependencies")]
        public Character character;

        private void Awake() {
            if (!hpScale) {
                hpScale = GetComponentInChildren<ChangeColorByScale>();
            }
        }

        private void Start() {
            UpdateHPScale();
        }

        private void Update() {
            UpdateHPScale();
        }

        private void UpdateHPScale() {
            hpScale.transform.localScale = new Vector3(
                (float)character.currentHp / (float)character.hp.currentValue,
                hpScale.transform.localScale.y,
                hpScale.transform.localScale.z
            );
        }
    }
}
