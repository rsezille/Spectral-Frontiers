using DG.Tweening;
using SF;
using TMPro;
using UnityEngine;

public class VictoryHUD : MonoBehaviour {
    [Header("Dependencies")]
    public BattleState battleState;

    [Header("Direct references")]
    public TextMeshProUGUI detailsText;
    public GameObject nextButton;

    private void Start() {
        nextButton.AddListener(UnityEngine.EventSystems.EventTriggerType.PointerClick, Next);
    }

    private void Next() {
        battleState.currentBattleStep = BattleState.BattleStep.Cutscene;
    }

    public void SetActiveWithAnimation(bool active, HUD.Speed speed = HUD.Speed.Fast) {
        float fSpeed = (int)speed / 1000f;

        if (active) {
            detailsText.SetText("Pouet");

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
