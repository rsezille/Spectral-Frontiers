using System.Collections;
using UnityEngine;

namespace SF {
    public class CutsceneHUD : MonoBehaviour {
        [Header("Direct references")]
        public Canvas canvas;

        private void Awake() {
            canvas.gameObject.SetActive(false);
        }

        public void OnEnterBattleStepEvent(BattleState.BattleStep battleStep) {
            if (battleStep == BattleState.BattleStep.Cutscene) {
                StartCoroutine(WaitForShadeIn());
            }
        }

        public void OnLeaveBattleStepEvent(BattleState.BattleStep battleStep) {
            if (battleStep == BattleState.BattleStep.Cutscene) {
                canvas.gameObject.SetActive(false);
            }
        }

        private IEnumerator WaitForShadeIn() {
            yield return new WaitForSeconds(Globals.ShadeInOutCutsceneTime);

            canvas.gameObject.SetActive(true);
        }
    }
}
