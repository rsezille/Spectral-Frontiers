using UnityEngine;

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
    }

    public void CheckIntensity() {
        float sunIntensity = Mathf.Clamp(BattleManager.instance.sunLight.intensity, fullyVisibleIntensity, nonVisibleIntensity);

        light.intensity = Mathf.Lerp(5.76f, 0f, sunIntensity / nonVisibleIntensity);
    }
}
