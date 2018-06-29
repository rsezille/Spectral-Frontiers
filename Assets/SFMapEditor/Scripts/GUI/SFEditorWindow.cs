using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(SFMapEditor), typeof(SFSpritePicker))]
public class SFEditorWindow : MonoBehaviour {}

[CustomEditor(typeof(SFEditorWindow))]
public class SFEditorWindowCustom : Editor {
    private SFEditorWindow sfEditorWindow;

    private SFMapEditor sfMapEditor; // Shortcut

    private static GUIStyle normalButton;
    private static GUIStyle activeButton;

    private void OnEnable() {
        sfEditorWindow = (SFEditorWindow)target;
        sfMapEditor = sfEditorWindow.GetComponent<SFMapEditor>();
    }

    // Disable default inspector GUI
    public override void OnInspectorGUI() { }

    private void OnSceneGUI() {
        // Initialize button styles
        if (normalButton == null) {
            normalButton = new GUIStyle(GUI.skin.button);
            activeButton = new GUIStyle(normalButton);
            activeButton.normal.background = activeButton.active.background;
        }

        // Base + UseWaterToggle + DeleteToggle + FillEmptyBtn + Separator + UndoBtn + UndoStackCount
        int windowHeight = sfMapEditor.currentMode == SFMapEditor.Mode.Draw ? 155 + 20 + 20 + 20 + 20 + 20 : 155;
        
        GUI.Window(0, new Rect(20, 20, 150, windowHeight), EditorToolbox, "SFMapEditor");
    }

    private void EditorToolbox(int windowID) {
        GUI.Label(new Rect(5, 20, 60, 20), "Mode (R): ");
        GUI.Label(new Rect(65, 20, 70, 20), sfMapEditor.currentMode.ToString(), EditorStyles.boldLabel);

        if (GUI.Button(new Rect(5, 40, 40, 20), "Draw", sfMapEditor.currentMode == SFMapEditor.Mode.Draw ? activeButton : normalButton)) {
            sfMapEditor.currentMode = SFMapEditor.Mode.Draw;
        } else if (GUI.Button(new Rect(45, 40, 50, 20), "Select", sfMapEditor.currentMode == SFMapEditor.Mode.Selection ? activeButton : normalButton)) {
            sfMapEditor.currentMode = SFMapEditor.Mode.Selection;
        } else if (GUI.Button(new Rect(95, 40, 50, 20), "Height", sfMapEditor.currentMode == SFMapEditor.Mode.Height ? activeButton : normalButton)) {
            sfMapEditor.currentMode = SFMapEditor.Mode.Height;
        } else if (GUI.Button(new Rect(45, 60, 50, 20), "Delete", sfMapEditor.currentMode == SFMapEditor.Mode.Delete ? activeButton : normalButton)) {
            sfMapEditor.currentMode = SFMapEditor.Mode.Delete;
        } else if (GUI.Button(new Rect(95, 60, 50, 20), "Block", sfMapEditor.currentMode == SFMapEditor.Mode.Block ? activeButton : normalButton)) {
            sfMapEditor.currentMode = SFMapEditor.Mode.Block;
        }

        sfMapEditor.showGrid = GUI.Toggle(new Rect(5, 85, 110, 20), sfMapEditor.showGrid, "Show grid (G)");

        GUI.Label(new Rect(5, 105, 60, 20), "Selection: ");
        GUI.Label(new Rect(65, 105, 40, 20), sfMapEditor.currentSelectMode.ToString(), EditorStyles.boldLabel);

        if (GUI.Button(new Rect(5, 125, 40, 20), "Grid", sfMapEditor.currentSelectMode == SFMapEditor.SelectMode.Grid ? activeButton : normalButton)) {
            sfMapEditor.currentSelectMode = SFMapEditor.SelectMode.Grid;
        } else if (GUI.Button(new Rect(45, 125, 40, 20), "Tile", sfMapEditor.currentSelectMode == SFMapEditor.SelectMode.Tile ? activeButton : normalButton)) {
            sfMapEditor.currentSelectMode = SFMapEditor.SelectMode.Tile;
        }

        if (sfMapEditor.currentMode == SFMapEditor.Mode.Draw) {
            sfMapEditor.useWater = GUI.Toggle(new Rect(5, 150, 110, 20), sfMapEditor.useWater, "Use water (W)");

            if (GUI.Button(new Rect(5, 170, 70, 20), "Fill empty")) {
                sfMapEditor.FillEmpty();
            }

            GUI.Label(new Rect(5, 190, 150, 20), "---------------------------");

            if (GUI.Button(new Rect(50, 205, 50, 20), "Undo")) {
                if (sfMapEditor.undoStack.Count > 0) {
                    (sfMapEditor.undoStack.Pop())();
                }
            }

            GUI.Label(new Rect(5, 230, 100, 20), "Undo stack: " + sfMapEditor.undoStack.Count);
        }
    }
}
