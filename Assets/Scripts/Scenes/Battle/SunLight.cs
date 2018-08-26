using UnityEngine;

public class SunLight : MonoBehaviour {
    private Light light;

    public float dayIntensity = 0.85f;
    public float nightIntensity = 0.1f;

    private void Awake() {
        light = GetComponent<Light>();
    }

    public float GetIntensity() {
        return light.intensity;
    }
}
