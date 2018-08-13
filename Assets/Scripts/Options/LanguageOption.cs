using SF;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageOption : MonoBehaviour {
    public Dropdown languages;

    private void Awake() {
        languages.ClearOptions();

        List<string> languagesToAdd = new List<string>();

        foreach (string isoCode in LanguageList.availableLanguages) {
            languagesToAdd.Add(LanguageList.allLanguages[isoCode].universalName);
        }

        languages.AddOptions(languagesToAdd);

        languages.onValueChanged.AddListener(newValue => {
            if (newValue < LanguageList.availableLanguages.Count) {
                PlayerOptions.SetValue(PlayerOptions.Language, LanguageList.availableLanguages[newValue]);
                LanguageManager.instance.LoadLanguage(LanguageList.availableLanguages[newValue]);
            }
        });
    }

    private void Start() {
        languages.value = LanguageList.availableLanguages.IndexOf(PlayerOptions.GetString(PlayerOptions.Language));
    }
}
