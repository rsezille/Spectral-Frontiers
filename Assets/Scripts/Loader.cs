using UnityEngine;

/**
 * Manage the creation of the GameManager and setup default configuration for each scene when debugging (to be able to access any screen immediately)
 * Must be attached to every scene as the first GameObject
 */
public class Loader : MonoBehaviour {
    public GameObject gameManager;

    // Initialization
    void Awake() {
        if (GameManager.instance == null) {
            Debug.Log("Game is starting...");
            Instantiate(gameManager);
        }
    }
}
