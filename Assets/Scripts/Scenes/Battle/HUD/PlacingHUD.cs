using DG.Tweening;
using SF;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlacingHUD : MonoBehaviour {
    private Tween startBattleTextAnimation;
    private bool isGoingEnabled = false;

    [Header("Dependencies")]
    public BattleState battleState;
    public Party party;
    public CharacterVariable currentPartyCharacter;
    public BattleCharacters battleCharacters;

    [Header("Direct references")]
    public Canvas canvas;
    public Text previousCharText;
    public Text nextCharText;
    public Text currentCharText;
    public Text startBattleText;

    public GameObject removeButton;

    public RectTransform placingHUDRect;

    private void Awake() {
        canvas.gameObject.SetActive(false);
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

        // Start battle text
        if (battleCharacters.player.Count <= 0 && startBattleText.gameObject.activeSelf) {
            if (startBattleTextAnimation != null) {
                startBattleTextAnimation.Pause();
            }

            startBattleText.gameObject.SetActive(false);
        } else if (battleCharacters.player.Count > 0 && !startBattleText.gameObject.activeSelf) {
            startBattleText.gameObject.SetActive(true);
            startBattleText.color = new Color(startBattleText.color.r, startBattleText.color.g, startBattleText.color.b, 0.3f);

            if (startBattleTextAnimation == null) {
                startBattleTextAnimation = startBattleText.DOColor(new Color(startBattleText.color.r, startBattleText.color.g, startBattleText.color.b, 1f), 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
            } else {
                startBattleTextAnimation.Play();
            }
        }
    }

    public void OnEnterBattleStepEvent(BattleState.BattleStep battleStep) {
        if (battleStep == BattleState.BattleStep.Placing) {
            SetActiveWithAnimation(true, HUD.Speed.Slow);
        }
    }

    public void OnLeaveBattleStepEvent(BattleState.BattleStep battleStep) {
        if (battleStep == BattleState.BattleStep.Placing) {
            canvas.gameObject.SetActive(false);
        }
    }

    public void OnEnterTurnStepEvent(BattleState.TurnStep turnStep) {
        if (battleState.currentBattleStep == BattleState.BattleStep.Placing) {
            if (turnStep == BattleState.TurnStep.None) {
                SetActiveWithAnimation(true);
            } else if (turnStep == BattleState.TurnStep.Status) {
                SetActiveWithAnimation(false);
            }
        }
    }

    public void Status() {
        battleState.currentTurnStep = BattleState.TurnStep.Status;
    }

    public void RemoveCurrentMapChar() {
        if (battleState.currentBattleStep != BattleState.BattleStep.Placing) {
            return;
        }

        if (currentPartyCharacter.value.boardCharacter == null) return;

        currentPartyCharacter.value.boardCharacter.Remove();

        if (battleCharacters.player.Count <= 0) {
            startBattleText.gameObject.SetActive(false);
        }
    }

    public void SetActiveWithAnimation(bool active, HUD.Speed speed = HUD.Speed.Normal) {
        float fSpeed = (int)speed / 1000f;

        if (active) {
            isGoingEnabled = true;
            canvas.gameObject.SetActive(true);

            placingHUDRect.anchoredPosition3D = new Vector3(0f, -250f, 0f);
            placingHUDRect.DOAnchorPos3D(new Vector3(0f, 0f, 0f), fSpeed).SetEase(Ease.OutCubic);
        } else {
            if (!gameObject.activeSelf) return;
            
            isGoingEnabled = false;
            startBattleText.gameObject.SetActive(false);
            
            placingHUDRect.DOAnchorPos3D(new Vector3(0f, -250f, 0f), fSpeed).SetEase(Ease.OutCubic).OnComplete(DisableGameObject);
        }
    }

    private void DisableGameObject() {
        if (isGoingEnabled) return;
        
        canvas.gameObject.SetActive(false);
    }
}
