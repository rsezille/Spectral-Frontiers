using DG.Tweening;
using UnityEngine;

/**
 * The intensity of board lights (campfire, torch, etc.) depends on the sun light intensity: no light when day, full light when night
 */
[RequireComponent(typeof(Light))]
public class BoardLight : MonoBehaviour {
    private BattleManager battleManager;
    private Light light;
    private float initialIntensity;
    private Tween glowAnimation;

    public float nonVisibleIntensity = 0.8f;
    public float fullyVisibleIntensity = 0.1f;

    private void Awake() {
        battleManager = BattleManager.instance;

        light = GetComponent<Light>();
        initialIntensity = light.intensity;

        battleManager.OnLightChange += CheckIntensity;
    }

    private void Start() {
        CheckIntensity();

        //DOTween.To(() => light.range, x => light.range = x, 7f, 1f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
    }
    
    public void CheckIntensity() {
        float sunIntensity = Mathf.Clamp(battleManager.sunLight.GetIntensity(), fullyVisibleIntensity, nonVisibleIntensity);

        glowAnimation.Kill();
        glowAnimation = null;

        light.intensity = Mathf.Lerp(initialIntensity, 0f, (sunIntensity - fullyVisibleIntensity) / (nonVisibleIntensity - fullyVisibleIntensity));

        if (light.intensity > 0) {
            light.intensity -= 0.5f;
            glowAnimation = light.DOIntensity(light.intensity + 0.5f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
        }
    }

    private void OnDestroy() {
        battleManager.OnLightChange -= CheckIntensity;
    }
}
