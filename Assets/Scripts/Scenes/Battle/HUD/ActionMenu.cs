using DG.Tweening;
using SF;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionMenu : MonoBehaviour {
    private FightHUD fightHUD;

    [Header("Dependencies")]
    public BattleState battleState;
    public BoardCharacterVariable currentFightBoardCharacter;

    [Header("Direct references")]
    public GameObject attackButton;
    public GameObject skillButton;
    public GameObject itemButton;

    private void Awake() {
        fightHUD = GetComponentInParent<FightHUD>();
        gameObject.SetActive(false);
    }

    private void Start() {
        attackButton.AddListener(EventTriggerType.PointerClick, Attack);
    }

    private void Attack() {
        if (battleState.currentTurnStep == BattleState.TurnStep.Attack) {
            battleState.currentTurnStep = BattleState.TurnStep.None;
        } else {
            fightHUD.actionMenu.SetActiveWithAnimation(false);

            if (currentFightBoardCharacter.value.CanDoAction()) {
                battleState.currentTurnStep = BattleState.TurnStep.Attack;
            }
        }
    }

    // Compute all checks on buttons availability
    public void Refresh() {
        attackButton.GetComponent<Button>().interactable = true;
        skillButton.GetComponent<Button>().interactable = false; // TODO [ALPHA] if !Silenced etc.
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
