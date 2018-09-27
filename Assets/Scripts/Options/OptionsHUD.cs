using SF;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionsHUD : MonoBehaviour {
    [Header("Direct references")]
    public Canvas canvas;
    public RectTransform keyBindingPrefab;
    public GameObject closeButton;
    public GameObject resetKeysButton;
    public RectTransform keyBindingsTransform;
    public RectTransform contentTransform;
    public GameObject waitingForKey;

    private Dictionary<string, TextMeshProUGUI> keyBindings;
    private KeyBind waitedKey;

    private void Awake() {
        canvas.gameObject.SetActive(false);

        keyBindings = new Dictionary<string, TextMeshProUGUI>();
        waitingForKey.SetActive(false);
    }

    private void Start() {
        closeButton.AddListener(EventTriggerType.PointerClick, Close);
        resetKeysButton.AddListener(EventTriggerType.PointerClick, ResetKeys);

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

    private void OnGUI() {
        if (Event.current.isKey && waitingForKey.activeSelf && waitedKey != null) {
            waitedKey.Bind(Event.current.keyCode);

            if (keyBindings.ContainsKey(waitedKey.name)) {
                keyBindings[waitedKey.name].SetText(waitedKey.bindedKey.ToString());
            }

            waitingForKey.SetActive(false);
        }
    }

    public void Show() {
        canvas.gameObject.SetActive(true);
        InputManager.Pause.enabled = false;
    }

    public void Close() {
        if (canvas.gameObject.activeSelf) {
            PlayerOptions.Save();
            canvas.gameObject.SetActive(false);
            InputManager.Pause.enabled = true;
        }
    }

    private void ResetKeys() {
        foreach (KeyBind key in InputManager.allKeyBinds) {
            key.ResetKey();

            if (keyBindings.ContainsKey(key.name)) {
                keyBindings[key.name].SetText(key.bindedKey.ToString());
            }
        }
    }

    private void AddKeyBinding(KeyBind key) {
        RectTransform instance = Instantiate(keyBindingPrefab, keyBindingsTransform) as RectTransform;
        instance.localPosition += new Vector3(0f, -50 * keyBindings.Count);

        instance.GetComponentInChildren<TextMeshProUGUI>().SetText(LanguageManager.instance.GetString(key.name));

        Button keyButton = instance.GetComponentInChildren<Button>();
        keyButton.gameObject.AddListener(EventTriggerType.PointerClick, () => WaitForKey(key));
        TextMeshProUGUI keyText = keyButton.GetComponentInChildren<TextMeshProUGUI>();
        keyText.SetText(key.bindedKey.ToString());

        keyBindings.Add(key.name, keyText);

        contentTransform.sizeDelta += new Vector2(0f, instance.sizeDelta.y);
    }

    private void WaitForKey(KeyBind keyBind) {
        waitedKey = keyBind;
        waitingForKey.SetActive(true);
    }
}
