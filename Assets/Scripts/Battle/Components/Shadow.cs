using UnityEngine;

public class Shadow : MonoBehaviour {
    public SpriteRenderer shadowPrefab;

    private Transform instance;

    private void Awake() {
        instance = Instantiate(shadowPrefab, transform).transform;
    }

    private void Start() {
        // Don't show a scaled shadow if the parent is self scaled
        instance.localScale = new Vector3(1f / instance.lossyScale.x, 1f / instance.lossyScale.y, 1f / instance.lossyScale.z);
    }
}
