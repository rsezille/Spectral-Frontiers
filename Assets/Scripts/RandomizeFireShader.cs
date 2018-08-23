using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class RandomizeFireShader : MonoBehaviour {
    private void Awake() {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();

        renderer.material.SetFloat("_Random", Random.value * 10);
    }
}
