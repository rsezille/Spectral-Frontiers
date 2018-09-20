using SF;
using System.Text.RegularExpressions;
using UnityEngine;

/**
 * Scope: GameManager (all scenes)
 * Be aware that BattleManager singleton isn't available here
 * /!\ Reserved characters: £, €, ¥
 * 
 * Can be used inside a coroutine with WaitForCustom:
 *      yield return new WaitForCustom(GameManager.instance.DialogBox.Show("prologue_01"));
 * 
 * TODO? Priority input
 */
public class DialogBox : MonoBehaviour, IWaitForCustom {
    public enum TextSpeed {
        VerySlow = 180,
        Slow = 135,
        Average = 80,
        Fast = 40,
        VeryFast = 20,
        Instant = 0,
    };

    public enum Position {
        Top, Bottom
    };

    private GameManager gameManager; // Shortcut

    private DialogPreset[] presets; // 0 is the default one
    private BoardCharacter attachedCharacter;
    private DialogStyle dialogStyle;

    [Header("Dependencies")]
    public StringVariable playerName;

    [Header("Data")]
    public TextSpeed textSpeed;
    public Color playerTagColor = new Color(0f, 0.5f, 1f);
    public Color specialTagColor = Color.red;

    private float timerLetters = 0f;
    private int countLetters = 0;
    private DialogPreset currentShownPreset;
    private string parsedText = "";

    private void Awake() {
        gameManager = GameManager.instance;

        textSpeed = EnumUtil.ParseEnum(PlayerOptions.GetString(PlayerOptions.TextSpeed), TextSpeed.Fast);

        presets = GetComponentsInChildren<DialogPreset>();

        if (presets.Length > 0) {
            foreach (DialogPreset preset in presets) {
                preset.gameObject.SetActive(false);
            }
        }
    }

    private void Update() {
        if (currentShownPreset == null) return;

        // Letters
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

            if (countLetters > parsedText.Length) {
                countLetters = parsedText.Length;
            }

            string tmpText = parsedText.Insert(countLetters, "€");

            Match m = Regex.Match(tmpText, @"(£+€?£+)", RegexOptions.RightToLeft);

            while (m.Success) {
                int alphaSymbol = m.Value.IndexOf("€");

                // The <alpha> tag is erased by the <color> tag, that's why we re-put it after </color>
                if (alphaSymbol != -1) {
                    tmpText = tmpText.Remove(m.Index, m.Value.Length).Insert(m.Index,
                        "<color=#" + ColorUtility.ToHtmlStringRGB(playerTagColor) + ">" + playerName.value.Substring(0, alphaSymbol) + "</color>"
                        + "<alpha=#00>" + playerName.value.Substring(alphaSymbol)
                    );
                } else {
                    if (countLetters < m.Index) { // If the alpha symbol is before, meaning that the tag is hidden, no need to put colors
                        tmpText = tmpText.Remove(m.Index, m.Value.Length).Insert(m.Index, playerName.value);
                    } else { // Otherwise if the alpha symbol is after, we need to set the color
                        tmpText = tmpText.Remove(m.Index, m.Value.Length).Insert(m.Index, "<color=#" + ColorUtility.ToHtmlStringRGB(playerTagColor) + ">" + playerName.value + "</color>");
                    }
                }

                m = m.NextMatch();
            }

            string finalText = tmpText.Replace("€", "<alpha=#00>");

            switch (dialogStyle) {
                case DialogStyle.Italic:
                    finalText = "<i>" + finalText + "</i>";
                    break;
                case DialogStyle.Bold:
                    finalText = "<b>" + finalText + "</i>";
                    break;
                case DialogStyle.BoldAndItalic:
                    finalText = "<b><i>" + finalText + "</i></b>";
                    break;
            }

            currentShownPreset.textMesh.SetText(finalText);
        }

        // Cursor
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

        // Key bindings
        if (InputManager.Confirm.IsKeyDown) {
            Next();
        }

        // The dialog box must follow the character if attached to one
        if (attachedCharacter != null) {
            transform.localPosition = attachedCharacter.transform.position;
        }
    }

    /**
     * Show a global dialog box
     */
    public IWaitForCustom Show(string dialogId, Position position = Position.Bottom, DialogStyle style = DialogStyle.Normal, int presetIndex = 0, string name = "") {
        if (currentShownPreset != null) {
            return null;
        }

        PreShow(dialogId, presetIndex, name);
        attachedCharacter = null;
        dialogStyle = style;

        currentShownPreset.canvas.renderMode = RenderMode.ScreenSpaceCamera;
        transform.localPosition = Vector3.zero;

        switch (position) {
            case Position.Bottom:
                currentShownPreset.image.rectTransform.anchorMin = new Vector2(0.5f, 0f);
                currentShownPreset.image.rectTransform.anchorMax = new Vector2(0.5f, 0f);
                
                currentShownPreset.image.rectTransform.anchoredPosition = new Vector3(0f, currentShownPreset.yOffset);
                break;
            case Position.Top:
                currentShownPreset.image.rectTransform.anchorMin = new Vector2(0.5f, 1f);
                currentShownPreset.image.rectTransform.anchorMax = new Vector2(0.5f, 1f);

                currentShownPreset.image.rectTransform.anchoredPosition = new Vector3(0f, - currentShownPreset.yOffset);
                break;
        }

        return this;
    }

    /**
     * Show a dialog box attached to a character, with his name
     */
    public IWaitForCustom Show(BoardCharacter boardCharacter, string dialogId, Position position = Position.Top, DialogStyle style = DialogStyle.Normal, int presetIndex = 0) {
        if (currentShownPreset != null) {
            return null;
        }

        PreShow(dialogId, presetIndex, boardCharacter.character.name);
        attachedCharacter = boardCharacter;
        dialogStyle = style;

        currentShownPreset.canvas.renderMode = RenderMode.WorldSpace;
        transform.localPosition = boardCharacter.transform.position;
        currentShownPreset.canvas.transform.localPosition = Vector3.zero;
        currentShownPreset.image.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        currentShownPreset.image.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        currentShownPreset.image.rectTransform.anchoredPosition = Vector3.zero;

        switch (position) {
            case Position.Bottom:
                currentShownPreset.image.rectTransform.anchoredPosition = new Vector3(0f, - currentShownPreset.yOffset, 0f);
                break;
            case Position.Top:
                currentShownPreset.image.rectTransform.anchoredPosition = new Vector3(0f, boardCharacter.sprite.bounds.size.y * Globals.PixelsPerUnit + currentShownPreset.yOffset, 0f);
                break;
        }

        return this;
    }

    private void PreShow(string dialogId, int presetIndex = 0, string name = "") {
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

        if (name != "") {
            currentShownPreset.nameImage.gameObject.SetActive(true);
            currentShownPreset.nameTextMesh.SetText(name);
        } else {
            currentShownPreset.nameImage.gameObject.SetActive(false);
        }

        parsedText = LanguageManager.instance.GetDialog(dialogId).Replace("[player_name]", "".PadLeft(playerName.value.Length, '£'));
        currentShownPreset.textMesh.SetText("");
    }

    public void Hide() {
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

        dialogStyle = DialogStyle.Normal;

        foreach (DialogPreset preset in presets) {
            preset.gameObject.SetActive(false);
        }
    }

    public bool IsFinished() {
        return currentShownPreset == null;
    }
}
