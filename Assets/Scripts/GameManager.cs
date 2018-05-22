using UnityEngine;

/**
 * GameManager is the same accross all scenes and is instantiated by the loader.
 * Initialize missions, translations, monsters, etc.
 */
public class GameManager : MonoBehaviour {
    public static GameManager instance = null;

    public Player player;

    // Game initialization
    void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }

        //TODO: do it when starting a new game (don't when loading a previously saved game)
        if (player == null) {
            player = new Player();
        }

        DontDestroyOnLoad(gameObject);

        Debug.Log("Game Manager awakes");
    }
}
