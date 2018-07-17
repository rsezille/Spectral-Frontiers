using UnityEngine;
using SpriteGlow;
using DG.Tweening;

/**
 * TODO [FINAL] May have a performance improvement: pause the DOTween animation when the glow is disabled
 */
[RequireComponent(typeof(SpriteGlowEffect))]
public class CharacterGlowAnimation : MonoBehaviour {
    public float minBrightness = 1f;
    public float maxBrightness = 4.5f;
    public float time = 1f;

    private void Start() {
        SpriteGlowEffect sge = GetComponent<SpriteGlowEffect>();

        sge.GlowBrightness = minBrightness;

        DOTween.To(() => sge.GlowBrightness, x => sge.GlowBrightness = x, maxBrightness, time).SetLoops(-1, LoopType.Yoyo);
    }
}
