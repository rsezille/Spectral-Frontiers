using UnityEngine;

/**
 * Attach this to any sprite renderer and it will duplicate it and toggle the flip Y to prepare for the water reflection shader.
 * Set layer as Reflectable and translate the y position given the pivot and the bottom of the sprite to have a "perfect" reflection.
 */
 [RequireComponent(typeof(SpriteRenderer))]
public class Reflectable : MonoBehaviour {
    private SpriteRenderer spriteRenderer;

    private GameObject reflectionGameObject;
    private SpriteRenderer reflectionSpriteRenderer;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();

        reflectionGameObject = new GameObject("WaterReflect", typeof(SpriteRenderer));
        reflectionGameObject.transform.parent = transform;
        reflectionGameObject.transform.localPosition = Vector3.zero;
        reflectionGameObject.layer = LayerMask.NameToLayer("Reflectable");

        reflectionSpriteRenderer = reflectionGameObject.GetComponent<SpriteRenderer>();
        reflectionSpriteRenderer.flipY = !spriteRenderer.flipY;
        reflectionSpriteRenderer.flipX = spriteRenderer.flipX;
    }

    private void LateUpdate() {
        reflectionSpriteRenderer.flipX = spriteRenderer.flipX;
        reflectionSpriteRenderer.sprite = spriteRenderer.sprite;
    }
}
