using DG.Tweening;
using SF;
using UnityEngine;

public class Shadow : MonoBehaviour {
    private Tween showAnimation;

    [Header("Dependencies")]
    public FloatVariable sunIntensity;
    public SunSettings sunSettings;

    public SpriteRenderer instance;

    public SpriteRenderer shadowPrefab;
    public float nightOpacity = 0.4f;
    public float dayOpacity = 0.7f;

    private void Awake() {
        instance = Instantiate(shadowPrefab, transform);
    }

    private void Start() {
        // Don't show a scaled shadow if the parent is self scaled
        instance.transform.localScale = new Vector3(1f / instance.transform.lossyScale.x, 1f / instance.transform.lossyScale.y, 1f / instance.transform.lossyScale.z);

        CheckOpacity();
    }

    #if UNITY_EDITOR
    private void Update() {
        CheckOpacity();
    }
    #endif

    public void CheckOpacity() {
        if (showAnimation == null || (!showAnimation.IsActive() || showAnimation.IsComplete())) {
            instance.color = new Color(instance.color.r, instance.color.g, instance.color.b, Mathf.Lerp(nightOpacity, dayOpacity, sunSettings.GetNormalizedIntensity(sunIntensity.value)));
        }
    }

    public void Show(float speed = 1f) {
        instance.color = new Color(instance.color.r, instance.color.g, instance.color.b, 0f);

        showAnimation = instance.DOColor(new Color(instance.color.r, instance.color.g, instance.color.b, Mathf.Lerp(nightOpacity, dayOpacity, sunSettings.GetNormalizedIntensity(sunIntensity.value))), speed);
    }
}
