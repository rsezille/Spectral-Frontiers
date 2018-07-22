using DG.Tweening;
using SF;
using UnityEngine;
using UnityEngine.UI;

public class VictoryHUD : MonoBehaviour {
    public Text detailsText;

    public void SetActiveWithAnimation(bool active, HUD.Speed speed = HUD.Speed.Fast) {
        float fSpeed = (int)speed / 1000f;

        if (active) {
            detailsText.text = "Turns: " + BattleManager.instance.turn;

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
