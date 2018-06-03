using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour {
    public string text;
    private Text textComponent;

    // Use this for initialization
    private void Start() {
        textComponent = gameObject.GetComponentInChildren<Text>();
        textComponent.text = text;

        transform.DOMoveY(1f, 2f).OnComplete(() => Destroy(gameObject));
    }
}
