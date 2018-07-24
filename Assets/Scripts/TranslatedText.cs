using SF;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/**
 * Automatically translate the attached UI Text (the default text must be an id specified in strings.json)
 * Work with Text (unity UI) or TextMeshProUGUI
 */
public class TranslatedText : MonoBehaviour {
    // Don't use awake as the LanguageManager needs to be loaded
    private void Start() {
        Text textBox = GetComponent<Text>();

        if (textBox != null) {
            textBox.text = LanguageManager.instance.GetString(textBox.text);
        } else { // Look for TextMesh
            TextMeshProUGUI textMesh = GetComponent<TextMeshProUGUI>();

            if (textMesh != null) {
                textMesh.SetText(LanguageManager.instance.GetString(textMesh.text));
            }
        }
    }
}
