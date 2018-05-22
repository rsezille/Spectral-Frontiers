using UnityEngine;
using UnityEngine.UI;

/**
 * Scale a color to another one given min & max values
 */
public class ChangeColorByScale : MonoBehaviour {
    // Target
    public Image image;

    // Parameters
    public float minValue = 0.0f;
    public float maxValue = 1.0f;
    public Color minColor = Color.red;
    public Color maxColor = Color.green;

    void Awake() {
        if (image == null) {
            image = GetComponent<Image>();
        }
    }

    void Update() {
        image.color = Color.Lerp(
            minColor,
            maxColor,
            Mathf.Lerp(
                minValue,
                maxValue,
                transform.localScale.x
            )
        );
    }
}
