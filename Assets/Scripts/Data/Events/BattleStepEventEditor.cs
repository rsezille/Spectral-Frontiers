using UnityEditor;
using UnityEngine;

namespace SF {
    /**
     * TODO: Add a dropdown to select the battle step to raise
     */
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
