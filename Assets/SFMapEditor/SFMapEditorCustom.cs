using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[CustomEditor(typeof(SFMapEditor))]
public class SFMapEditorCustom : Editor {
    private enum Mode {
        Draw, Selection, Height, Delete
    };

    private enum SelectionMode {
        Grid, Tile
    };

    private Mode currentMode = Mode.Draw;
    private SelectionMode currentSelectionMode = SelectionMode.Tile;

    private SFMapEditor sfMapEditor;
    
    private Vector2 scrollPos;

    private int selectedIndex= -1;
    private int selectedTileset = 0;

    private bool useWater = false;

    private Stack<Action> undoStack = new Stack<Action>();

    private GameObject[] tileset;

    private static GUIStyle normalButton;
    private static GUIStyle activeButton;

    private void OnEnable() {
        sfMapEditor = (SFMapEditor)target;
    }

    private void EditorToolbox(int windowID) {
        GUI.Label(new Rect(5, 20, 60, 20), "Mode (R): ");
        GUI.Label(new Rect(65, 20, 70, 20), currentMode.ToString(), EditorStyles.boldLabel);

        if (GUI.Button(new Rect(5, 40, 40, 20), "Draw", currentMode == Mode.Draw ? activeButton : normalButton)) {
            currentMode = Mode.Draw;
        } else if (GUI.Button(new Rect(45, 40, 50, 20), "Select", currentMode == Mode.Selection ? activeButton : normalButton)) {
            currentMode = Mode.Selection;
        } else if (GUI.Button(new Rect(95, 40, 50, 20), "Height", currentMode == Mode.Height ? activeButton : normalButton)) {
            currentMode = Mode.Height;
        } else if (GUI.Button(new Rect(45, 60, 50, 20), "Delete", currentMode == Mode.Delete ? activeButton : normalButton)) {
            currentMode = Mode.Delete;
        }

        sfMapEditor.showGrid = GUI.Toggle(new Rect(5, 85, 110, 20), sfMapEditor.showGrid, "Show grid (G)");

        GUI.Label(new Rect(5, 105, 60, 20), "Selection: ");
        GUI.Label(new Rect(65, 105, 40, 20), currentSelectionMode.ToString(), EditorStyles.boldLabel);

        if (GUI.Button(new Rect(5, 125, 40, 20), "Grid", currentSelectionMode == SelectionMode.Grid ? activeButton : normalButton)) {
            currentSelectionMode = SelectionMode.Grid;
        } else if (GUI.Button(new Rect(45, 125, 40, 20), "Tile", currentSelectionMode == SelectionMode.Tile ? activeButton : normalButton)) {
            currentSelectionMode = SelectionMode.Tile;
        }

        if (currentMode == Mode.Draw) {
            useWater = GUI.Toggle(new Rect(5, 150, 110, 20), useWater, "Use water (W)");

            if (GUI.Button(new Rect(5, 170, 70, 20), "Fill empty")) {
                if (selectedIndex >= 0 && !useWater) {
                    List<GameObject> createdSquares = new List<GameObject>();

                    for (int i = 0; i < sfMapEditor.size.x; i++) {
                        for (int j = 0; j < sfMapEditor.size.y; j++) {
                            GameObject square = GameObject.Find("Square(" + i + "," + j + ")");

                            // Create the square if it doesn't exist
                            if (!square) {
                                square = CreateSquare(i, j);

                                CreateTile(square);

                                createdSquares.Add(square);
                            }
                        }
                    }

                    Debug.Log("Filled empty squares: " + createdSquares.Count);

                    if (createdSquares.Count > 0) {
                        undoStack.Push(() => {
                            foreach (GameObject createdSquare in createdSquares) {
                                DestroyImmediate(createdSquare);
                            }
                        });
                    }
                } else {
                    Debug.LogWarning("Can't fill squares when using water or no sprite selected");
                }
            }

            GUI.Label(new Rect(5, 190, 150, 20), "---------------------------");

            if (GUI.Button(new Rect(50, 205, 50, 20), "Undo")) {
                if (undoStack.Count > 0) {
                    (undoStack.Pop())();
                }
            }

            GUI.Label(new Rect(5, 230, 100, 20), "Undo stack: " + undoStack.Count);
        }
    }

