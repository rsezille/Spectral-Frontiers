using UnityEngine;

namespace SF {
    public class PausedHUD : MonoBehaviour {
        [Header("Direct references")]
        public Canvas canvas;

        private void Awake() {
            canvas.gameObject.SetActive(false);
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

        public void QuitGame() {
            GameUtil.QuitGame();
        }
    }
}
