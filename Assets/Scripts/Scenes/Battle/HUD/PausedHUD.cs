using SF;
using UnityEngine;
using UnityEngine.EventSystems;

public class PausedHUD : MonoBehaviour {
    [Header("Direct references")]
    public Canvas canvas;
    public GameObject optionsButton;
    public GameObject quitButton;

    private void Awake() {
        canvas.gameObject.SetActive(false);
    }

    private void Start() {
        optionsButton.AddListener(EventTriggerType.PointerClick, GameManager.instance.options.Show);
        quitButton.AddListener(EventTriggerType.PointerClick, GameManager.instance.QuitGame);
    }

    private void Update() {
        if (InputManager.Pause.IsKeyDown) {
            if (canvas.gameObject.activeSelf) {
                Resume();
            } else {
                canvas.gameObject.SetActive(true);
                Time.timeScale = 0;
            }
        }
    }

    public void Resume() {
        Time.timeScale = PlayerOptions.GetFloat(PlayerOptions.BattleSpeed);
        canvas.gameObject.SetActive(false);
    }
}
