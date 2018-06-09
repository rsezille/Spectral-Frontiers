using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[CustomEditor(typeof(SFMapEditor))]
public class SFMapCustomEditor : Editor {
    private SFMapEditor world;

    private bool clickToUp = false;
    private bool drawMode = true;

    private Vector2 scrollPos;

    private void OnEnable() {
        world = (SFMapEditor)target;
    }

    private void EditorToolbox(int windowID) {
        drawMode = GUI.Toggle(new Rect(10, 20, 110, 20), drawMode, "Draw mode (D)");
        world.showGrid = GUI.Toggle(new Rect(10, 40, 110, 20), world.showGrid, "Toggle grid (G)");

        GUI.DragWindow(new Rect(0, 0, 10000, 20));
    }

    Sprite selectedSprite;
    Rect selectedRect;
    int selectedIndex;

    public override void OnInspectorGUI() {
        float startTime = Time.realtimeSinceStartup;

        Event e = Event.current;

        serializedObject.Update();
        GUILayout.Label("Grid", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("size"), new GUIContent("Size"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("gridColor"), new GUIContent("Color"));
        GUILayout.Label("Sprite picker", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("currentAtlas"), new GUIContent("Atlas"));
        serializedObject.ApplyModifiedProperties();

        // Sprite picker
        if (!world.currentAtlas) return;

        float layoutWidth = Screen.width - 15; // 15 is for the scrollbar
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(layoutWidth), GUILayout.Height(200));

        Sprite[] atlasSprites = new Sprite[world.currentAtlas.spriteCount];
        world.currentAtlas.GetSprites(atlasSprites);

        float currentWidth = 0f;
        float currentHeight = 0f;
        float maxCurrentHeight = 0f;


        /*if (selectedSprite && selectedRect != null) {
            Texture2D selectedBackground = new Texture2D(1, 1);
            selectedBackground.SetPixel(0, 0, Color.yellow);
            selectedBackground.wrapMode = TextureWrapMode.Repeat;
            selectedBackground.Apply();
            GUI.DrawTexture(selectedRect, selectedBackground);
        }*/

        for (int i = 0; i < atlasSprites.Length; i++) {
            Rect spriteRect = new Rect(currentWidth, currentHeight, atlasSprites[i].bounds.size.x * Globals.PixelsPerUnit / 2, atlasSprites[i].bounds.size.y * Globals.PixelsPerUnit / 2);

            if (e.type == EventType.MouseDown && e.button == 0 && spriteRect.Contains(e.mousePosition)) {
                Debug.Log("pouet" + i);
                selectedSprite = atlasSprites[i];
                selectedRect = spriteRect;
                selectedIndex = i;
                
            }

            if (selectedIndex == i) {
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

        Debug.Log("time: " + (Time.realtimeSinceStartup - startTime));
    }

    private void OnSceneGUI() {
        Event e = Event.current;
        

        Handles.BeginGUI();
        GUI.Window(0, new Rect(20, 20, 130, 80), EditorToolbox, "SFMapEditor");
        Handles.EndGUI();

        if (e.type == EventType.KeyDown) {
            switch (e.keyCode) {
                case KeyCode.D:
                    drawMode = !drawMode;
                    break;
                case KeyCode.G:
                    world.showGrid = !world.showGrid;
                    break;
            }

            e.Use();
        }

        if (drawMode) {
            if (e.isMouse && e.type == EventType.MouseDown && e.button == 0 && selectedSprite) {
                e.Use();

                Vector3 mousePosition = new Vector3(e.mousePosition.x, -e.mousePosition.y + Camera.current.pixelHeight);
                Vector3 localMousePos = Camera.current.ScreenToWorldPoint(mousePosition);

                // Square coords
                int Sx = Mathf.FloorToInt((localMousePos.x / 2) + localMousePos.y);
                int Sy = Mathf.FloorToInt(localMousePos.y - (localMousePos.x / 2));

                if (Sx < 0 || Sx >= world.size.x || Sy < 0 || Sy >= world.size.y) return;

                GameObject square = GameObject.Find("Square(" + Sx + "," + Sy + ")");

                //TODO: testing
                /*if (clickToUp) {
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
                }*/

                // Center of the square
                float Cx = Sx - Sy;
                float Cy = (Sx + Sy + 1f) / 2f;

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

                GameObject go = new GameObject("Tile");
                go.transform.position = new Vector3(Cx, Cy, 0f);
                SpriteRenderer spriteRenderer = go.AddComponent<SpriteRenderer>();
                go.transform.SetParent(square.transform);
                spriteRenderer.sprite = selectedSprite;
                spriteRenderer.sortingOrder = highestSortingOrder;

            }
        }

        Selection.activeGameObject = world.gameObject;
    }
}
