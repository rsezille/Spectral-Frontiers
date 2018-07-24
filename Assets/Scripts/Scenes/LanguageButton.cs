using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LanguageButton : MonoBehaviour {
    public string languageCode = "en";

    private void Start() {
        GetComponent<Button>().gameObject.AddListener(EventTriggerType.PointerClick, () => SelectLanguageManager.LoadLanguageAndLeave(languageCode));
    }
}
