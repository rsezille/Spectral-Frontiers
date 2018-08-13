using SF;
using UnityEngine;
using UnityEngine.UI;

public class TextSpeedOption : MonoBehaviour {
    public Button slow;
    public Button mid;
    public Button fast;
    public Button instant;

    private void Start() {
        slow.gameObject.AddListener(UnityEngine.EventSystems.EventTriggerType.PointerClick, () => {
            PlayerOptions.SetValue(PlayerOptions.TextSpeed, DialogBox.TextSpeed.Slow.ToString());
            GameManager.instance.DialogBox.textSpeed = DialogBox.TextSpeed.Slow;
            CheckInteractable();
        });
        mid.gameObject.AddListener(UnityEngine.EventSystems.EventTriggerType.PointerClick, () => {
            PlayerOptions.SetValue(PlayerOptions.TextSpeed, DialogBox.TextSpeed.Fast.ToString());
            GameManager.instance.DialogBox.textSpeed = DialogBox.TextSpeed.Fast;
            CheckInteractable();
        });
        fast.gameObject.AddListener(UnityEngine.EventSystems.EventTriggerType.PointerClick, () => {
            PlayerOptions.SetValue(PlayerOptions.TextSpeed, DialogBox.TextSpeed.VeryFast.ToString());
            GameManager.instance.DialogBox.textSpeed = DialogBox.TextSpeed.VeryFast;
            CheckInteractable();
        });
        instant.gameObject.AddListener(UnityEngine.EventSystems.EventTriggerType.PointerClick, () => {
            PlayerOptions.SetValue(PlayerOptions.TextSpeed, DialogBox.TextSpeed.Instant.ToString());
            GameManager.instance.DialogBox.textSpeed = DialogBox.TextSpeed.Instant;
            CheckInteractable();
        });

        CheckInteractable();
    }

    private void CheckInteractable() {
        switch (EnumUtil.ParseEnum(PlayerOptions.GetString(PlayerOptions.TextSpeed), DialogBox.TextSpeed.Fast)) {
            case DialogBox.TextSpeed.Slow:
                slow.interactable = false;
                mid.interactable = true;
                fast.interactable = true;
                instant.interactable = true;
                break;
            case DialogBox.TextSpeed.Fast:
                slow.interactable = true;
                mid.interactable = false;
                fast.interactable = true;
                instant.interactable = true;
                break;
            case DialogBox.TextSpeed.VeryFast:
                slow.interactable = true;
                mid.interactable = true;
                fast.interactable = false;
                instant.interactable = true;
                break;
            case DialogBox.TextSpeed.Instant:
                slow.interactable = true;
                mid.interactable = true;
                fast.interactable = true;
                instant.interactable = false;
                break;
            default:
                PlayerOptions.SetValue(PlayerOptions.TextSpeed, DialogBox.TextSpeed.Fast.ToString());
                slow.interactable = true;
                mid.interactable = false;
                fast.interactable = true;
                instant.interactable = true;
                break;
        }
    }
}
