using SF;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * Manage the creation of the GameManager and setup default configuration for each scene when debugging (to be able to access any screen immediately)
 * Must be attached to every scene as the first GameObject
 */
public class Loader : MonoBehaviour {
    public GameObject gameManager;

    [Header("Dependencies")]
    public MissionVariable missionToLoad;
    public Mission testingMission;

    // Initialization
    private void Awake() {
        if (GameManager.instance == null) {
            Debug.Log("Game is starting...");
            Instantiate(gameManager);

            LanguageManager.instance.LoadDefaultLanguage();
            LanguageManager.instance.LoadLanguage(PlayerOptions.GetString(PlayerOptions.Language));

#if UNITY_EDITOR
            // Allow us to start with any scene instead of the first one
            switch (SceneManager.GetActiveScene().name) {
                case Scenes.Language:
                    // Do nothing, normal execution (language is the first scene)
                    Debug.Log("First scene: Language");
                    break;
                case Scenes.Title:
                    Debug.Log("First scene: Title");
                    break;
                case Scenes.InGame:
                    Debug.Log("First scene: InGame");
                    break;
                case Scenes.Battle:
                    Debug.Log("First scene: Battle");
                    missionToLoad.value = testingMission;
                    break;
            }
#endif
        }
    }
}
