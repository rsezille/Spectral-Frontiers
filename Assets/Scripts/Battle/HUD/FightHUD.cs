using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FightHUD : MonoBehaviour {
    public GameObject moveButton;

    private void Start() {
        moveButton.AddEventTriggerAndListener(EventTriggerType.PointerClick, BattleManager.instance.fight.Move);
    }
}
