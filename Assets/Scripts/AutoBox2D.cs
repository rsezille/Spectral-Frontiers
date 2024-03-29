﻿using UnityEngine;

/**
 * Add a collider2D on the attached GameObject.
 * Useful on UI elements to get hit by raycasts
 */
[RequireComponent(typeof(RectTransform))]
public class AutoBox2D : MonoBehaviour {
    private BoxCollider2D box;
    private RectTransform rect;

    private void Awake() {
        if (rect == null) {
            rect = gameObject.GetComponent<RectTransform>();
        }

        box = gameObject.AddComponent<BoxCollider2D>();

        box.size = new Vector2(rect.rect.width, rect.rect.height);
        box.offset = Vector2.zero;
    }

    private void OnRectTransformDimensionsChange() {
        if (box != null) {
            box.size = new Vector2(rect.rect.width, rect.rect.height);
        }
    }
}
