using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FightHUD : MonoBehaviour {
    public GameObject moveButton;
    public GameObject waitButton;
    public GameObject statusButton;

    public RectTransform fightMenu;
    public RectTransform currentSquare;
    public RectTransform selectedSquare;

    private bool isGoingEnabled = false;

    public BattleManager battleManager;

    private void Awake() {
        battleManager = BattleManager.instance;
    }

    private void Start() {
        moveButton.AddListener(EventTriggerType.PointerClick, battleManager.fight.Move);
        waitButton.AddListener(EventTriggerType.PointerClick, battleManager.fight.Wait);
        statusButton.AddListener(EventTriggerType.PointerClick, battleManager.fight.Status);
    }

    // Compute all checks on buttons availability
    public void Refresh() {
        if (battleManager.GetSelectedBoardChar() == null) return;
        
        moveButton.GetComponent<Button>().interactable = battleManager.GetSelectedBoardChar().movable.movementTokens > 0;
        waitButton.GetComponent<Button>().interactable = true;
        statusButton.GetComponent<Button>().interactable = true;
    }

    public void SetActiveWithAnimation(bool active) {
        float speed = 0.6f;

        if (active) {
            isGoingEnabled = true;
            this.gameObject.SetActive(true);

            fightMenu.anchoredPosition3D = new Vector3(fightMenu.anchoredPosition3D.x, -120f, fightMenu.anchoredPosition3D.z);
            fightMenu.DOAnchorPos3D(new Vector3(fightMenu.anchoredPosition3D.x, fightMenu.sizeDelta.y / 2f, fightMenu.anchoredPosition3D.z), speed).SetEase(Ease.OutCubic);

            currentSquare.anchoredPosition3D = new Vector3(currentSquare.anchoredPosition3D.x, -120f, currentSquare.anchoredPosition3D.z);
            currentSquare.DOAnchorPos3D(new Vector3(currentSquare.anchoredPosition3D.x, currentSquare.sizeDelta.y / 2f, currentSquare.anchoredPosition3D.z), speed).SetEase(Ease.OutCubic);

            selectedSquare.anchoredPosition3D = new Vector3(selectedSquare.anchoredPosition3D.x, -120f, selectedSquare.anchoredPosition3D.z);
            selectedSquare.DOAnchorPos3D(new Vector3(selectedSquare.anchoredPosition3D.x, selectedSquare.sizeDelta.y / 2f, selectedSquare.anchoredPosition3D.z), speed).SetEase(Ease.OutCubic);
        } else {
            isGoingEnabled = false;

            fightMenu.DOAnchorPos3D(new Vector3(fightMenu.anchoredPosition3D.x, -120f, fightMenu.anchoredPosition3D.z), speed).SetEase(Ease.OutCubic);
            currentSquare.DOAnchorPos3D(new Vector3(currentSquare.anchoredPosition3D.x, -120f, currentSquare.anchoredPosition3D.z), speed).SetEase(Ease.OutCubic);
            selectedSquare.DOAnchorPos3D(new Vector3(selectedSquare.anchoredPosition3D.x, -120f, selectedSquare.anchoredPosition3D.z), speed).SetEase(Ease.OutCubic).OnComplete(DisableGameObject);
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
                BoardChar boardCharacter = square.boardEntity.GetComponent<BoardChar>();

                if (boardCharacter != null) {
                    selectedSquareText.text += "\nCharacter: " + boardCharacter.character.name + " (" + boardCharacter.character.GetCurrentHP() + "/" + boardCharacter.character.GetMaxHP() + ")";
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

        if (battleManager.GetSelectedBoardChar() != null) {
            currentSquareText.text =
                battleManager.GetSelectedBoardChar().character.name +
                "\n" + battleManager.GetSelectedBoardChar().character.GetCurrentHP() + "/" + battleManager.GetSelectedBoardChar().character.GetMaxHP() + " HP";
        } else {
            currentSquareText.text = "No currently selected character";
        }
    }
}
