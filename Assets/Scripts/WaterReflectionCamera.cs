﻿using UnityEngine;

/**
 * TODO[BETA]   In this script, we detect screen size change when in the editor, to not distort the RenderTexture we assign to the reflection camera.
 *              Obviously it is done in the update method, that is why it is editor only.
 *              Redo this using a global method, to re-generate the render texture (GenerateRenderTexture) only at launch and when the player changes the settings.
 */
[RequireComponent(typeof(Camera))]
public class WaterReflectionCamera : MonoBehaviour {
    private Camera reflectionCamera;
    private Camera mainCamera;

    [SerializeField, Range(0, 1)]
    private float refractionVisibility = 0;
    [SerializeField, Range(0, 0.1f)]
    private float refractionMagnitude = 0;

    private static readonly string globalTextureName = "_GlobalRefractionTex";
    private static readonly string globalVisibilityName = "_GlobalVisibility";
    private static readonly string globalMagnitudeName = "_GlobalRefractionMag";

#if UNITY_EDITOR
    private Vector2Int screenResolution;
#endif

    private void Awake() {
        reflectionCamera = GetComponent<Camera>();
        mainCamera = transform.parent.GetComponent<Camera>();

#if UNITY_EDITOR
        screenResolution = new Vector2Int(Screen.width, Screen.height);
#endif
    }

    private void Update() {
#if UNITY_EDITOR
        if (screenResolution.x != Screen.width || screenResolution.y != Screen.height) {
            GenerateRenderTexture();
            screenResolution = new Vector2Int(Screen.width, Screen.height);
        }
#endif
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

    private void GenerateRenderTexture() {
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