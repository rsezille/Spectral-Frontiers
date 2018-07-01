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

    public Square hoveredSquare;

    public Mode currentMode = Mode.Draw;
    public SelectMode currentSelectMode = SelectMode.Tile;

    public Stack<Action> undoStack = new Stack<Action>();

    public bool useWater = false;

    public Color orange = new Color(1f, 0.5f, 0f);

    private void OnDrawGizmos() {
        if (!map) map = GameObject.Find("Map") ?? CreateNewMap();

        size.Clamp(new Vector2Int(1, 1), new Vector2Int(1000, 1000));
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
        Square sfSquare = square.AddComponent<Square>();
        sfSquare.x = x;
        sfSquare.y = y;
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

        // Create the tile selector
        GameObject tileSelector = PrefabUtility.InstantiatePrefab(GetComponent<SFSpritePicker>().tileSelectorPrefab) as GameObject;
        tileSelector.transform.SetParent(entityContainer.transform);
        tileSelector.transform.localPosition = Vector3.zero;
        tileSelector.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        tileSelector.GetComponent<SFTileSelector>().square = sfSquare;

        sfSquare.tileSelector = tileSelector.GetComponent<SpriteRenderer>();

        return square;
    }

    public GameObject CreateTile(Square square, int sortingOrder = 0, float height = 0f) {
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
            tile.transform.localPosition = new Vector3(0f, ((float)sfSpritePicker.waterOffset / Globals.PixelsPerUnit) + height);
        } else {
            tile.transform.localPosition = new Vector3(0f, height);
        }

        spriteRenderer.sortingOrder = sortingOrder;

        return tile;
    }

    public GameObject CreateEntity(Square square) {
        SFSpritePicker sfSpritePicker = GetComponent<SFSpritePicker>();

        GameObject entity = PrefabUtility.InstantiatePrefab(sfSpritePicker.tileset[sfSpritePicker.selectedIndex]) as GameObject;
        entity.transform.SetParent(square.entityContainer.transform);
        entity.transform.localPosition = Vector3.zero;

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

                        CreateTile(square.GetComponent<Square>());

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