    public override void OnInspectorGUI() {
        Event e = Event.current;

        GUILayout.Label("/!\\ Do NOT touch the Map GameObject and its children", EditorStyles.boldLabel);

        serializedObject.Update();
        GUILayout.Label("Grid", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("size"), new GUIContent("Size"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("gridColor"), new GUIContent("Color"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("scrollStep"), new GUIContent("Scroll Step"));
        GUILayout.Label("Sprite picker", EditorStyles.boldLabel);

        string[] subdirectories = Directory.GetDirectories(Application.dataPath + "/Resources/SFMapEditor/Tiles");

        for (int i = 0; i < subdirectories.Length; i++) {
            subdirectories[i] = System.IO.Path.GetFileName(subdirectories[i]);
        }

        EditorGUILayout.BeginHorizontal();

        GUILayout.Label("Tileset to use");

        int newTileset = EditorGUILayout.Popup(selectedTileset, subdirectories);

        if (tileset == null || tileset.Length == 0 || newTileset != selectedTileset) {
            selectedTileset = newTileset;
            selectedIndex = -1;
            tileset = Resources.LoadAll<GameObject>("SFMapEditor/Tiles/" + subdirectories[selectedTileset]);
        }

        EditorGUILayout.EndHorizontal();

        GUILayout.Label("(Subfolders in Resources/SFMapEditor/Tiles)");

        if (tileset != null && tileset.Length > 0) {
            float layoutWidth = Screen.width - 15; // 15 is for the scrollbar
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(layoutWidth), GUILayout.Height(200));

            float currentWidth = 0f;
            float currentHeight = 0f;
            float maxCurrentHeight = 0f;

            for (int i = 0; i < tileset.Length; i++) {
                Sprite currentTile = tileset[i].GetComponent<SpriteRenderer>().sprite;

                if (currentTile == null) continue;

                Rect spriteRect = new Rect(currentWidth, currentHeight, currentTile.bounds.size.x * Globals.PixelsPerUnit / 2, currentTile.bounds.size.y * Globals.PixelsPerUnit / 2);

                if (e.type == EventType.MouseDown && e.button == 0 && spriteRect.Contains(e.mousePosition)) {
                    selectedIndex = i;
                    useWater = false;
                }

                if (selectedIndex == i) {
                    Texture2D selectedBackground = new Texture2D(1, 1);
                    selectedBackground.SetPixel(0, 0, new Color(1f, 1f, 0.35f, 0.5f));
                    selectedBackground.wrapMode = TextureWrapMode.Repeat;
                    selectedBackground.Apply();
                    GUI.DrawTexture(spriteRect, selectedBackground);
                }

                GUI.DrawTexture(spriteRect, currentTile.texture);

                currentWidth += currentTile.bounds.size.x * Globals.PixelsPerUnit / 2;

                if (currentTile.bounds.size.y * Globals.PixelsPerUnit / 2 > maxCurrentHeight) {
                    maxCurrentHeight = currentTile.bounds.size.y * Globals.PixelsPerUnit / 2;
                }

                if (i < tileset.Length - 1 && currentWidth + tileset[i + 1].GetComponent<SpriteRenderer>().sprite.bounds.size.x * Globals.PixelsPerUnit / 2 >= layoutWidth) {
                    currentWidth = 0f;
                    currentHeight += maxCurrentHeight;
                    maxCurrentHeight = 0f;
                }
            }

            EditorGUILayout.EndScrollView();
        }

        GUILayout.Label("Water", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("water"), new GUIContent("GameObject"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("waterColor"), new GUIContent("Color"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("underwaterColor"), new GUIContent("Underwater color"));

        if (GUILayout.Button("Reset colors")) {
            sfMapEditor.ResetWaterColor();
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("waterOffset"), new GUIContent("Offset (32)"));

        serializedObject.ApplyModifiedProperties();
    }

    private void OnSceneGUI() {
        Event e = Event.current;

        // Initialize button styles
        if (normalButton == null) {
            normalButton = new GUIStyle(GUI.skin.button);
            activeButton = new GUIStyle(normalButton);
            activeButton.normal.background = activeButton.active.background;
        }

        // Base + UseWaterToggle + DeleteToggle + FillEmptyBtn + Separator + UndoBtn + UndoStackCount
        int windowHeight = currentMode == Mode.Draw ? 155 + 20 + 20 + 20 + 20 + 20 : 155;

        Handles.BeginGUI();
        GUI.Window(0, new Rect(20, 20, 150, windowHeight), EditorToolbox, "SFMapEditor");
        Handles.EndGUI();

        // Shortcuts
        if (e.type == EventType.KeyDown) {
            switch (e.keyCode) {
                case KeyCode.R:
                    e.Use();

                    if (currentMode == Mode.Draw) currentMode = Mode.Selection;
                    else if (currentMode == Mode.Selection) currentMode = Mode.Height;
                    else if (currentMode == Mode.Height) currentMode = Mode.Delete;
                    else if (currentMode == Mode.Delete) currentMode = Mode.Draw;

                    break;
                case KeyCode.G:
                    e.Use();
                    sfMapEditor.showGrid = !sfMapEditor.showGrid;
                    break;
                case KeyCode.W:
                    e.Use();

                    if (currentMode == Mode.Draw) {
                        useWater = !useWater;
                    }

                    break;
            }
        }

        sfMapEditor.hoveredSquare = null;

        // Selection
        if (currentSelectionMode == SelectionMode.Tile) {
            SFSquare visibleSquare = GetSquareHit();

            if (visibleSquare != null) {
                sfMapEditor.hoveredSquare = visibleSquare;
            }

            HandleUtility.Repaint();
        }

        // Update height of the last sprite of a square
        if (currentMode == Mode.Height) {
            if (e.isScrollWheel && e.type == EventType.ScrollWheel) {
                e.Use();


                if (currentSelectionMode == SelectionMode.Tile) {
                    GameObject tileHit = GetTileHit();

                    if (tileHit != null) {
                        float delta = -1f;

                        if (e.delta.y < 0) delta = 1f;

                        SFSquare squareHit = tileHit.GetComponentInParent<SFSquare>();

                        SpriteRenderer highestTile = null;

                        SpriteRenderer[] sprites = squareHit.GetComponentsInChildren<SpriteRenderer>();

                        foreach (SpriteRenderer sprite in sprites) {
                            if (highestTile == null || sprite.sortingOrder > highestTile.sortingOrder) {
                                highestTile = sprite;
                            }
                        }

                        tileHit.transform.Translate(new Vector3(0f, (delta * sfMapEditor.scrollStep) / Globals.TileHeight));

                        // If the tile hit is the highest of the square, we need to update the square height
                        if (highestTile != null && highestTile.gameObject == tileHit) {
                            SFSquare sfSquare = highestTile.GetComponentInParent<SFSquare>();
                            sfSquare.height += (int)delta * sfMapEditor.scrollStep;
                        }
                    }
                } else {
                    Vector3 mousePosition = new Vector3(e.mousePosition.x, -e.mousePosition.y + Camera.current.pixelHeight);
                    Vector3 localMousePos = Camera.current.ScreenToWorldPoint(mousePosition);

                    // Square coords
                    int Sx = Mathf.FloorToInt((localMousePos.x / 2) + localMousePos.y);
                    int Sy = Mathf.FloorToInt(localMousePos.y - (localMousePos.x / 2));

                    if (Sx < 0 || Sx >= sfMapEditor.size.x || Sy < 0 || Sy >= sfMapEditor.size.y) return;

                    if (currentSelectionMode == SelectionMode.Tile) {
                        SFSquare squareHit = GetSquareHit();

                        if (squareHit != null) {
                            Sx = squareHit.x;
                            Sy = squareHit.y;
                        }
                    }

                    GameObject square = GameObject.Find("Square(" + Sx + "," + Sy + ")");

                    if (square) {
                        SpriteRenderer highestTile = null;

                        SpriteRenderer[] sprites = square.GetComponentsInChildren<SpriteRenderer>();

                        foreach (SpriteRenderer sprite in sprites) {
                            if (highestTile == null || sprite.sortingOrder > highestTile.sortingOrder) {
                                highestTile = sprite;
                            }
                        }

                        if (highestTile != null) {
                            float delta = -1f;

                            if (e.delta.y < 0) delta = 1f;

                            highestTile.transform.Translate(new Vector3(0f, (delta * sfMapEditor.scrollStep) / Globals.TileHeight));

                            SFSquare sfSquare = highestTile.GetComponentInParent<SFSquare>();
                            sfSquare.height += (int)delta * sfMapEditor.scrollStep;
                        }
                    }
                }
            }
        }

        // Draw
        if (currentMode == Mode.Draw) {
            if (e.isMouse && e.type == EventType.MouseDown && e.button == 0 && (selectedIndex >= 0 || useWater)) {
                e.Use();

                if (useWater && !sfMapEditor.water) {
                    Debug.LogWarning("Trying to draw water but the sprite is null");

                    return;
                }

                Vector3 mousePosition = new Vector3(e.mousePosition.x, -e.mousePosition.y + Camera.current.pixelHeight);
                Vector3 localMousePos = Camera.current.ScreenToWorldPoint(mousePosition);

                // Square coords
                int Sx = Mathf.FloorToInt((localMousePos.x / 2) + localMousePos.y);
                int Sy = Mathf.FloorToInt(localMousePos.y - (localMousePos.x / 2));

                if (Sx < 0 || Sx >= sfMapEditor.size.x || Sy < 0 || Sy >= sfMapEditor.size.y) return;

                if (currentSelectionMode == SelectionMode.Tile) {
                    SFSquare squareHit = GetSquareHit();

                    if (squareHit != null) {
                        Sx = squareHit.x;
                        Sy = squareHit.y;
                    }
                }

                int highestSortingOrder = 0;

                GameObject square = GameObject.Find("Square(" + Sx + "," + Sy + ")");

                // Create the square if it doesn't exist
                if (!square) {
                    square = CreateSquare(Sx, Sy);

                    CreateTile(square, highestSortingOrder);

                    undoStack.Push(() => {
                        DestroyImmediate(square);
                    });
                } else {
                    SpriteRenderer[] sprites = square.GetComponentsInChildren<SpriteRenderer>();
                    
                    foreach (SpriteRenderer sprite in sprites) {
                        if (sprite.sortingOrder > highestSortingOrder) {
                            highestSortingOrder = sprite.sortingOrder;
                        }
                    }

                    highestSortingOrder++;

                    GameObject tile = CreateTile(square, highestSortingOrder, (float)square.GetComponent<SFSquare>().height / Globals.PixelsPerUnit);

                    undoStack.Push(() => {
                        DestroyImmediate(tile);

                        if (useWater) {
                            SpriteRenderer[] underwaterSprites = square.GetComponentsInChildren<SpriteRenderer>();
                            
                            foreach (SpriteRenderer underwaterSprite in underwaterSprites) {
                                underwaterSprite.color = Color.white;
                            }
                        }
                    });
                }
            }
        }

        if (currentMode == Mode.Draw || currentMode == Mode.Height) {
            Selection.activeGameObject = sfMapEditor.gameObject;
        }
    }

    private GameObject CreateSquare(int x, int y) {
        // Center of the square
        float Cx = x - y;
        float Cy = (x + y + 1f) / 2f;

        GameObject square = new GameObject("Square(" + x + "," + y + ")");
        square.transform.position = new Vector3(Cx, Cy, 0f);
        SFSquare sfSquare = square.AddComponent<SFSquare>();
        sfSquare.x = x;
        sfSquare.y = y;
        sfSquare.height = 0;
        SortingGroup sortingGroup = square.AddComponent<SortingGroup>();
        sortingGroup.sortingOrder = -(sfMapEditor.size.x * y + x);
        square.transform.SetParent(sfMapEditor.map.transform);

        return square;
    }

    private GameObject CreateTile(GameObject square, int sortingOrder = 0, float height = 0f) {
        GameObject tile = PrefabUtility.InstantiatePrefab(useWater ? sfMapEditor.water : tileset[selectedIndex]) as GameObject;
        tile.transform.SetParent(square.transform);

        SpriteRenderer spriteRenderer = tile.GetComponent<SpriteRenderer>();

        if (useWater) {
            SpriteRenderer[] sprites = square.GetComponentsInChildren<SpriteRenderer>();

            // Make underwater sprites look better
            foreach (SpriteRenderer sprite in sprites) {
                sprite.color = sfMapEditor.underwaterColor;
            }

            spriteRenderer.color = sfMapEditor.waterColor;
            tile.transform.localPosition = new Vector3(0f, (float)sfMapEditor.waterOffset / Globals.PixelsPerUnit);
        } else {
            tile.transform.localPosition = new Vector3(0f, height);
        }

        spriteRenderer.sortingOrder = sortingOrder;

        return tile;
    }

    private SFSquare GetSquareHit() {
        GameObject tileHit = GetTileHit();

        return tileHit ? tileHit.GetComponentInParent<SFSquare>() : null;
    }

    private GameObject GetTileHit() {
        GameObject visibleTileHit = null;

        foreach (RaycastHit2D hit in Physics2D.RaycastAll(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin, Vector2.zero)) {
            int hitSortingOrder = hit.collider.GetComponentInParent<SortingGroup>().sortingOrder;

            // Retrieve the closiest map object, the one we are seeing
            if (visibleTileHit == null || hitSortingOrder > visibleTileHit.GetComponentInParent<SortingGroup>().sortingOrder) {
                visibleTileHit = hit.collider.gameObject;
            }
        }

        return visibleTileHit;
    }
}
