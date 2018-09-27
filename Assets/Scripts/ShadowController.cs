using DG.Tweening;
using UnityEngine;

namespace SF {
    public class ShadowController : MonoBehaviour {
        private SpriteRenderer sprite;
        private Tween showAnimation;

        [Header("Dependencies")]
        public SunSettings sunSettings;
        public FloatVariable sunIntensity;

        [Header("Variables")]
        public float nightOpacity = 0.4f;
        public float dayOpacity = 0.7f;

        private void Awake() {
            sprite = GetComponent<SpriteRenderer>();
        }

        private void Start() {
            // Don't show a scaled shadow if the parent is self scaled
            transform.localScale = new Vector3(1f / transform.lossyScale.x, 1f / transform.lossyScale.y, 1f / transform.lossyScale.z);

            CheckOpacity();
        }

#if UNITY_EDITOR
        private void Update() {
            CheckOpacity();
        }
#endif

        public void CheckOpacity() {
            if (showAnimation == null || (!showAnimation.IsActive() || showAnimation.IsComplete())) {
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, Mathf.Lerp(nightOpacity, dayOpacity, sunSettings.GetNormalizedIntensity(sunIntensity.value)));
            }
        }

        /**
         * As it starts by hidding the shadow, it should be called on parent creation
         * Used by the cutscene to gradually show the shadow together with the sprite 
         */
        public void ShowCutscene(float speed = 1f) {
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0f);

            showAnimation = sprite.DOColor(new Color(sprite.color.r, sprite.color.g, sprite.color.b, Mathf.Lerp(nightOpacity, dayOpacity, sunSettings.GetNormalizedIntensity(sunIntensity.value))), speed);
        }
    }
}
