using UnityEngine;

/**
 * TODO [BETA]  In this script, we detect screen size change when in the editor, to not distort the RenderTexture we assign to the reflection camera.
 *              Obviously it is done in the update method, that is why it is editor only.
 *              Redo this using a global method, to re-generate the render texture (GenerateRenderTexture) only at launch and when the player changes the settings.
 */
[RequireComponent(typeof(Camera))]
public class WaterReflectionCamera : MonoBehaviour {
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

    private void Start() {
        ComputeOrthographicSize();
    }

    public void ComputeOrthographicSize() {
        reflectionCamera.orthographicSize = GetComponentInParent<BattleCamera>().GetComponent<Camera>().orthographicSize;
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

    public void VisibilityChange(float value) {
        refractionVisibility = value;

        Shader.SetGlobalFloat(globalVisibilityName, refractionVisibility);
    }

    public void MagnitudeChange(float value) {
        refractionMagnitude = value;

        Shader.SetGlobalFloat(globalMagnitudeName, refractionMagnitude);
    }

    public void GenerateRenderTexture() {
        // Avoid memory leak
        if (reflectionCamera.targetTexture != null) {
            RenderTexture temp = reflectionCamera.targetTexture;

            reflectionCamera.targetTexture = null;
            DestroyImmediate(temp);
        }
        
        reflectionCamera.targetTexture = new RenderTexture(Screen.width, Screen.height, 16);
        reflectionCamera.targetTexture.filterMode = FilterMode.Bilinear;

        Shader.SetGlobalTexture(globalTextureName, reflectionCamera.targetTexture);
    }
}
