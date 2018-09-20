using DG.Tweening;
using SF;
using UnityEngine;

/**
 * The intensity of board lights (campfire, torch, etc.) depends on the sun light intensity: no light during the day, full light during the night
 */
[RequireComponent(typeof(Light))]
public class BoardLight : MonoBehaviour {
    private Light light;
    private float initialIntensity;
    private Tween glowAnimation;

    [Header("Dependencies")]
    public FloatVariable sunIntensity;

    public float nonVisibleIntensity = 0.8f;
    public float fullyVisibleIntensity = 0.1f;

    private void Awake() {
        light = GetComponent<Light>();
        initialIntensity = light.intensity;
    }

    private void Start() {
        CheckIntensity();

        //DOTween.To(() => light.range, x => light.range = x, 7f, 1f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
    }
    
    public void CheckIntensity() {
        float sunNormalizedIntensity = Mathf.Clamp(sunIntensity.value, fullyVisibleIntensity, nonVisibleIntensity);

        glowAnimation.Kill();
        glowAnimation = null;

        light.intensity = Mathf.Lerp(initialIntensity, 0f, (sunNormalizedIntensity - fullyVisibleIntensity) / (nonVisibleIntensity - fullyVisibleIntensity));

        if (light.intensity > 0) {
            light.intensity -= 0.5f;
            glowAnimation = light.DOIntensity(light.intensity + 0.5f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
        }
    }
}
