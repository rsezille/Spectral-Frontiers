using UnityEngine;
using UnityEngine.EventSystems;

public class PausedHUD : MonoBehaviour {
    public GameObject resumeButton;
    public GameObject optionsButton;
    public GameObject quitButton;

    private void Start() {
        resumeButton.AddListener(EventTriggerType.PointerClick, Resume);
        optionsButton.AddListener(EventTriggerType.PointerClick, GameManager.instance.options.Show);
        quitButton.AddListener(EventTriggerType.PointerClick, GameManager.instance.QuitGame);
    }

    public void Resume() {
        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }
}
