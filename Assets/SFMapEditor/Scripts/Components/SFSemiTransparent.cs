using UnityEngine;

/**
 * Change the attached game object layer with "SemiTransparent"
 */
[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class SFSemiTransparent : MonoBehaviour {
    private SpriteRenderer spriteRenderer;

    // Because several objects can trigger the opacity change, we need to store the count of those objects
    public int transparentObjectsCount = 0;

    private void Start() {
        gameObject.layer = LayerMask.NameToLayer("SemiTransparent");
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /**
     * Triggered by Board and also by other objects (board character...)
     */
    public void MouseEnter() {
        if (transparentObjectsCount == 0) {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.5f);
        }

        transparentObjectsCount++;
    }

    /**
     * Triggered by Board and also by other objects (board character...)
     */
    public void MouseLeave() {
        if (transparentObjectsCount == 1) {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
        }

        transparentObjectsCount--;
    }
}
