using SF;
using UnityEngine;

public class SelectLanguageManager : MonoBehaviour {
    private void Start() {
        #if UNITY_EDITOR
            PlayerPrefs.DeleteAll();
        #endif

        // Only show this scene once, at the first launch of the game
        if (PlayerOptions.HasKey(PlayerOptions.Language)) {
            LoadLanguageAndLeave(PlayerOptions.GetString(PlayerOptions.Language));
        }
    }

    public static void LoadLanguageAndLeave(string languageCode) {
        PlayerOptions.SetValue(PlayerOptions.Language, languageCode);
        PlayerOptions.ResetKeys();
        PlayerOptions.Save();

        Debug.Log("Loading language... [" + languageCode + "]");
        LanguageManager.instance.LoadLanguage(languageCode);
        Debug.Log("End of loading language... [" + languageCode + "]");

        GameManager.instance.LoadSceneAsync(Scenes.Title);
    }
}
