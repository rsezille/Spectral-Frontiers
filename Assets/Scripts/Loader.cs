using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * Manage the creation of the GameManager and setup default configuration for each scene when debugging (to be able to access any screen immediately)
 * Must be attached to every scene as the first GameObject
 */
public class Loader : MonoBehaviour {
    public GameObject gameManager;

    // Initialization
    private void Awake() {
        if (GameManager.instance == null) {
            Debug.Log("Game is starting...");
            Instantiate(gameManager);

#if UNITY_EDITOR
            // Allow us to start with any scene instead of the first one
            switch (SceneManager.GetActiveScene().name) {
                case Scenes.Battle:
                    Debug.Log("First scene: Battle");
                    LanguageManager.getInstance().loadDefaultLanguage();
                    break;
            }
#endif
        }
    }
}
