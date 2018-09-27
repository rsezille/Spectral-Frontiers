using UnityEngine;
using DG.Tweening;

namespace SF {
    public class Glow : MonoBehaviour {
        private Material material;
        private float brightness = 1f;
        private Tween glowAnimation;

        [Header("Dependencies")]
        public GlowSettings glowSettings;

        // Shader properties
        private const string SHADER_OUTLINE_ENABLED = "_IsOutlineEnabled";
        private const string SHADER_OUTLINE_COLOR = "_OutlineColor";

        private void Awake() {
            material = GetComponent<SpriteRenderer>().material;

            ChangeBrightness(glowSettings.minBrightness);

            glowAnimation = DOTween.To(() => brightness, x => { brightness = x; ChangeBrightness(x); }, glowSettings.maxBrightness, glowSettings.time).SetLoops(-1, LoopType.Yoyo);
        }

        public void ChangeBrightness(float newBrightness) {
            material.SetColor(SHADER_OUTLINE_COLOR, Color.white * newBrightness);
        }

        public void Enable() {
            material.SetFloat(SHADER_OUTLINE_ENABLED, 1f);

            if (glowAnimation != null) {
                glowAnimation.Play();
            }
        }

        public void Disable() {
            material.SetFloat(SHADER_OUTLINE_ENABLED, 0f);

            if (glowAnimation != null) {
                glowAnimation.Pause();
            }
        }
    }
}
