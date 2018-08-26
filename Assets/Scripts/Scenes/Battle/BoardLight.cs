using UnityEngine;

/**
 * The intensity of board lights (campfire, torch, etc.) depends on the sun light intensity: no light when day, full light when night
 */
[RequireComponent(typeof(Light))]
public class BoardLight : MonoBehaviour {
    private Light light;
    private float initialIntensity;

    public float nonVisibleIntensity = 0.8f;
    public float fullyVisibleIntensity = 0.1f;

    private void Start() {
        light = GetComponent<Light>();
        initialIntensity = light.intensity;

        BattleManager.instance.OnLightChange += CheckIntensity;

        CheckIntensity();
    }

    #if UNITY_EDITOR
    private void Update() {
        CheckIntensity();
    }
    #endif

    public void CheckIntensity() {
        float sunIntensity = Mathf.Clamp(BattleManager.instance.sunLight.GetIntensity(), fullyVisibleIntensity, nonVisibleIntensity);

        light.intensity = Mathf.Lerp(5.76f, 0f, sunIntensity / nonVisibleIntensity);
    }
}
