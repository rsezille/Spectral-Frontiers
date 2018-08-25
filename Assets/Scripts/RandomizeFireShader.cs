using UnityEngine;

/**
 * Used to not have the same frame of the shader at the same time on all fires
 */
[RequireComponent(typeof(SpriteRenderer))]
public class RandomizeFireShader : MonoBehaviour {
    private void Awake() {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();

        renderer.material.SetFloat("_Random", Random.value * 10);
    }
}
