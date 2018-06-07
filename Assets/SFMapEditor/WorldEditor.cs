using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[CustomEditor(typeof(World))]
public class WorldEditor : Editor {
    World world;

    private bool clickToUp = false;

    private void OnEnable() {
        world = (World)target;
    }

    private void EditorToolbox(int windowID) {
        world.showGrid = GUI.Toggle(new Rect(10, 20, 100, 20), world.showGrid, "Toggle grid");
        clickToUp = GUI.Toggle(new Rect(10, 40, 100, 20), clickToUp, "ClickToUp");

        GUI.DragWindow(new Rect(0, 0, 10000, 20));
    }
    
    private void OnSceneGUI() {
        Event e = Event.current;

        Handles.BeginGUI();
        GUI.Window(0, new Rect(20, 20, 120, 80), EditorToolbox, "SFMapBuilder");
        

        Handles.EndGUI();

        if (world.drawMode) {
            if (e.isMouse && e.type == EventType.MouseDown && e.button == 0) {
                e.Use();

                Vector3 mousePosition = new Vector3(e.mousePosition.x, -e.mousePosition.y + Camera.current.pixelHeight);
                Vector3 localMousePos = Camera.current.ScreenToWorldPoint(mousePosition);

                // Square coords
                int Sx = Mathf.FloorToInt((localMousePos.x / 2) + localMousePos.y);
                int Sy = Mathf.FloorToInt(localMousePos.y - (localMousePos.x / 2));

                if (Sx < 0 || Sx >= world.size.x || Sy < 0 || Sy >= world.size.y) return;

                GameObject square = GameObject.Find("Square(" + Sx + "," + Sy + ")");

                if (clickToUp) {
                    if (square) {
                        SpriteRenderer highestTile = null;

                        SpriteRenderer[] sprites = square.GetComponentsInChildren<SpriteRenderer>();

                        foreach (SpriteRenderer sprite in sprites) {
                            if (highestTile == null || sprite.sortingOrder > highestTile.sortingOrder) {
                                highestTile = sprite;
                            }
                        }

                        if (highestTile != null) {
                            highestTile.transform.Translate(new Vector3(0f, 5f / Globals.TileHeight));
                        }
                    }

                    return;
                }

                // Center of the square
                float Cx = Sx - Sy;
                float Cy = (Sx + Sy + 1f) / 2f;

                GameObject tile = world.useTileOne ? world.tile : world.tileTwo;

                int highestSortingOrder = 0;

                if (!square) {
                    square = new GameObject("Square(" + Sx + "," + Sy + ")");
                    SortingGroup sortingGroup = square.AddComponent<SortingGroup>();
                    sortingGroup.sortingOrder = -(world.size.x * Sy + Sx);
                    square.transform.SetParent(world.map.transform);
                } else {
                    SpriteRenderer[] sprites = square.GetComponentsInChildren<SpriteRenderer>();

                    

                    foreach (SpriteRenderer sprite in sprites) {
                        if (sprite.sortingOrder > highestSortingOrder) {
                            highestSortingOrder = sprite.sortingOrder;
                        }
                    }

                    highestSortingOrder++;
                }

                GameObject go = Instantiate(tile, new Vector3(Cx, Cy, 0f), Quaternion.identity);
                go.name = "Tile";
                go.transform.SetParent(square.transform);
                go.GetComponent<SpriteRenderer>().sortingOrder = highestSortingOrder;

            }

            Selection.activeGameObject = world.gameObject;
        }
    }
}
