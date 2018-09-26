using DG.Tweening;
using SF;
using TMPro;
using UnityEngine;

public class VictoryHUD : MonoBehaviour {
    [Header("Dependencies")]
    public BattleState battleState;

    [Header("Direct references")]
    public Canvas canvas;
    public TextMeshProUGUI detailsText;

    private void Awake() {
        canvas.gameObject.SetActive(false);
    }

    public void OnEnterBattleStepEvent(BattleState.BattleStep battleStep) {
        if (battleStep == BattleState.BattleStep.Victory) {
            SetActiveWithAnimation(true);
        }
    }

    public void OnLeaveBattleStepEvent(BattleState.BattleStep battleStep) {
        if (battleStep == BattleState.BattleStep.Victory) {
            SetActiveWithAnimation(false);
        }
    }

    public void Next() {
        battleState.currentBattleStep = BattleState.BattleStep.Cutscene;
    }

    public void SetActiveWithAnimation(bool active, HUD.Speed speed = HUD.Speed.Fast) {
        float fSpeed = (int)speed / 1000f;

        if (active) {
            detailsText.SetText("Pouet");

            canvas.gameObject.transform.localScale = Vector3.zero;
            canvas.gameObject.SetActive(true);

            canvas.gameObject.transform.DOScale(Vector3.one, fSpeed);
        } else {
            if (!canvas.gameObject.activeSelf) return;

            canvas.gameObject.transform.localScale = Vector3.one;

            canvas.gameObject.transform.DOScale(Vector3.zero, fSpeed).OnComplete(() => canvas.gameObject.SetActive(false));
        }
    }
}
