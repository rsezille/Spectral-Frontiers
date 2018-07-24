using UnityEngine;
using UnityEngine.EventSystems;

public class PausedHUD : MonoBehaviour {
    public GameObject resumeButton;
    public GameObject quitButton;

    private void Start() {
        resumeButton.AddListener(EventTriggerType.PointerClick, Resume);
        quitButton.AddListener(EventTriggerType.PointerClick, Quit);
    }

    public void Resume() {
        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }

    private void Quit() {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
