using UnityEngine;

public class DialogBox : MonoBehaviour {
    public enum TextSpeed {
        VerySlow = 180,
        Slow = 135,
        Average = 80,
        Fast = 40,
        VeryFast = 20,
        Instant = 0,
    };

    public Canvas[] presets; // 0 is the default one

    private void Awake() {
        if (presets == null) {
            presets = new Canvas[0]; // Avoid undefined
        }

        if (presets.Length > 0) {
            foreach (Canvas preset in presets) {
                preset.gameObject.SetActive(false);
            }
        }
    }

    /**
     * Show a global dialog box
     * Can set a name if wanna display one
     */
    public void Show(int presetIndex, string name = "") {
        Debug.Log("Show:   " + presetIndex);

        if (presets.Length == 0) {
            Debug.LogWarning("No preset set, dialogbox will not be shown");
        }

        if (presetIndex >= presets.Length) {
            Debug.LogWarning("Preset out of bounds, will use default preset");
            presetIndex = 0;
        }

        presets[presetIndex].gameObject.SetActive(true);

        //ResetAllProperties();
        //EnablePreset(preset);   // The dialogbox prefab comes with several sub game objects containing a background and a canvas with a textmesh pro text
        // Allowing to customize the text per background and so on
        // Should also contain a little arrow to point the dialogbox to the character
        //GetTextFromPreset(); // Just do a preset.GetComponentInChildren<TextMesh>
        //EnableDialogBox(); // By enabling the game object and try to use DOTween to animate the text ?
    }

    /**
     * Show this dialog box on the board character, with his name
     */
    public void Show(BoardCharacter boardCharacter) {

    }
}
