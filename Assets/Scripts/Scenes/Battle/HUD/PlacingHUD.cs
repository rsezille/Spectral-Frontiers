using DG.Tweening;
using SF;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlacingHUD : MonoBehaviour {
    private BattleManager battleManager;

    [Header("Dependencies")]
    public BattleState battleState;
    public Party party;
    public CharacterVariable currentPartyCharacter;
    public BattleCharacters battleCharacters;

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

        // Current character text
        if (currentPartyCharacter.value.boardCharacter != null) {
            currentCharText.color = Color.gray;

            if (!removeButton.activeSelf) {
                removeButton.SetActive(true);
            }
        } else {
            currentCharText.color = Color.white;

            if (removeButton.activeSelf) {
                removeButton.SetActive(false);
            }
        }

        currentCharText.text = currentPartyCharacter.value.characterName;

        // Previous character text
        Character previousCharacter = party.GetPreviousCharacter(currentPartyCharacter.value);

        if (previousCharacter.boardCharacter != null) {
            previousCharText.color = Color.gray;
        } else {
            previousCharText.color = Color.white;
        }

        previousCharText.text = "Previous [" + InputManager.Previous.bindedKey + "]\n" + previousCharacter.characterName; //TODO [BETA] LanguageManager with a multi key translation

        // Next character text
        Character nextCharacter = party.GetNextCharacter(currentPartyCharacter.value);

        if (nextCharacter.boardCharacter != null) {
            nextCharText.color = Color.gray;
        } else {
            nextCharText.color = Color.white;
        }

        nextCharText.text = "Next [" + InputManager.Next.bindedKey + "]\n" + nextCharacter.characterName; //TODO [BETA] LanguageManager with a multi key translation
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
