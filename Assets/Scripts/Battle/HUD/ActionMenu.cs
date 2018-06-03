using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionMenu : MonoBehaviour {
    private BattleManager battleManager;

    public GameObject attackButton;
    public GameObject skillButton;
    public GameObject itemButton;

    private void Awake() {
        battleManager = BattleManager.instance;

        gameObject.SetActive(false);
    }

    private void Start() {
        attackButton.AddListener(EventTriggerType.PointerClick, battleManager.fight.Attack);
    }

    // Compute all checks on buttons availability
    public void Refresh() {
        attackButton.GetComponent<Button>().interactable = true;
        skillButton.GetComponent<Button>().interactable = false; // TODO [STATUS] if !Silenced etc.
        itemButton.GetComponent<Button>().interactable = false;
    }

    public void Toggle() {
        SetActiveWithAnimation(!gameObject.activeSelf);
    }

    public void SetActiveWithAnimation(bool active) {
        if (active) {
            gameObject.transform.localScale = Vector3.zero;
            gameObject.SetActive(true);

            gameObject.transform.DOScale(Vector3.one, 0.3f);
        } else {
            if (!gameObject.activeSelf) return;

            gameObject.transform.localScale = Vector3.one;

            gameObject.transform.DOScale(Vector3.zero, 0.3f).OnComplete(() => gameObject.SetActive(false));
        }
    }
}
