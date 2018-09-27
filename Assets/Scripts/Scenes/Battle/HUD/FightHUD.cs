using DG.Tweening;
using SF;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FightHUD : MonoBehaviour {
    private Text currentSquareText;

    [Header("Dependencies")]
    public BoardCharacterVariable currentFightBoardCharacter;
    public BattleState battleState;

    [Header("Events")]
    public GameEvent endTurn;

    [Header("References")]
    public Canvas canvas;
    public GameObject moveButton;
    public GameObject actionButton;
    public GameObject statusButton;
    public GameObject directionButton;
    public GameObject endTurnButton;

    public ActionMenu actionMenu;

    public RectTransform fightMenu;
    public RectTransform currentSquare;
    public RectTransform selectedSquare;

    private bool isGoingEnabled = false;

    private void Awake() {
        currentSquareText = currentSquare.GetComponentInChildren<Text>();

        canvas.gameObject.SetActive(false);
    }

    private void Start() {
        moveButton.AddListener(EventTriggerType.PointerClick, Move);
        actionButton.AddListener(EventTriggerType.PointerClick, Action);
        statusButton.AddListener(EventTriggerType.PointerClick, Status);
        directionButton.AddListener(EventTriggerType.PointerClick, Direction);
        endTurnButton.AddListener(EventTriggerType.PointerClick, EndTurn);
    }

    private void Update() {
        if (currentFightBoardCharacter.value != null) {
            currentSquareText.text =
                currentFightBoardCharacter.value.character.characterName +
                "\n" + currentFightBoardCharacter.value.character.currentHp + "/" + currentFightBoardCharacter.value.character.maxHP + " HP";

            moveButton.GetComponent<Button>().interactable = currentFightBoardCharacter.value.CanMove();

            actionButton.GetComponent<Button>().interactable = currentFightBoardCharacter.value.CanDoAction();
            actionMenu.Refresh();

            statusButton.GetComponent<Button>().interactable = true;
            endTurnButton.GetComponent<Button>().interactable = true;
        } else {
            currentSquareText.text = "No currently selected character";
        }
    }

    public void OnEnterBattleStepEvent(BattleState.BattleStep battleStep) {
        if (battleStep == BattleState.BattleStep.Fight) {
            SetActiveWithAnimation(true);
        }
    }

    public void OnLeaveBattleStepEvent(BattleState.BattleStep battleStep) {
        if (battleStep == BattleState.BattleStep.Fight) {
            SetActiveWithAnimation(false);
        }
    }

    public void OnEnterTurnStepEvent(BattleState.TurnStep turnStep) {
        if (battleState.currentBattleStep == BattleState.BattleStep.Fight) {
            if (turnStep == BattleState.TurnStep.Status || turnStep == BattleState.TurnStep.Direction) {
                SetActiveWithAnimation(false);
            }
        }
    }

    public void OnLeaveTurnStepEvent(BattleState.TurnStep turnStep) {
        if (battleState.currentBattleStep == BattleState.BattleStep.Fight) {
            if (turnStep == BattleState.TurnStep.Status || turnStep == BattleState.TurnStep.Direction) {
                SetActiveWithAnimation(true);
            }
        }
    }

    private void Move() {
        if (battleState.currentTurnStep == BattleState.TurnStep.Move) {
            battleState.currentTurnStep = BattleState.TurnStep.None;
        } else {
            if (currentFightBoardCharacter.value.CanMove()) {
                battleState.currentTurnStep = BattleState.TurnStep.Move;
            }

            actionMenu.SetActiveWithAnimation(false);
        }
    }

    private void Action() {
        battleState.currentTurnStep = BattleState.TurnStep.None;

        if (currentFightBoardCharacter.value.CanDoAction()) {
            actionMenu.Toggle();
        }
    }

    private void Status() {
        battleState.currentTurnStep = BattleState.TurnStep.Status;
    }

    private void Direction() {
        battleState.currentTurnStep = BattleState.TurnStep.Direction;
    }
    
    public void EndTurn() {
        actionMenu.SetActiveWithAnimation(false);
        // TODO [ALPHA] FlashMessage
        // TODO [ALPHA] Disable inputs

        /*if (currentFightBoardCharacter.value.glow != null) {
            currentFightBoardCharacter.value.glow.Disable();
        }*/

        //EnterTurnStepEnemy();
        endTurn.Raise();
    }

    public void SetActiveWithAnimation(bool active, HUD.Speed speed = HUD.Speed.Normal) {
        float fSpeed = (int)speed / 1000f;

        if (active) {
            isGoingEnabled = true;
            canvas.gameObject.SetActive(true);

            fightMenu.anchoredPosition3D = new Vector3(fightMenu.anchoredPosition3D.x, -120f, fightMenu.anchoredPosition3D.z);
            fightMenu.DOAnchorPos3D(new Vector3(fightMenu.anchoredPosition3D.x, fightMenu.sizeDelta.y / 2f, fightMenu.anchoredPosition3D.z), fSpeed).SetEase(Ease.OutCubic);

            currentSquare.anchoredPosition3D = new Vector3(currentSquare.anchoredPosition3D.x, -120f, currentSquare.anchoredPosition3D.z);
            currentSquare.DOAnchorPos3D(new Vector3(currentSquare.anchoredPosition3D.x, currentSquare.sizeDelta.y / 2f, currentSquare.anchoredPosition3D.z), fSpeed).SetEase(Ease.OutCubic);

            selectedSquare.anchoredPosition3D = new Vector3(selectedSquare.anchoredPosition3D.x, -120f, selectedSquare.anchoredPosition3D.z);
            selectedSquare.DOAnchorPos3D(new Vector3(selectedSquare.anchoredPosition3D.x, selectedSquare.sizeDelta.y / 2f, selectedSquare.anchoredPosition3D.z), fSpeed).SetEase(Ease.OutCubic);
        } else {
            if (!canvas.gameObject.activeSelf) return;

            actionMenu.SetActiveWithAnimation(false);

            isGoingEnabled = false;

            fightMenu.DOAnchorPos3D(new Vector3(fightMenu.anchoredPosition3D.x, -120f, fightMenu.anchoredPosition3D.z), fSpeed).SetEase(Ease.OutCubic);
            currentSquare.DOAnchorPos3D(new Vector3(currentSquare.anchoredPosition3D.x, -120f, currentSquare.anchoredPosition3D.z), fSpeed).SetEase(Ease.OutCubic);
            selectedSquare.DOAnchorPos3D(new Vector3(selectedSquare.anchoredPosition3D.x, -120f, selectedSquare.anchoredPosition3D.z), fSpeed).SetEase(Ease.OutCubic).OnComplete(DisableGameObject);
        }
    }

    private void DisableGameObject() {
        if (isGoingEnabled) return;

        canvas.gameObject.SetActive(false);
    }

    public void SquareHovered(Square square) {
        // if (BattleManager.instance.currentBattleStep != BattleManager.BattleStep.Fight) return;
        // It would be more optimized but we would lose the current square information when entering FightStep

        Text selectedSquareText = selectedSquare.GetComponentInChildren<Text>();

        if (square == null) {
            selectedSquareText.text = "Square: none";
        } else {
            selectedSquareText.text = "Square: [" + square.x + "," + square.y + "]";

            if (square.boardEntity != null) {
                BoardCharacter boardCharacter = square.boardEntity.GetComponent<BoardCharacter>();

                if (boardCharacter != null) {
                    selectedSquareText.text += "\nCharacter: " + boardCharacter.character.characterName + " (" + boardCharacter.character.currentHp + "/" + boardCharacter.character.maxHP + ")";
                } else {
                    selectedSquareText.text += "\nCharacter: none";
                }
            } else {
                selectedSquareText.text += "\nCharacter: none";
            }
        }
    }
}
