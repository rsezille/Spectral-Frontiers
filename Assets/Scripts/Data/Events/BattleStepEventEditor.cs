using UnityEditor;
using UnityEngine;

namespace SF {
    [CustomEditor(typeof(BattleStepEvent))]
    public class BattleStepEventEditor : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            GUI.enabled = Application.isPlaying;

            BattleStepEvent e = target as BattleStepEvent;

            if (GUILayout.Button("Raise")) {
                e.Raise(BattleState.BattleStep.Cutscene);
            }
        }
    }
}
