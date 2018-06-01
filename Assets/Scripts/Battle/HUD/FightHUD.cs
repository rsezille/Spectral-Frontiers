using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FightHUD : MonoBehaviour {
    private BattleManager battleManager;

    public GameObject moveButton;
    public GameObject previousButton;
    public GameObject nextButton;
    public GameObject statusButton;

    public RectTransform fightMenu;
    public RectTransform currentSquare;
    public RectTransform selectedSquare;

    private bool isGoingEnabled = false;

    private void Awake() {
        battleManager = BattleManager.instance;
    }

    private void Start() {
        moveButton.AddListener(EventTriggerType.PointerClick, battleManager.fight.Move);
        previousButton.AddListener(EventTriggerType.PointerClick, battleManager.fight.Previous);
        nextButton.AddListener(EventTriggerType.PointerClick, battleManager.fight.Next);
        statusButton.AddListener(EventTriggerType.PointerClick, battleManager.fight.Status);
    }

    // Compute all checks on buttons availability
    public void Refresh() {
        if (battleManager.GetSelectedPlayerBoardCharacter() == null) return;

        Movable movable = battleManager.GetSelectedPlayerBoardCharacter().GetComponent<Movable>();
        moveButton.GetComponent<Button>().interactable = movable != null && movable.movementTokens > 0;

        previousButton.GetComponent<Button>().interactable = true; //TODO: if no other character available, disable it
        nextButton.GetComponent<Button>().interactable = true; //TODO: if no other character available, disable it
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
                PlayerCharacter playerCharacter = square.boardEntity.GetComponent<PlayerCharacter>();

                if (playerCharacter != null) {
                    selectedSquareText.text += "\nCharacter: " + playerCharacter.boardCharacter.character.name + " (" + playerCharacter.boardCharacter.character.GetCurrentHP() + "/" + playerCharacter.boardCharacter.character.GetMaxHP() + ")";
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

        if (battleManager.GetSelectedPlayerBoardCharacter() != null) {
            currentSquareText.text =
                battleManager.GetSelectedPlayerBoardCharacter().character.name +
                "\n" + battleManager.GetSelectedPlayerBoardCharacter().character.GetCurrentHP() + "/" + battleManager.GetSelectedPlayerBoardCharacter().character.GetMaxHP() + " HP";
        } else {
            currentSquareText.text = "No currently selected character";
        }
    }
}
