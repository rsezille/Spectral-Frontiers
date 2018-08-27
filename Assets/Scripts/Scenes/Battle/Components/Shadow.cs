using DG.Tweening;
using UnityEngine;

public class Shadow : MonoBehaviour {
    private BattleManager battleManager;
    private Tween showAnimation;

    public SpriteRenderer instance;

    public SpriteRenderer shadowPrefab;
    public float nightOpacity = 0.4f;
    public float dayOpacity = 0.7f;

    private void Awake() {
        battleManager = BattleManager.instance;

        instance = Instantiate(shadowPrefab, transform);

        battleManager.OnLightChange += CheckOpacity;
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

    private void CheckOpacity() {
        if (showAnimation == null || (!showAnimation.IsActive() || showAnimation.IsComplete())) {
            instance.color = new Color(instance.color.r, instance.color.g, instance.color.b, Mathf.Lerp(nightOpacity, dayOpacity, battleManager.sunLight.GetNormalizedIntensity()));
        }
    }

    private void OnDestroy() {
        battleManager.OnLightChange -= CheckOpacity;
    }

    public void Show(float speed = 1f) {
        instance.color = new Color(instance.color.r, instance.color.g, instance.color.b, 0f);

        showAnimation = instance.DOColor(new Color(instance.color.r, instance.color.g, instance.color.b, Mathf.Lerp(nightOpacity, dayOpacity, battleManager.sunLight.GetNormalizedIntensity())), speed);
    }
}
