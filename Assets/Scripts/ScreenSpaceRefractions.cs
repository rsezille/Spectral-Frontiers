using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ScreenSpaceRefractions : MonoBehaviour {
    private Camera reflectionCamera;

    [SerializeField, Range(0, 1)]
    private float refractionVisibility = 0;
    [SerializeField, Range(0, 0.1f)]
    private float refractionMagnitude = 0;

    private static readonly string globalTextureName = "_GlobalRefractionTex";
    private static readonly string globalVisibilityName = "_GlobalVisibility";
    private static readonly string globalMagnitudeName = "_GlobalRefractionMag";

    private void Awake() {
        reflectionCamera = GetComponent<Camera>();
    }

    public void VisibilityChange(float value) {
        refractionVisibility = value;

        Shader.SetGlobalFloat(globalVisibilityName, refractionVisibility);
    }

    public void MagnitudeChange(float value) {
        refractionMagnitude = value;

        Shader.SetGlobalFloat(globalMagnitudeName, refractionMagnitude);
    }

    private void OnEnable() {
        GenerateRenderTexture();

        Shader.SetGlobalFloat(globalVisibilityName, refractionVisibility);
        Shader.SetGlobalFloat(globalMagnitudeName, refractionMagnitude);
    }

    private void OnValidate() {
        Shader.SetGlobalFloat(globalVisibilityName, refractionVisibility);
        Shader.SetGlobalFloat(globalMagnitudeName, refractionMagnitude);
    }

    private void GenerateRenderTexture() {
        // Avoid memory leak
        if (reflectionCamera.targetTexture != null) {
            RenderTexture temp = reflectionCamera.targetTexture;

            reflectionCamera.targetTexture = null;
            DestroyImmediate(temp);
        }

        reflectionCamera.targetTexture = new RenderTexture(reflectionCamera.pixelWidth, reflectionCamera.pixelHeight, 16);
        reflectionCamera.targetTexture.filterMode = FilterMode.Bilinear;

        Shader.SetGlobalTexture(globalTextureName, reflectionCamera.targetTexture);
    }
}
