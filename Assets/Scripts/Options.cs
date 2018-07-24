using SF;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Options : MonoBehaviour {
    public RectTransform keyBindingPrefab;
    public GameObject closeButton;
    public RectTransform keyBindingsTransform;
    public RectTransform contentTransform;

    private List<string> keyBindings;

    private void Awake() {
        keyBindings = new List<string>();
    }

    private void Start() {
        closeButton.AddListener(EventTriggerType.PointerClick, Close);

        contentTransform.sizeDelta = new Vector2(0f, -keyBindingsTransform.localPosition.y);

        AddKeyBinding(InputManager.CameraUp);
        AddKeyBinding(InputManager.CameraLeft);
        AddKeyBinding(InputManager.CameraDown);
        AddKeyBinding(InputManager.CameraRight);
        AddKeyBinding(InputManager.Up);
        AddKeyBinding(InputManager.Left);
        AddKeyBinding(InputManager.Down);
        AddKeyBinding(InputManager.Right);
        AddKeyBinding(InputManager.Previous);
        AddKeyBinding(InputManager.Next);
        AddKeyBinding(InputManager.Confirm);
        AddKeyBinding(InputManager.Pause);
        AddKeyBinding(InputManager.Special1);
    }

    public void Show() {
        if (!gameObject.activeSelf) {
            gameObject.SetActive(true);
        }
    }

    public void Close() {
        if (gameObject.activeSelf) {
            gameObject.SetActive(false);
        }
    }

    private void AddKeyBinding(KeyBind key) {
        RectTransform instance = Instantiate(keyBindingPrefab, keyBindingsTransform) as RectTransform;
        instance.localPosition += new Vector3(0f, -50 * keyBindings.Count);

        instance.GetComponentInChildren<TextMeshProUGUI>().SetText(LanguageManager.instance.GetString(key.name));
        instance.GetComponentInChildren<Button>().GetComponentInChildren<TextMeshProUGUI>().SetText(key.bindedKey.ToString());

        keyBindings.Add(key.bindedKey.ToString());

        contentTransform.sizeDelta += new Vector2(0f, instance.sizeDelta.y);
    }
}
