using SF;
using System.Text.RegularExpressions;
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

    private DialogPreset[] presets; // 0 is the default one

    public TextSpeed textSpeed;
    public Color playerTagColor = new Color(0f, 0.5f, 1f);
    public Color specialTagColor = Color.red;

    private float timerLetters = 0f;
    private int countLetters = 0;
    private DialogPreset currentShownPreset;
    private string parsedText = "";

    private void Awake() {
        gameManager = GameManager.instance;

        textSpeed = TextSpeed.Fast;

        presets = GetComponentsInChildren<DialogPreset>();

        if (presets.Length > 0) {
            foreach (DialogPreset preset in presets) {
                preset.gameObject.SetActive(false);
            }
        }
    }

    private void Update() {
        if (currentShownPreset != null) {
            if (countLetters <= currentShownPreset.textMesh.textInfo.pageInfo[currentShownPreset.textMesh.pageToDisplay - 1].lastCharacterIndex) {
                if (textSpeed == TextSpeed.Instant) {
                    countLetters = currentShownPreset.textMesh.textInfo.pageInfo[currentShownPreset.textMesh.pageToDisplay - 1].lastCharacterIndex;
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

                currentShownPreset.textMesh.SetText(tmpText.Replace("€", "<alpha=#00>"));
            }

            if (countLetters >= currentShownPreset.textMesh.textInfo.pageInfo[currentShownPreset.textMesh.pageToDisplay - 1].lastCharacterIndex
                   && currentShownPreset.textMesh.pageToDisplay < currentShownPreset.textMesh.textInfo.pageCount) {
                currentShownPreset.NextCursor();
            } else if (countLetters >= currentShownPreset.textMesh.textInfo.pageInfo[currentShownPreset.textMesh.pageToDisplay - 1].lastCharacterIndex
                    && currentShownPreset.textMesh.pageToDisplay >= currentShownPreset.textMesh.textInfo.pageCount
                    && currentShownPreset.textMesh.textInfo.pageInfo[currentShownPreset.textMesh.pageToDisplay - 1].lastCharacterIndex > 0) {
                currentShownPreset.EndCursor();
            } else {
                currentShownPreset.NoCursor();
            }

            if (InputManager.Confirm.IsKeyDown) {
                Next();
            }
        }
    }

    /**
     * Show a global dialog box
     */
    public void Show(string dialogId, int presetIndex = 0, string name = "") {
        if (currentShownPreset != null) {
            return;
        }

        if (presets.Length == 0) {
            Debug.LogWarning("No preset set, dialogbox will not be shown");

            return;
        }

        if (presetIndex >= presets.Length) {
            Debug.LogWarning("Preset out of bounds, will use default preset");
            presetIndex = 0;
        }

        ResetAllProperties();

        currentShownPreset = presets[presetIndex];
        currentShownPreset.gameObject.SetActive(true);
        currentShownPreset.NoCursor();
        currentShownPreset.textMesh.pageToDisplay = 1;

        parsedText = LanguageManager.instance.getDialog(dialogId).Replace("[player_name]", "".PadLeft(gameManager.player.playerName.Length, '£'));
        currentShownPreset.textMesh.SetText("");

        currentShownPreset.canvas.renderMode = RenderMode.ScreenSpaceCamera;
        transform.localPosition = Vector3.zero;
    }

    /**
     * Show a dialog box attached to a character, with his name
     */
    public void Show(BoardCharacter boardCharacter, string dialogId, int presetIndex = 0) {
        Show(dialogId, presetIndex, boardCharacter.character.name);

        currentShownPreset.canvas.renderMode = RenderMode.WorldSpace;
        transform.localPosition = boardCharacter.GetSquare().transform.localPosition;
        currentShownPreset.canvas.transform.localPosition = Vector3.zero;
    }

    private void Hide() {
        ResetAllProperties();
    }

    public void Next() {
        if (countLetters < currentShownPreset.textMesh.textInfo.pageInfo[currentShownPreset.textMesh.pageToDisplay - 1].lastCharacterIndex) { // Show instantly the current page
            countLetters = currentShownPreset.textMesh.textInfo.pageInfo[currentShownPreset.textMesh.pageToDisplay - 1].lastCharacterIndex;
        } else if (currentShownPreset.textMesh.pageToDisplay < currentShownPreset.textMesh.textInfo.pageCount) { // Show the next page
            currentShownPreset.textMesh.pageToDisplay++;
        } else if (currentShownPreset.textMesh.pageToDisplay >= currentShownPreset.textMesh.textInfo.pageCount
                && currentShownPreset.textMesh.textInfo.pageInfo[currentShownPreset.textMesh.pageToDisplay - 1].lastCharacterIndex > 0) { // End the dialog box
            Hide();
        }
    }

    private void ResetAllProperties() {
        if (currentShownPreset != null) {
            currentShownPreset.textMesh.pageToDisplay = 1;
        }

        timerLetters = 0f;
        countLetters = 0;
        currentShownPreset = null;
        parsedText = "";

        foreach (DialogPreset preset in presets) {
            preset.gameObject.SetActive(false);
        }
    }
}
