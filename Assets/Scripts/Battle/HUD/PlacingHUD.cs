using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlacingHUD : MonoBehaviour {
    private BattleManager battleManager;

    public Text previousCharText;
    public Text nextCharText;
    public Text currentCharText;

    public Text startBattleText;

    public GameObject removeButton;
    public GameObject statusButton;

    public RectTransform placingHUDRect;

    private bool isGoingEnabled = false;

    private void Awake() {
        battleManager = BattleManager.instance;
    }

    private void Start() {
        removeButton.AddListener(EventTriggerType.PointerClick, battleManager.placing.RemoveCurrentMapChar);
        statusButton.AddListener(EventTriggerType.PointerClick, battleManager.EnterTurnStepStatus);
    }

    private void Update() {
        if (battleManager.currentBattleStep != BattleManager.BattleStep.Placing) return;

        // Remove button
        if (battleManager.placing.GetCurrentPlacingChar().boardCharacter != null && !removeButton.activeSelf) {
            removeButton.SetActive(true);
        } else if (battleManager.placing.GetCurrentPlacingChar().boardCharacter == null && removeButton.activeSelf) {
            removeButton.SetActive(false);
        }

        // Current character text
        if (battleManager.placing.GetCurrentPlacingChar().boardCharacter != null) {
            currentCharText.color = Color.gray;
        } else {
            currentCharText.color = Color.white;
        }

        currentCharText.text = battleManager.placing.GetCurrentPlacingChar().name;

        // Previous character text
        Character previousCharacter = battleManager.placing.GetPreviousPlacingChar();

        if (previousCharacter.boardCharacter != null) {
            previousCharText.color = Color.gray;
        } else {
            previousCharText.color = Color.white;
        }

        previousCharText.text = "Previous [" + KeyCode.A + "]\n" + previousCharacter.name; //TODO: Custom InputManager + LanguageManager with a multi key translation

        // Next character text
        Character nextCharacter = battleManager.placing.GetNextPlacingChar();

        if (nextCharacter.boardCharacter != null) {
            nextCharText.color = Color.gray;
        } else {
            nextCharText.color = Color.white;
        }

        nextCharText.text = "Next [" + KeyCode.E + "]\n" + nextCharacter.name; //TODO: Custom InputManager + LanguageManager with a multi key translation
    }

    public void SetActiveWithAnimation(bool active) {
        float speed = 0.6f;
        
        if (active) {
            isGoingEnabled = true;
            this.gameObject.SetActive(true);

            if (placingHUDRect.anchoredPosition3D != new Vector3(0f, 0f, 0f)) {
                placingHUDRect.anchoredPosition3D = new Vector3(0f, -250f, 0f);
                placingHUDRect.DOAnchorPos3D(new Vector3(0f, 0f, 0f), speed).SetEase(Ease.OutCubic);
            }
            
            BattleManager.instance.placing.RefreshStartBattleText();
        } else {
            isGoingEnabled = false;
            startBattleText.gameObject.SetActive(false);
            
            placingHUDRect.DOAnchorPos3D(new Vector3(0f, -250f, 0f), speed).SetEase(Ease.OutCubic).OnComplete(DisableGameObject);
        }
    }

    void DisableGameObject() {
        if (isGoingEnabled) return;
        
        gameObject.SetActive(false);
    }
}
