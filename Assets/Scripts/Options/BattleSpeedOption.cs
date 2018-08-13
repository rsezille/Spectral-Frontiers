using SF;
using UnityEngine;
using UnityEngine.UI;

public class BattleSpeedOption : MonoBehaviour {
    public Button x1;
    public Button x2;
    public Button x4;

    private void Start() {
        x1.gameObject.AddListener(UnityEngine.EventSystems.EventTriggerType.PointerClick, () => {
            PlayerOptions.SetValue(PlayerOptions.BattleSpeed, 1f);
            CheckInteractable();
        });
        x2.gameObject.AddListener(UnityEngine.EventSystems.EventTriggerType.PointerClick, () => {
            PlayerOptions.SetValue(PlayerOptions.BattleSpeed, 2f);
            CheckInteractable();
        });
        x4.gameObject.AddListener(UnityEngine.EventSystems.EventTriggerType.PointerClick, () => {
            PlayerOptions.SetValue(PlayerOptions.BattleSpeed, 4f);
            CheckInteractable();
        });

        CheckInteractable();
    }

    private void CheckInteractable() {
        switch (Mathf.RoundToInt(PlayerOptions.GetFloat(PlayerOptions.BattleSpeed))) {
            case 1:
                x1.interactable = false;
                x2.interactable = true;
                x4.interactable = true;
                break;
            case 2:
                x1.interactable = true;
                x2.interactable = false;
                x4.interactable = true;
                break;
            case 4:
                x1.interactable = true;
                x2.interactable = true;
                x4.interactable = false;
                break;
            default:
                PlayerOptions.SetValue(PlayerOptions.BattleSpeed, 1f);
                x1.interactable = false;
                x2.interactable = true;
                x4.interactable = true;
                break;
        }
    }
}
