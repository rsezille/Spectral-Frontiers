using DG.Tweening;
using SF;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlacingHUD : MonoBehaviour {
    private BattleManager battleManager;

    [Header("Dependencies")]
    public BattleState battleState;

    [Header("References")]
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
        if (battleState.currentBattleStep != BattleState.BattleStep.Placing) return;

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

        previousCharText.text = "Previous [" + InputManager.Previous.bindedKey + "]\n" + previousCharacter.name; //TODO [BETA] LanguageManager with a multi key translation

        // Next character text
        Character nextCharacter = battleManager.placing.GetNextPlacingChar();

        if (nextCharacter.boardCharacter != null) {
            nextCharText.color = Color.gray;
        } else {
            nextCharText.color = Color.white;
        }

        nextCharText.text = "Next [" + InputManager.Next.bindedKey + "]\n" + nextCharacter.name; //TODO [BETA] LanguageManager with a multi key translation
    }

    public void SetActiveWithAnimation(bool active, HUD.Speed speed = HUD.Speed.Normal) {
        float fSpeed = (int)speed / 1000f;

        if (active) {
            isGoingEnabled = true;
            gameObject.SetActive(true);

            placingHUDRect.anchoredPosition3D = new Vector3(0f, -250f, 0f);
            placingHUDRect.DOAnchorPos3D(new Vector3(0f, 0f, 0f), fSpeed).SetEase(Ease.OutCubic);
            
            BattleManager.instance.placing.RefreshStartBattleText();
        } else {
            if (!gameObject.activeSelf) return;
            
            isGoingEnabled = false;
            startBattleText.gameObject.SetActive(false);
            
            placingHUDRect.DOAnchorPos3D(new Vector3(0f, -250f, 0f), fSpeed).SetEase(Ease.OutCubic).OnComplete(DisableGameObject);
        }
    }

    void DisableGameObject() {
        if (isGoingEnabled) return;
        
        gameObject.SetActive(false);
    }
}
