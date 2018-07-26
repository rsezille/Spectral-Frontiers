using SF;
using UnityEngine;

public class SelectLanguageManager : MonoBehaviour {
    private void Start() {
        #if UNITY_EDITOR
            PlayerPrefs.DeleteKey(PlayerOptions.Language.key);
        #endif

        // Only show this scene once, at the first launch of the game
        if (PlayerOptions.HasKey(PlayerOptions.Language)) {
            LoadLanguageAndLeave(PlayerOptions.GetString(PlayerOptions.Language));
        }
    }

    public static void LoadLanguageAndLeave(string languageCode) {
        Debug.Log("Loading language... [" + languageCode + "]");
        LanguageManager.instance.LoadLanguage(languageCode);
        Debug.Log("End of loading language... [" + languageCode + "]");

        PlayerOptions.SetValue(PlayerOptions.Language, languageCode);
        PlayerOptions.Save();

        GameManager.instance.LoadSceneAsync(Scenes.Title);
    }
}
