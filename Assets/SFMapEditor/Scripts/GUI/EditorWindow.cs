using UnityEditor;
using UnityEngine;

namespace SF {
    [RequireComponent(typeof(MapEditor), typeof(SpritePicker))]
    public class EditorWindow : MonoBehaviour { }

    [CustomEditor(typeof(EditorWindow))]
    public class EditorWindowCustom : Editor {
        private EditorWindow editorWindow;

        private MapEditor mapEditor; // Shortcut

        private static GUIStyle normalButton;
        private static GUIStyle activeButton;

        private void OnEnable() {
            editorWindow = (EditorWindow)target;
            mapEditor = editorWindow.GetComponent<MapEditor>();
        }

        // Disable default inspector GUI
        public override void OnInspectorGUI() {
            Event e = Event.current;
            serializedObject.Update();

            if (GUILayout.Button("Toggle editor settings", GUILayout.Width(180))) {
                mapEditor.editorToolboxEnabled = !mapEditor.editorToolboxEnabled;
            }

            if (GUILayout.Button("Toggle lighting settings", GUILayout.Width(180))) {
                mapEditor.lightingToolboxEnabled = !mapEditor.lightingToolboxEnabled;
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void OnSceneGUI() {
            // Initialize button styles
            if (normalButton == null) {
                normalButton = new GUIStyle(GUI.skin.button);
                activeButton = new GUIStyle(normalButton);
                activeButton.normal.background = activeButton.active.background;
            }

            // Base + UseWaterToggle + DeleteToggle + FillEmptyBtn + Separator + UndoBtn + UndoStackCount
            int windowHeight = mapEditor.currentMode == MapEditor.Mode.Draw ? 155 + 20 + 20 + 20 + 20 + 20 : 155;

            if (mapEditor.editorToolboxEnabled) {
                GUI.Window(0, new Rect(5, 20, 150, windowHeight), EditorToolbox, "SFMapEditor");
            }

            if (mapEditor.lightingToolboxEnabled) {
                GUI.Window(1, new Rect(Screen.width - 160, 20, 150, 100), LightingToolbox, "Lighting");
            }
        }

        private void LightingToolbox(int windowID) {
            GUI.Label(new Rect(5, 20, 60, 20), "Pouet");
        }

        private void EditorToolbox(int windowID) {
            GUI.Label(new Rect(5, 20, 60, 20), "Mode (R): ");
            GUI.Label(new Rect(65, 20, 70, 20), mapEditor.currentMode.ToString(), EditorStyles.boldLabel);

            if (GUI.Button(new Rect(5, 40, 40, 20), "Draw", mapEditor.currentMode == MapEditor.Mode.Draw ? activeButton : normalButton)) {
                mapEditor.currentMode = MapEditor.Mode.Draw;
            } else if (GUI.Button(new Rect(45, 40, 50, 20), "Select", mapEditor.currentMode == MapEditor.Mode.Selection ? activeButton : normalButton)) {
                mapEditor.currentMode = MapEditor.Mode.Selection;
            } else if (GUI.Button(new Rect(95, 40, 50, 20), "Height", mapEditor.currentMode == MapEditor.Mode.Height ? activeButton : normalButton)) {
                mapEditor.currentMode = MapEditor.Mode.Height;
            } else if (GUI.Button(new Rect(45, 60, 50, 20), "Delete", mapEditor.currentMode == MapEditor.Mode.Delete ? activeButton : normalButton)) {
                mapEditor.currentMode = MapEditor.Mode.Delete;
            } else if (GUI.Button(new Rect(95, 60, 50, 20), "Block", mapEditor.currentMode == MapEditor.Mode.Block ? activeButton : normalButton)) {
                mapEditor.currentMode = MapEditor.Mode.Block;
            }

            mapEditor.showGrid = GUI.Toggle(new Rect(5, 85, 110, 20), mapEditor.showGrid, "Show grid (G)");

            GUI.Label(new Rect(5, 105, 60, 20), "Selection: ");
            GUI.Label(new Rect(65, 105, 40, 20), mapEditor.currentSelectMode.ToString(), EditorStyles.boldLabel);

            if (GUI.Button(new Rect(5, 125, 40, 20), "Grid", mapEditor.currentSelectMode == MapEditor.SelectMode.Grid ? activeButton : normalButton)) {
                mapEditor.currentSelectMode = MapEditor.SelectMode.Grid;
            } else if (GUI.Button(new Rect(45, 125, 40, 20), "Tile", mapEditor.currentSelectMode == MapEditor.SelectMode.Tile ? activeButton : normalButton)) {
                mapEditor.currentSelectMode = MapEditor.SelectMode.Tile;
            }

            if (mapEditor.currentMode == MapEditor.Mode.Draw) {
                mapEditor.useWater = GUI.Toggle(new Rect(5, 150, 110, 20), mapEditor.useWater, "Use water (W)");

                if (GUI.Button(new Rect(5, 170, 70, 20), "Fill empty")) {
                    mapEditor.FillEmpty();
                }

                GUI.Label(new Rect(5, 190, 150, 20), "---------------------------");

                if (GUI.Button(new Rect(50, 205, 50, 20), "Undo")) {
                    if (mapEditor.undoStack.Count > 0) {
                        (mapEditor.undoStack.Pop())();
                    }
                }

                GUI.Label(new Rect(5, 230, 100, 20), "Undo stack: " + mapEditor.undoStack.Count);
            }
        }
    }
}
