using UnityEngine;

/**
 * GameManager is the same accross all scenes and is instantiated by the loader.
 * Initialize missions, translations, monsters, etc.
 */
public class GameManager : MonoBehaviour {
    public static GameManager instance = null;

    // Game initialization
    void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        Debug.Log("Game Manager awakes");
    }
}
