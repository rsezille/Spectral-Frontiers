using UnityEditor;
using UnityEngine;

public class SFMapGizmos : MonoBehaviour {
    private SFMapEditor sfMapEditor;

    private void OnDrawGizmos() {
        if (sfMapEditor == null) sfMapEditor = GetComponent<SFMapEditor>();

        if (sfMapEditor.showGrid && sfMapEditor.currentMode != SFMapEditor.Mode.Block) {
            Gizmos.color = sfMapEditor.gridColor;

            for (int x = 0; x <= sfMapEditor.size.x; x++) {
                Gizmos.DrawLine(new Vector3(x, x / 2f), new Vector3(-sfMapEditor.size.y + x, (sfMapEditor.size.y + x) / 2f));
            }

            for (int y = 0; y <= sfMapEditor.size.y; y++) {
                Gizmos.DrawLine(new Vector3(-y, y / 2f), new Vector3(sfMapEditor.size.x - y, (sfMapEditor.size.x + y) / 2f));
            }
        }

        if (sfMapEditor.currentMode == SFMapEditor.Mode.Block) {
            Square[] squares = sfMapEditor.map.GetComponentsInChildren<Square>();

            foreach (Square square in squares) {
                Gizmos.color = square.solid ? sfMapEditor.orange : square.IsNotBlocking() ? Color.green : Color.red;

                Vector3 bottom = new Vector3(square.x - square.y, (square.x + square.y) / 2f);
                Vector3 right = new Vector3(square.x - square.y + 1f, (square.x + square.y + 1f) / 2f);
                Vector3 left = new Vector3(square.x - square.y - 1f, (square.x + square.y + 1f) / 2f);
                Vector3 top = new Vector3(square.x - square.y, (square.x + square.y) / 2f + 1f);

                Gizmos.DrawLine(bottom + new Vector3(0f, 0.01f), right + new Vector3(-0.02f, 0f));
                Gizmos.DrawLine(left + new Vector3(0.02f, 0f), top + new Vector3(0f, -0.01f));
                Gizmos.DrawLine(bottom + new Vector3(0f, 0.01f), left + new Vector3(0.02f, 0f));
                Gizmos.DrawLine(right + new Vector3(-0.02f, 0f), top + new Vector3(0f, -0.01f));

                // Simulate thicker lines by drawing inner lines
                Gizmos.DrawLine(bottom + new Vector3(0f, 0.025f), right + new Vector3(-0.05f, 0f));
                Gizmos.DrawLine(left + new Vector3(0.05f, 0f), top + new Vector3(0f, -0.025f));
                Gizmos.DrawLine(bottom + new Vector3(0f, 0.025f), left + new Vector3(0.05f, 0f));
                Gizmos.DrawLine(right + new Vector3(-0.05f, 0f), top + new Vector3(0f, -0.025f));

                if (square.solid || !square.IsNotBlocking()) {
                    Gizmos.DrawLine(bottom + new Vector3(0f, 0.2f), top + new Vector3(0f, -0.2f));
                    Gizmos.DrawLine(left + new Vector3(0.4f, 0f), right + new Vector3(-0.4f, 0f));
                } else {
                    Gizmos.DrawWireSphere(new Vector3(square.x - square.y, (square.x + square.y + 1f) / 2f), 0.2f);
                }
            }
        }

        if (sfMapEditor.hoveredSquare != null) {
            Color hoveredColor = Color.black; //new Color(0f, 0.7f, 1f);
            Gizmos.color = hoveredColor;

            Vector3 bottom = new Vector3(sfMapEditor.hoveredSquare.x - sfMapEditor.hoveredSquare.y, (sfMapEditor.hoveredSquare.x + sfMapEditor.hoveredSquare.y) / 2f);
            Vector3 right = new Vector3(sfMapEditor.hoveredSquare.x - sfMapEditor.hoveredSquare.y + 1f, (sfMapEditor.hoveredSquare.x + sfMapEditor.hoveredSquare.y + 1f) / 2f);
            Vector3 left = new Vector3(sfMapEditor.hoveredSquare.x - sfMapEditor.hoveredSquare.y - 1f, (sfMapEditor.hoveredSquare.x + sfMapEditor.hoveredSquare.y + 1f) / 2f);
            Vector3 top = new Vector3(sfMapEditor.hoveredSquare.x - sfMapEditor.hoveredSquare.y, (sfMapEditor.hoveredSquare.x + sfMapEditor.hoveredSquare.y) / 2f + 1f);

            Gizmos.DrawLine(bottom + new Vector3(0f, 0.01f), right + new Vector3(-0.02f, 0f));
            Gizmos.DrawLine(left + new Vector3(0.02f, 0f), top + new Vector3(0f, -0.01f));
            Gizmos.DrawLine(bottom + new Vector3(0f, 0.01f), left + new Vector3(0.02f, 0f));
            Gizmos.DrawLine(right + new Vector3(-0.02f, 0f), top + new Vector3(0f, -0.01f));

            // Simulate thicker lines by drawing inner lines
            Gizmos.DrawLine(bottom + new Vector3(0f, 0.025f), right + new Vector3(-0.05f, 0f));
            Gizmos.DrawLine(left + new Vector3(0.05f, 0f), top + new Vector3(0f, -0.025f));
            Gizmos.DrawLine(bottom + new Vector3(0f, 0.025f), left + new Vector3(0.05f, 0f));
            Gizmos.DrawLine(right + new Vector3(-0.05f, 0f), top + new Vector3(0f, -0.025f));

            GUIStyle style = new GUIStyle();
            style.normal.textColor = hoveredColor;
            style.fontStyle = FontStyle.Bold;
            style.fontSize = 11;

            Handles.Label(sfMapEditor.hoveredSquare.transform.position - new Vector3(0.5f, -0.2f), "(" + sfMapEditor.hoveredSquare.x + "," + sfMapEditor.hoveredSquare.y + "," + sfMapEditor.hoveredSquare.Height + ")", style);
        }
    }
}
