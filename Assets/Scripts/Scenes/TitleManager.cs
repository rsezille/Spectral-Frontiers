using UnityEngine;
using UnityEngine.EventSystems;

public class TitleManager : MonoBehaviour {
    public GameObject newButton;
    public GameObject loadButton;
    public GameObject optionsButton;
    public GameObject quitButton;

    private void Start() {
        newButton.AddListener(EventTriggerType.PointerClick, NewGame);
        loadButton.AddListener(EventTriggerType.PointerClick, LoadGame);
        optionsButton.AddListener(EventTriggerType.PointerClick, GameManager.instance.options.Show);
        quitButton.AddListener(EventTriggerType.PointerClick, GameManager.instance.QuitGame);
    }

    private void NewGame() {
        GameManager.instance.LoadSceneAsync(Scenes.InGame);
    }

    private void LoadGame() { }
}
