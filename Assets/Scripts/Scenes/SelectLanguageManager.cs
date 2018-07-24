using SF;
using UnityEngine;

public class SelectLanguageManager : MonoBehaviour {
    private void Start() {
        #if UNITY_EDITOR
            PlayerPrefs.DeleteKey(OptionKeys.Language);
        #endif

        // Only show this scene once, at the first launch of the game
        if (PlayerPrefs.HasKey(OptionKeys.Language)) {
            LoadLanguageAndLeave(PlayerPrefs.GetString(OptionKeys.Language));
        }
    }

    public static void LoadLanguageAndLeave(string languageCode) {
        Debug.Log("Loading language... [" + languageCode + "]");
        LanguageManager.instance.loadLanguage(languageCode);
        Debug.Log("End of loading language... [" + languageCode + "]");

        PlayerPrefs.SetString(OptionKeys.Language, languageCode);

        GameManager.instance.LoadSceneAsync(Scenes.Battle);
    }
}
