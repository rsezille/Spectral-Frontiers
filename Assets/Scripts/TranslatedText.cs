using SF;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/**
 * Automatically translate the attached UI Text (the default text must be an id specified in strings.json)
 * Work with Text (unity UI) or TextMeshProUGUI
 */
public class TranslatedText : MonoBehaviour {
    public string key;

    private void Awake() {
        if (string.IsNullOrEmpty(key)) {
            Text textBox = GetComponent<Text>();

            if (textBox != null) {
                key = textBox.text;
            } else {
                TextMeshProUGUI textMesh = GetComponent<TextMeshProUGUI>();

                if (textMesh != null) {
                    key = textMesh.text;
                }
            }
        }
    }

    // Don't use awake as the LanguageManager needs to be loaded
    private void Start() {
        GameManager.instance.OnLanguageChange += SetText;

        SetText();
    }

    private void SetText() {
        Text textBox = GetComponent<Text>();

        if (textBox != null) {
            textBox.text = LanguageManager.instance.GetString(key);
        } else {
            TextMeshProUGUI textMesh = GetComponent<TextMeshProUGUI>();

            if (textMesh != null) {
                textMesh.SetText(LanguageManager.instance.GetString(key));
            }
        }
    }

    private void OnDestroy() {
        GameManager.instance.OnLanguageChange -= SetText;
    }
}
