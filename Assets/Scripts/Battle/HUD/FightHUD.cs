using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FightHUD : MonoBehaviour {
    public GameObject moveButton;

    private void Start() {
        moveButton.AddListener(EventTriggerType.PointerClick, BattleManager.instance.fight.Move);
    }

    // Compute all checks on buttons availability
    public void Refresh() {
        BoardChar boardChar = BattleManager.instance.currentBoardChar;

        moveButton.GetComponent<Button>().interactable = boardChar.movable.movementTokens > 0;
    }
}
