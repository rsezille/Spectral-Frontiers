using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[CustomEditor(typeof(SFMapEditor))]
public class SFMapCustomEditor : Editor {
    private enum Mode {
        Draw, Selection, Height
    };

    private Mode currentMode = Mode.Draw;

    private SFMapEditor world;
    
    private Vector2 scrollPos;

    private Sprite selectedSprite;
    private int selectedIndex;

    private bool useWater = false;

    private Stack<Action> undoStack = new Stack<Action>();

    private void OnEnable() {
        world = (SFMapEditor)target;
    }

    private void EditorToolbox(int windowID) {
        GUI.Label(new Rect(5, 20, 60, 20), "Mode (D): ");
        GUI.Label(new Rect(65, 20, 70, 20), currentMode.ToString(), EditorStyles.boldLabel);

        if (GUI.Button(new Rect(5, 40, 40, 20), "Draw")) {
            currentMode = Mode.Draw;
        } else if (GUI.Button(new Rect(45, 40, 50, 20), "Select")) {
            currentMode = Mode.Selection;
        } else if (GUI.Button(new Rect(95, 40, 50, 20), "Height")) {
            currentMode = Mode.Height;
        }
        
        world.showGrid = GUI.Toggle(new Rect(5, 65, 110, 20), world.showGrid, "Toggle grid (G)");

        if (currentMode == Mode.Draw) {
            useWater = GUI.Toggle(new Rect(5, 85, 110, 20), useWater, "Use water (W)");

            if (GUI.Button(new Rect(5, 105, 70, 20), "Fill empty")) {
                if (selectedSprite && !useWater) {
                    List<GameObject> createdSquares = new List<GameObject>();

                    for (int i = 0; i < world.size.x; i++) {
                        for (int j = 0; j < world.size.y; j++) {
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

            GUI.Label(new Rect(5, 125, 150, 20), "---------------------------");

            if (GUI.Button(new Rect(50, 140, 50, 20), "Undo")) {
                if (undoStack.Count > 0) {
                    (undoStack.Pop())();
                }
            }

            GUI.Label(new Rect(5, 165, 100, 20), "Undo stack: " + undoStack.Count);
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
        EditorGUILayout.PropertyField(serializedObject.FindProperty("currentAtlas"), new GUIContent("Atlas"));

        // Sprite picker
        if (world.currentAtlas) {
            float layoutWidth = Screen.width - 15; // 15 is for the scrollbar
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(layoutWidth), GUILayout.Height(200));

            Sprite[] atlasSprites = new Sprite[world.currentAtlas.spriteCount];
            world.currentAtlas.GetSprites(atlasSprites);

            float currentWidth = 0f;
            float currentHeight = 0f;
            float maxCurrentHeight = 0f;

            for (int i = 0; i < atlasSprites.Length; i++) {
                Rect spriteRect = new Rect(currentWidth, currentHeight, atlasSprites[i].bounds.size.x * Globals.PixelsPerUnit / 2, atlasSprites[i].bounds.size.y * Globals.PixelsPerUnit / 2);

                if (e.type == EventType.MouseDown && e.button == 0 && spriteRect.Contains(e.mousePosition)) {
                    selectedSprite = atlasSprites[i];
                    selectedIndex = i;
                    useWater = false;
                }

                if (selectedIndex == i && selectedSprite) {
                    Texture2D selectedBackground = new Texture2D(1, 1);
                    selectedBackground.SetPixel(0, 0, new Color(1f, 1f, 0.35f, 0.5f));
                    selectedBackground.wrapMode = TextureWrapMode.Repeat;
                    selectedBackground.Apply();
                    GUI.DrawTexture(spriteRect, selectedBackground);
                }

                GUI.DrawTexture(spriteRect, atlasSprites[i].texture);

                currentWidth += atlasSprites[i].bounds.size.x * Globals.PixelsPerUnit / 2;

                if (atlasSprites[i].bounds.size.y * Globals.PixelsPerUnit / 2 > maxCurrentHeight) {
                    maxCurrentHeight = atlasSprites[i].bounds.size.y * Globals.PixelsPerUnit / 2;
                }

                if (i < atlasSprites.Length - 1 && currentWidth + atlasSprites[i + 1].bounds.size.x * Globals.PixelsPerUnit / 2 >= layoutWidth) {
                    currentWidth = 0f;
                    currentHeight += maxCurrentHeight;
                    maxCurrentHeight = 0f;
                }
            }

            EditorGUILayout.EndScrollView();
        }

        GUILayout.Label("Water", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("waterSprite"), new GUIContent("Sprite"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("waterColor"), new GUIContent("Color"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("underwaterColor"), new GUIContent("Underwater color"));

        if (GUILayout.Button("Reset colors")) {
            world.ResetWaterColor();
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("waterOffset"), new GUIContent("Offset (32)"));

        serializedObject.ApplyModifiedProperties();
    }

    private void OnSceneGUI() {
        Event e = Event.current;

        int windowHeight = currentMode == Mode.Draw ? 90 + 20 + 20 + 20 + 20 + 20: 90; // Base + UseWaterToggle + FillEmptyBtn + Separator + UndoBtn + UndoStackCount

        Handles.BeginGUI();
        GUI.Window(0, new Rect(20, 20, 150, windowHeight), EditorToolbox, "SFMapEditor");
        Handles.EndGUI();

        // Shortcuts
        if (e.type == EventType.KeyDown) {
            switch (e.keyCode) {
                case KeyCode.D:
                    e.Use();

                    if (currentMode == Mode.Draw) currentMode = Mode.Selection;
                    else if (currentMode == Mode.Selection) currentMode = Mode.Height;
                    else if (currentMode == Mode.Height) currentMode = Mode.Draw;

                    break;
                case KeyCode.G:
                    e.Use();
                    world.showGrid = !world.showGrid;
                    break;
                case KeyCode.W:
                    e.Use();

                    if (currentMode == Mode.Draw) {
                        useWater = !useWater;
                    }

                    break;
            }
        }

        // Update height of the last sprite of a square
        if (currentMode == Mode.Height) {
            if (e.isScrollWheel && e.type == EventType.ScrollWheel) {
                e.Use();

                Vector3 mousePosition = new Vector3(e.mousePosition.x, -e.mousePosition.y + Camera.current.pixelHeight);
                Vector3 localMousePos = Camera.current.ScreenToWorldPoint(mousePosition);

                // Square coords
                int Sx = Mathf.FloorToInt((localMousePos.x / 2) + localMousePos.y);
                int Sy = Mathf.FloorToInt(localMousePos.y - (localMousePos.x / 2));

                if (Sx < 0 || Sx >= world.size.x || Sy < 0 || Sy >= world.size.y) return;

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

                        highestTile.transform.Translate(new Vector3(0f, (delta * world.scrollStep) / Globals.TileHeight));

                        SFSquare sfSquare = highestTile.GetComponentInParent<SFSquare>();
                        sfSquare.altitude += (int)delta * world.scrollStep;
                    }
                }
            }
        }

        if (currentMode == Mode.Draw) {
            if (e.isMouse && e.type == EventType.MouseDown && e.button == 0 && (selectedSprite || useWater)) {
                e.Use();

                if (useWater && !world.waterSprite) {
                    Debug.LogWarning("Trying to draw water but the sprite is null");

                    return;
                }

                Vector3 mousePosition = new Vector3(e.mousePosition.x, -e.mousePosition.y + Camera.current.pixelHeight);
                Vector3 localMousePos = Camera.current.ScreenToWorldPoint(mousePosition);

                // Square coords
                int Sx = Mathf.FloorToInt((localMousePos.x / 2) + localMousePos.y);
                int Sy = Mathf.FloorToInt(localMousePos.y - (localMousePos.x / 2));

                if (Sx < 0 || Sx >= world.size.x || Sy < 0 || Sy >= world.size.y) return;

                

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

                    GameObject tile = CreateTile(square, highestSortingOrder);

                    undoStack.Push(() => {
                        DestroyImmediate(tile);
                    });
                }
            }
        }

        if (currentMode == Mode.Draw || currentMode == Mode.Height) {
            Selection.activeGameObject = world.gameObject;
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
        sfSquare.altitude = 0;
        SortingGroup sortingGroup = square.AddComponent<SortingGroup>();
        sortingGroup.sortingOrder = -(world.size.x * y + x);
        square.transform.SetParent(world.map.transform);

        return square;
    }

    private GameObject CreateTile(GameObject square, int sortingOrder = 0) {
        GameObject tile = new GameObject("Tile");
        tile.transform.SetParent(square.transform);

        SpriteRenderer spriteRenderer = tile.AddComponent<SpriteRenderer>();

        // Using the right sprite if we're drawing water or not
        if (useWater) {
            SpriteRenderer[] sprites = square.GetComponentsInChildren<SpriteRenderer>();

            // Make underwater sprites look better
            foreach (SpriteRenderer sprite in sprites) {
                sprite.color = world.underwaterColor;
            }

            spriteRenderer.sprite = world.waterSprite;
            spriteRenderer.color = world.waterColor;
        } else {
            spriteRenderer.sprite = selectedSprite;
        }

        spriteRenderer.sortingOrder = sortingOrder;

        if (useWater) {
            tile.transform.localPosition = new Vector3(0f, (float)world.waterOffset / Globals.PixelsPerUnit);
        } else {
            tile.transform.localPosition = Vector3.zero;
        }

        PolygonCollider2D poly = tile.AddComponent<PolygonCollider2D>();

        GameObject collider = new GameObject("Collider");
        collider.transform.SetParent(tile.transform);
        PolygonCollider2D pouet = collider.AddComponent<PolygonCollider2D>();
        pouet.SetPath(0, poly.GetPath(0));
        poly.enabled = false;
        collider.transform.localPosition = Vector3.zero;
        collider.transform.localScale = new Vector3(0.95f, 0.95f);

        return tile;
    }
}
