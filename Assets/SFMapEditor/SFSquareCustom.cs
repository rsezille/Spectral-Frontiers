using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SFSquare))]
public class SFSquareCustom : Editor {
    SFSquare square;

    private void OnEnable() {
        square = (SFSquare)target;
    }

    private void OnSceneGUI() {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.green;
        style.fontStyle = FontStyle.Bold;
        style.fontSize = 14;

        Vector3 position = square.transform.position;

        Handles.Label(position, "MapPos(" + square.x + "," + square.y + ")\nAltitude(" + square.altitude + ")", style);
    }
}
