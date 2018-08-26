using UnityEngine;
using DG.Tweening;

public class CharacterGlow : MonoBehaviour {
    private Material material;
    private float brightness = 1f;
    private Tween glowAnimation;

    public float minBrightness = 1f;
    public float maxBrightness = 4.5f;
    public float time = 1f;

    // Shader properties
    private const string SHADER_OUTLINE_ENABLED = "_IsOutlineEnabled";
    private const string SHADER_OUTLINE_COLOR = "_OutlineColor";

    private void Awake() {
        material = GetComponent<SpriteRenderer>().material;

        ChangeBrightness(minBrightness);

        glowAnimation = DOTween.To(() => brightness, x => { brightness = x; ChangeBrightness(x); }, maxBrightness, time).SetLoops(-1, LoopType.Yoyo);
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
