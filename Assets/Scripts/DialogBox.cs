using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

/**
 * Scope: GameManager (all scenes)
 * Be aware that BattleManager singleton isn't available here
 */
public class DialogBox : MonoBehaviour {
    public enum TextSpeed {
        VerySlow = 180,
        Slow = 135,
        Average = 80,
        Fast = 40,
        VeryFast = 20,
        Instant = 0,
    };

    private GameManager gameManager; // Shortcut

    public Canvas[] presets; // 0 is the default one

    public TextSpeed textSpeed;

    private void Awake() {
        gameManager = GameManager.instance;

        textSpeed = TextSpeed.VeryFast;

        if (presets == null) {
            presets = new Canvas[0]; // Avoid undefined
        }

        if (presets.Length > 0) {
            foreach (Canvas preset in presets) {
                preset.gameObject.SetActive(false);
            }
        }
    }

    string initialText = "Lorem ipsum dolor sit amet, [player_name] adipiscing adipiscing adipiscing [player_name] adipiscing adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.";
    int countLetters = 0;
    bool shown = false;
    public float timerLetters = 0f;

    private void Update() {
        if (shown && textMesh != null) {
            if (countLetters < initialText.Length) {
                if (textSpeed == TextSpeed.Instant) {
                    textMesh.text = initialText;
                    //Canvas.ForceUpdateCanvases();
                    //countLetters = dialog.cachedTextGenerator.characterCountVisible + 2;
                    countLetters = initialText.Length;
                } else {
                    timerLetters += Time.deltaTime * 1000;

                    while (timerLetters >= (int)textSpeed) {
                        countLetters++;
                        timerLetters -= (int)textSpeed;
                    }
                }
            }

            countLetters = Mathf.Min(countLetters, initialText.Length);
            string tmpText = initialText.Substring(0, countLetters);



            Match m = Regex.Match(initialText.Substring(0, countLetters), @"(£+)", RegexOptions.RightToLeft);

            while (m.Success) {
                tmpText = tmpText.Remove(m.Index, m.Value.Length).Insert(m.Index, "<color=\"red\">" + gameManager.player.playerName.Substring(0, m.Value.Length) + "</color>");
                m = m.NextMatch();
            }

            Debug.Log("rich?   " + textMesh.text.Length + "                         " + textMesh.GetParsedText().Length);

            textMesh.SetText(tmpText);
            //textMesh.text = tmpText;
        }
    }

    TextMeshProUGUI textMesh;

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
        textMesh = presets[presetIndex].transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();

        initialText = initialText.Replace("[player_name]", "".PadLeft(gameManager.player.playerName.Length, '£'));
        textMesh.richText = true;
        textMesh.text = initialText;

        Debug.Log("Pouet    " + textMesh.textInfo.characterCount + "    " + textMesh.textInfo.lineCount + "     " + textMesh.GetTextInfo(initialText).lineCount);
        Debug.Log("Text:   " + textMesh.text);

        ResetAllProperties();
        //EnablePreset(preset);   // The dialogbox prefab comes with several sub game objects containing a background and a canvas with a textmesh pro text
        // Allowing to customize the text per background and so on
        // Should also contain a little arrow to point the dialogbox to the character
        //GetTextFromPreset(); // Just do a preset.GetComponentInChildren<TextMesh>
        //EnableDialogBox(); // By enabling the game object and try to use DOTween to animate the text ?
    }

    private void ResetAllProperties() {
        timerLetters = 0f;
        shown = true;
        
    }

    /**
     * Show this dialog box on the board character, with his name
     */
    public void Show(BoardCharacter boardCharacter) {

    }
}
