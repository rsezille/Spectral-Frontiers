using DG.Tweening;
using SF;
using TMPro;
using UnityEngine;

public class VictoryHUD : MonoBehaviour {
    public TextMeshProUGUI detailsText;
    public GameObject nextButton;

    private void Start() {
        nextButton.AddListener(UnityEngine.EventSystems.EventTriggerType.PointerClick, BattleManager.instance.victory.Next);
    }

    public void SetActiveWithAnimation(bool active, HUD.Speed speed = HUD.Speed.Fast) {
        float fSpeed = (int)speed / 1000f;

        if (active) {
            detailsText.SetText("Turns: " + BattleManager.instance.turn);

            gameObject.transform.localScale = Vector3.zero;
            gameObject.SetActive(true);

            gameObject.transform.DOScale(Vector3.one, fSpeed);
        } else {
            if (!gameObject.activeSelf) return;

            gameObject.transform.localScale = Vector3.one;

            gameObject.transform.DOScale(Vector3.zero, fSpeed).OnComplete(() => gameObject.SetActive(false));
        }
    }
}
