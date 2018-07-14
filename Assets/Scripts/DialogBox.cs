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

    /**
     * Show a global dialog box
     * Can set a name if wanna display one
     */
    public void Show(int preset, string name = "") {
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
