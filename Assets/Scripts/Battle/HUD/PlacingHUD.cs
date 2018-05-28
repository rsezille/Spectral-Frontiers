using UnityEngine;
using UnityEngine.UI;

public class PlacingHUD : MonoBehaviour {
    public Text previousCharText;
    public Text nextCharText;
    public Text currentCharText;

    public Text startBattleText;

    public GameObject removeButton;
    public GameObject statusButton;

    public RectTransform placingHUDRect;

    void Start() {
        removeButton.GetComponent<Button>().onClick.AddListener(BattleManager.instance.placing.RemoveCurrentMapChar);
        statusButton.GetComponent<Button>().onClick.AddListener(BattleManager.instance.EnterTurnStepStatusFromPlacing);
    }

    void Update() {
        // Remove button
        if (BattleManager.instance.currentBattleStep == BattleManager.BattleStep.Placing) {
            if (BattleManager.instance.placing.GetCurrentPlacingChar().boardChar != null && !removeButton.activeSelf) {
                removeButton.SetActive(true);
            } else if (BattleManager.instance.placing.GetCurrentPlacingChar().boardChar == null && removeButton.activeSelf) {
                removeButton.SetActive(false);
            }
        }

        // Current character text
        if (BattleManager.instance.placing.GetCurrentPlacingChar().boardChar != null) {
            currentCharText.color = Color.gray;
        } else {
            currentCharText.color = Color.white;
        }

        currentCharText.text = BattleManager.instance.placing.GetCurrentPlacingChar().name;

        // Previous character text
        Character previousCharacter = BattleManager.instance.placing.GetPreviousPlacingChar();

        if (previousCharacter.boardChar != null) {
            previousCharText.color = Color.gray;
        } else {
            previousCharText.color = Color.white;
        }

        previousCharText.text = "Previous [" + KeyCode.A + "]\n" + previousCharacter.name; //TODO: Custom InputManager + LanguageManager with a multi key translation

        // Next character text
        Character nextCharacter = BattleManager.instance.placing.GetNextPlacingChar();

        if (nextCharacter.boardChar != null) {
            nextCharText.color = Color.gray;
        } else {
            nextCharText.color = Color.white;
        }

        nextCharText.text = "Next [" + KeyCode.E + "]\n" + nextCharacter.name; //TODO: Custom InputManager + LanguageManager with a multi key translation
    }

    public void SetActive(bool active) {
        StopAllCoroutines();

        if (active) {
            this.gameObject.SetActive(active);
            StartCoroutine(GenericCoroutine.MoveRectTransformSmooth(
                placingHUDRect,
                new Vector3(0f, -250f, 0f),
                new Vector3(0f, 0f, 0f),
                0.25f
            ));
            BattleManager.instance.placing.RefreshStartBattleText();
        } else {
            startBattleText.gameObject.SetActive(false);
            StartCoroutine(GenericCoroutine.MoveRectTransformSmooth(
                placingHUDRect,
                new Vector3(0f, 0f, 0f),
                new Vector3(0f, -250f, 0f),
                0.25f,
                DisableGameObject
            ));
        }
    }

    private void DisableGameObject() {
        this.gameObject.SetActive(false);
    }
}
