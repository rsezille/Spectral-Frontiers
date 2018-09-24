using DG.Tweening;
using SF;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FightHUD : MonoBehaviour {
    private BattleManager battleManager;

    [Header("Dependencies")]
    public BoardCharacterVariable currentFightBoardCharacter;

    [Header("References")]
    public GameObject moveButton;
    public GameObject actionButton;
    public GameObject previousButton;
    public GameObject nextButton;
    public GameObject statusButton;
    public GameObject directionButton;
    public GameObject endTurnButton;

    public ActionMenu actionMenu;

    public RectTransform fightMenu;
    public RectTransform currentSquare;
    public RectTransform selectedSquare;

    private bool isGoingEnabled = false;

    private void Awake() {
        battleManager = BattleManager.instance;
    }

    private void Start() {
        moveButton.AddListener(EventTriggerType.PointerClick, battleManager.fight.Move);
        actionButton.AddListener(EventTriggerType.PointerClick, battleManager.fight.Action);
        previousButton.AddListener(EventTriggerType.PointerClick, battleManager.fight.Previous);
        nextButton.AddListener(EventTriggerType.PointerClick, battleManager.fight.Next);
        statusButton.AddListener(EventTriggerType.PointerClick, battleManager.fight.Status);
        directionButton.AddListener(EventTriggerType.PointerClick, battleManager.fight.Direction);
        endTurnButton.AddListener(EventTriggerType.PointerClick, battleManager.fight.EndTurn);
    }

    // Compute all checks on buttons availability
    public void Refresh() {
        if (currentFightBoardCharacter.value == null) return;
        
        moveButton.GetComponent<Button>().interactable = currentFightBoardCharacter.value.CanMove();
        
        actionButton.GetComponent<Button>().interactable = currentFightBoardCharacter.value.CanDoAction();
        actionMenu.Refresh();

        previousButton.GetComponent<Button>().interactable = true; //TODO [ALPHA] if no other character available, disable it
        nextButton.GetComponent<Button>().interactable = true; //TODO [ALPHA] if no other character available, disable it
        statusButton.GetComponent<Button>().interactable = true;
        endTurnButton.GetComponent<Button>().interactable = true;
    }

    public void SetActiveWithAnimation(bool active, HUD.Speed speed = HUD.Speed.Normal) {
        float fSpeed = (int)speed / 1000f;

        if (active) {
            isGoingEnabled = true;
            gameObject.SetActive(true);

            fightMenu.anchoredPosition3D = new Vector3(fightMenu.anchoredPosition3D.x, -120f, fightMenu.anchoredPosition3D.z);
            fightMenu.DOAnchorPos3D(new Vector3(fightMenu.anchoredPosition3D.x, fightMenu.sizeDelta.y / 2f, fightMenu.anchoredPosition3D.z), fSpeed).SetEase(Ease.OutCubic);

            currentSquare.anchoredPosition3D = new Vector3(currentSquare.anchoredPosition3D.x, -120f, currentSquare.anchoredPosition3D.z);
            currentSquare.DOAnchorPos3D(new Vector3(currentSquare.anchoredPosition3D.x, currentSquare.sizeDelta.y / 2f, currentSquare.anchoredPosition3D.z), fSpeed).SetEase(Ease.OutCubic);

            selectedSquare.anchoredPosition3D = new Vector3(selectedSquare.anchoredPosition3D.x, -120f, selectedSquare.anchoredPosition3D.z);
            selectedSquare.DOAnchorPos3D(new Vector3(selectedSquare.anchoredPosition3D.x, selectedSquare.sizeDelta.y / 2f, selectedSquare.anchoredPosition3D.z), fSpeed).SetEase(Ease.OutCubic);
        } else {
            if (!gameObject.activeSelf) return;

            actionMenu.SetActiveWithAnimation(false);

            isGoingEnabled = false;

            fightMenu.DOAnchorPos3D(new Vector3(fightMenu.anchoredPosition3D.x, -120f, fightMenu.anchoredPosition3D.z), fSpeed).SetEase(Ease.OutCubic);
            currentSquare.DOAnchorPos3D(new Vector3(currentSquare.anchoredPosition3D.x, -120f, currentSquare.anchoredPosition3D.z), fSpeed).SetEase(Ease.OutCubic);
            selectedSquare.DOAnchorPos3D(new Vector3(selectedSquare.anchoredPosition3D.x, -120f, selectedSquare.anchoredPosition3D.z), fSpeed).SetEase(Ease.OutCubic).OnComplete(DisableGameObject);
        }
    }

    private void DisableGameObject() {
        if (isGoingEnabled) return;

        gameObject.SetActive(false);
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

    public void UpdateSelectedSquare() {
        Text currentSquareText = currentSquare.GetComponentInChildren<Text>();

        if (currentFightBoardCharacter.value != null) {
            currentSquareText.text =
                currentFightBoardCharacter.value.character.characterName +
                "\n" + currentFightBoardCharacter.value.character.currentHp + "/" + currentFightBoardCharacter.value.character.maxHP + " HP";
        } else {
            currentSquareText.text = "No currently selected character";
        }
    }
}
