using UnityEngine;
using UnityEngine.UI;

/**
 * Automatically translate the attached UI Text (the default text must be an id specified in strings.json)
 */
[RequireComponent(typeof(Text))]
public class TranslatedText : MonoBehaviour {
    Text textBox;

    // Don't use awake as the LanguageManager needs to be loaded
    void Start() {
        if (textBox == null) {
            textBox = GetComponent<Text>();
        }

        textBox.text = LanguageManager.getInstance().getString(textBox.text);
    }
}
