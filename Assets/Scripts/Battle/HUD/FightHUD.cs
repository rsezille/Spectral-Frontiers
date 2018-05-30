using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FightHUD : MonoBehaviour {
    public GameObject moveButton;

    public RectTransform fightMenu;
    public RectTransform currentSquare;
    public RectTransform selectedSquare;

    private bool isGoingEnabled = false;

    private void Start() {
        moveButton.AddListener(EventTriggerType.PointerClick, BattleManager.instance.fight.Move);
    }

    // Compute all checks on buttons availability
    public void Refresh() {
        BoardChar boardChar = BattleManager.instance.currentBoardChar;

        moveButton.GetComponent<Button>().interactable = boardChar.movable.movementTokens > 0;
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

    void DisableGameObject() {
        if (isGoingEnabled) return;

        gameObject.SetActive(false);
    }
}
