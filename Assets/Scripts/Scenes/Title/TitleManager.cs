using UnityEngine;
using UnityEngine.EventSystems;

namespace SF {
    public class TitleManager : MonoBehaviour {
        public GameObject newButton;
        public GameObject loadButton;
        public GameObject quitButton;

        private void Start() {
            newButton.AddListener(EventTriggerType.PointerClick, NewGame);
            loadButton.AddListener(EventTriggerType.PointerClick, LoadGame);
            quitButton.AddListener(EventTriggerType.PointerClick, GameUtil.QuitGame);
        }

        private void NewGame() {
            GameManager.instance.LoadSceneAsync(Scenes.InGame);
        }

        private void LoadGame() { }
    }
}
