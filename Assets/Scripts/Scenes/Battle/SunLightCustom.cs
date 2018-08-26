using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SunLight))]
public class SunLightCustom : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        if (GUILayout.Button("Manually refresh lights (Play mode only)")) {
            if (EditorApplication.isPlaying) {
                BattleManager.instance.EventOnLightChange();
            }
        }
    }
}
