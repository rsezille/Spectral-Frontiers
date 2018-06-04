using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class VictoryHUD : MonoBehaviour {
    public Text detailsText;

    public void SetActiveWithAnimation(bool active) {
        float speed = 0.3f;

        if (active) {
            detailsText.text = "Turns: " + BattleManager.instance.turn;

            gameObject.transform.localScale = Vector3.zero;
            gameObject.SetActive(true);

            gameObject.transform.DOScale(Vector3.one, speed);
        } else {
            if (!gameObject.activeSelf) return;

            gameObject.transform.localScale = Vector3.one;

            gameObject.transform.DOScale(Vector3.zero, speed).OnComplete(() => gameObject.SetActive(false));
        }
    }
}
