using SF;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

/**
 * Scope: GameManager (all scenes)
 * Be aware that BattleManager singleton isn't available here
 * /!\ Reserved characters: £, €, ¥
 * 
 * TODO? Priority input
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
    public Color playerTagColor = new Color(0f, 0.5f, 1f);
    public Color specialTagColor = Color.red;

    private float timerLetters = 0f;
    private int countLetters = 0;

    private void Awake() {
        gameManager = GameManager.instance;

        textSpeed = TextSpeed.Fast;

        if (presets == null) {
            presets = new Canvas[0]; // Avoid undefined
        }

        if (presets.Length > 0) {
            foreach (Canvas preset in presets) {
                preset.gameObject.SetActive(false);
            }
        }
    }

    //string initialText = "Lorem ipsum dolor sit amet, [player_name] adipiscing adipiscing adipiscing [player_name] adipiscing adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.";
    string initialText = "Bienvenue dans ce super monde [player_name] où règne traitrises et trahisons. [player_name] est prêt à arpenter ce monde plein de malédictions et de magie. La seule âme qui puisse être sauvé n'est sans nul doute que celle du faux héros de l'histoire, triomphant du mal et du boss final très moche. C'est sans nul doute que ce vis [player_name] pourra donner lieu à rien.";
    string parsedText = "";

    private void Update() {
        if (textMesh != null) {
            if (countLetters <= textMesh.textInfo.pageInfo[textMesh.pageToDisplay - 1].lastCharacterIndex) {
                if (textSpeed == TextSpeed.Instant) {
                    countLetters = textMesh.textInfo.pageInfo[textMesh.pageToDisplay - 1].lastCharacterIndex;
                } else {
                    timerLetters += Time.deltaTime * 1000;

                    while (timerLetters >= (int)textSpeed) {
                        countLetters++;
                        timerLetters -= (int)textSpeed;
                    }
                }

                string tmpText = parsedText.Insert(countLetters, "€");

                Match m = Regex.Match(tmpText, @"(£+€?£+)", RegexOptions.RightToLeft);

                while (m.Success) {
                    int alphaSymbol = m.Value.IndexOf("€");

                    // The <alpha> tag is erased by the <color> tag, that's why we re-put it after </color>
                    if (alphaSymbol != -1) {
                        tmpText = tmpText.Remove(m.Index, m.Value.Length).Insert(m.Index,
                            "<color=#" + ColorUtility.ToHtmlStringRGB(playerTagColor) + ">" + gameManager.player.playerName.Substring(0, alphaSymbol) + "</color>"
                            + "<alpha=#00>" + gameManager.player.playerName.Substring(alphaSymbol)
                        );
                    } else {
                        if (countLetters < m.Index) { // If the alpha symbol is before, meaning that the tag is hidden, no need to put colors
                            tmpText = tmpText.Remove(m.Index, m.Value.Length).Insert(m.Index, gameManager.player.playerName);
                        } else { // Otherwise if the alpha symbol is after, we need to set the color
                            tmpText = tmpText.Remove(m.Index, m.Value.Length).Insert(m.Index, "<color=#" + ColorUtility.ToHtmlStringRGB(playerTagColor) + ">" + gameManager.player.playerName + "</color>");
                        }
                    }

                    m = m.NextMatch();
                }

                textMesh.SetText(tmpText.Replace("€", "<alpha=#00>"));
            }

            if (InputManager.Confirm.IsKeyDown) {
                if (countLetters < textMesh.textInfo.pageInfo[textMesh.pageToDisplay - 1].lastCharacterIndex) { // Show instantly the current page
                    countLetters = textMesh.textInfo.pageInfo[textMesh.pageToDisplay - 1].lastCharacterIndex;
                } else if (textMesh.pageToDisplay < textMesh.textInfo.pageCount) { // Show the next page
                    textMesh.pageToDisplay++;
                } else if (textMesh.pageToDisplay >= textMesh.textInfo.pageCount) { // End the dialog box
                    Hide();
                }
            }
        }
    }

    TextMeshProUGUI textMesh;

    /**
     * Show a global dialog box
     */
    public void Show(string dialogId, int presetIndex = 0, string name = "") {
        Debug.Log("Show:   " + presetIndex);

        ResetAllProperties();

        if (presets.Length == 0) {
            Debug.LogWarning("No preset set, dialogbox will not be shown");
        }

        if (presetIndex >= presets.Length) {
            Debug.LogWarning("Preset out of bounds, will use default preset");
            presetIndex = 0;
        }

        presets[presetIndex].gameObject.SetActive(true);
        textMesh = presets[presetIndex].transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();

        parsedText = LanguageManager.instance.getDialog(dialogId).Replace("[player_name]", "".PadLeft(gameManager.player.playerName.Length, '£'));
        textMesh.richText = true;
        textMesh.text = "";
        
        //EnablePreset(preset);   // The dialogbox prefab comes with several sub game objects containing a background and a canvas with a textmesh pro text
        // Allowing to customize the text per background and so on
        // Should also contain a little arrow to point the dialogbox to the character
        //GetTextFromPreset(); // Just do a preset.GetComponentInChildren<TextMesh>
        //EnableDialogBox(); // By enabling the game object and try to use DOTween to animate the text ?
    }

    private void Hide() {
        ResetAllProperties();
    }

    private void ResetAllProperties() {
        timerLetters = 0f;
        countLetters = 0;
        textMesh = null;

        foreach (Canvas preset in presets) {
            preset.gameObject.SetActive(false);
        }
    }

    /**
     * Show a dialog box attached to a character, with his name
     */
    public void Show(BoardCharacter boardCharacter) {

    }
}
