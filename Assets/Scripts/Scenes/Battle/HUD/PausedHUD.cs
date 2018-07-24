using UnityEngine;
using UnityEngine.EventSystems;

public class PausedHUD : MonoBehaviour {
    public GameObject resumeButton;
    public GameObject quitButton;

    private void Start() {
        resumeButton.AddListener(EventTriggerType.PointerClick, Resume);
        quitButton.AddListener(EventTriggerType.PointerClick, GameManager.instance.QuitGame);
    }

    public void Resume() {
        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }
}
