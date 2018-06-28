using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class SFMapEditor : MonoBehaviour {
    public enum Mode {
        Draw, Selection, Height, Delete, Block
    };

    public enum SelectMode {
        Grid, Tile
    };

    public GameObject map;

    public Vector2Int size = new Vector2Int(3, 3);

    public bool showGrid = true;
    public Color gridColor = Color.grey;
    
    [Range(1, 64)]
    public int scrollStep = 1;

    public SFSquare hoveredSquare;

    public Mode currentMode = Mode.Draw;
    public SelectMode currentSelectMode = SelectMode.Tile;

    public Stack<Action> undoStack = new Stack<Action>();

    public bool useWater = false;

    private Color orange = new Color(1f, 0.5f, 0f);

    private void OnDrawGizmos() {
        if (!map) map = GameObject.Find("Map") ?? CreateNewMap();

        size.Clamp(new Vector2Int(1, 1), new Vector2Int(1000, 1000));

        if (showGrid && currentMode != Mode.Block) {
            Gizmos.color = gridColor;

            for (int x = 0; x <= size.x; x++) {
                Gizmos.DrawLine(new Vector3(x, x / 2f), new Vector3(-size.y + x, (size.y + x) / 2f));
            }

            for (int y = 0; y <= size.y; y++) {
                Gizmos.DrawLine(new Vector3(-y, y / 2f), new Vector3(size.x - y, (size.x + y) / 2f));
            }
        }

        if (currentMode == Mode.Block) {
            SFSquare[] squares = map.GetComponentsInChildren<SFSquare>();

            foreach (SFSquare square in squares) {
                Gizmos.color = square.solid ? orange : square.IsNotBlocking() ? Color.green : Color.red;

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

        if (hoveredSquare != null) {
            Color hoveredColor = new Color(0f, 0.7f, 1f);
            Gizmos.color = hoveredColor;

            Vector3 bottom = new Vector3(hoveredSquare.x - hoveredSquare.y, (hoveredSquare.x + hoveredSquare.y) / 2f);
            Vector3 right = new Vector3(hoveredSquare.x - hoveredSquare.y + 1f, (hoveredSquare.x + hoveredSquare.y + 1f) / 2f);
            Vector3 left = new Vector3(hoveredSquare.x - hoveredSquare.y - 1f, (hoveredSquare.x + hoveredSquare.y + 1f) / 2f);
            Vector3 top = new Vector3(hoveredSquare.x - hoveredSquare.y, (hoveredSquare.x + hoveredSquare.y) / 2f + 1f);

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

            Handles.Label(hoveredSquare.transform.position - new Vector3(0.5f, -0.2f), "(" + hoveredSquare.x + "," + hoveredSquare.y + "," + hoveredSquare.Height + ")", style);
        }
    }

    private GameObject CreateNewMap() {
        GameObject map = new GameObject("Map");

        map.AddComponent<SFMap>();

        return map;
    }

    public GameObject CreateSquare(int x, int y) {
        // Center of the square
        float Cx = x - y;
        float Cy = (x + y + 1f) / 2f;

        GameObject square = new GameObject("Square(" + x + "," + y + ")");
        square.transform.position = new Vector3(Cx, Cy, 0f);
        SFSquare sfSquare = square.AddComponent<SFSquare>();
        sfSquare.x = x;
        sfSquare.y = y;
        sfSquare.Height = 0;
        SortingGroup sortingGroup = square.AddComponent<SortingGroup>();
        sortingGroup.sortingOrder = -(size.x * y + x);
        square.transform.SetParent(map.transform);

        // Create the tile container
        GameObject tileContainer = new GameObject("TileContainer");
        tileContainer.AddComponent<SFTileContainer>();
        SortingGroup sgTileContainer = tileContainer.AddComponent<SortingGroup>();
        sgTileContainer.sortingOrder = 0;
        tileContainer.transform.SetParent(square.transform);
        tileContainer.transform.localPosition = Vector3.zero;

        // Create the entity container
        GameObject entityContainer = new GameObject("EntityContainer");
        entityContainer.AddComponent<SFEntityContainer>();
        SortingGroup sgEntityContainer = entityContainer.AddComponent<SortingGroup>();
        sgEntityContainer.sortingOrder = 1;
        entityContainer.transform.SetParent(square.transform);
        entityContainer.transform.localPosition = Vector3.zero;

        return square;
    }

    public GameObject CreateTile(SFSquare square, int sortingOrder = 0, float height = 0f) {
        SFSpritePicker sfSpritePicker = GetComponent<SFSpritePicker>();

        GameObject tile = PrefabUtility.InstantiatePrefab(useWater ? sfSpritePicker.waterPrefab : sfSpritePicker.tileset[sfSpritePicker.selectedIndex]) as GameObject;
        tile.transform.SetParent(square.tileContainer.transform);

        SpriteRenderer spriteRenderer = tile.GetComponent<SpriteRenderer>();

        if (useWater) {
            SpriteRenderer[] sprites = square.tileContainer.GetComponentsInChildren<SpriteRenderer>();

            // Make underwater sprites look better
            foreach (SpriteRenderer sprite in sprites) {
                sprite.color = sfSpritePicker.underwaterColor;
            }

            spriteRenderer.color = sfSpritePicker.waterColor;
            tile.transform.localPosition = new Vector3(0f, (float)sfSpritePicker.waterOffset / Globals.PixelsPerUnit);
        } else {
            tile.transform.localPosition = new Vector3(0f, height);
        }

        spriteRenderer.sortingOrder = sortingOrder;

        return tile;
    }

    public GameObject CreateEntity(SFSquare square) {
        SFSpritePicker sfSpritePicker = GetComponent<SFSpritePicker>();

        GameObject entity = PrefabUtility.InstantiatePrefab(sfSpritePicker.tileset[sfSpritePicker.selectedIndex]) as GameObject;
        entity.transform.SetParent(square.entityContainer.transform);

        return entity;
    }

    public void FillEmpty() {
        if (GetComponent<SFSpritePicker>().selectedIndex >= 0 && !useWater) {
            List<GameObject> createdSquares = new List<GameObject>();

            for (int i = 0; i < size.x; i++) {
                for (int j = 0; j < size.y; j++) {
                    GameObject square = GameObject.Find("Square(" + i + "," + j + ")");

                    // Create the square if it doesn't exist
                    if (!square) {
                        square = CreateSquare(i, j);

                        CreateTile(square.GetComponent<SFSquare>());

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
}
