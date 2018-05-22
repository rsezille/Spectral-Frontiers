using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class GlowOutline : MonoBehaviour {
    public Color outlineColor;

    SpriteRenderer outline;

    void Awake() {
        outlineColor = new Color(1f, 1f, 0.6f, 1f);
        outline = GetComponent<SpriteRenderer>();

        outline.color = outlineColor;
    }

    // Use this for initialization
    void OnEnable() {
        StartCoroutine("GlowFade");
    }

    void OnDisable() {
        StopCoroutine("GlowFade");
    }

    IEnumerator GlowFade() {
        float initialFade = 0f;
        float maxFade = 0.4f;
        float smoothness = 0.02f;
        float duration = 1f;
        float progress = initialFade;
        float increment = smoothness / duration;

        while (isActiveAndEnabled) {
            outline.color = Color.Lerp(outlineColor, Color.clear, progress);

            if (progress > maxFade || progress < initialFade) {
                increment = -increment;
            }

            progress += increment;

            yield return new WaitForSeconds(smoothness);
        }
    }
}
