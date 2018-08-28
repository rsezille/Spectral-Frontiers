using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace SF {
    public class MapEditor : MonoBehaviour {
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

        public Light sunLight;
        public bool lightingToolboxEnabled = false;

        public bool editorToolboxEnabled = true;

        private void OnDrawGizmos() {
            if (!map) map = GameObject.Find("Map") ?? CreateNewMap();

            size.Clamp(new Vector2Int(1, 1), new Vector2Int(1000, 1000));
        }

        private GameObject CreateNewMap() {
            return new GameObject("Map", typeof(Map));
        }

        public GameObject CreateSquare(int x, int y) {
            GameObject squareGameObject = new GameObject("Square(" + x + "," + y + ")", typeof(Square));
            squareGameObject.transform.position = BoardUtil.CoordToWorldPosition(x, y);
            Square square = squareGameObject.GetComponent<Square>();
            square.x = x;
            square.y = y;
            squareGameObject.GetComponent<SortingGroup>().sortingOrder = -(size.x * y + x);
            squareGameObject.transform.SetParent(map.transform);

            // Create the tile container
            GameObject tileContainer = new GameObject("TileContainer", typeof(TileContainer));
            tileContainer.GetComponent<SortingGroup>().sortingOrder = 0;
            tileContainer.transform.SetParent(squareGameObject.transform);
            tileContainer.transform.localPosition = Vector3.zero;

            // Create the entity container
            GameObject entityContainer = new GameObject("EntityContainer", typeof(EntityContainer));
            entityContainer.GetComponent<SortingGroup>().sortingOrder = 1;
            entityContainer.transform.SetParent(squareGameObject.transform);
            entityContainer.transform.localPosition = Vector3.zero;

            // Create the tile selector
            GameObject tileSelector = PrefabUtility.InstantiatePrefab(GetComponent<SpritePicker>().tileSelectorPrefab) as GameObject;
            tileSelector.transform.SetParent(entityContainer.transform);
            tileSelector.transform.localPosition = Vector3.zero;
            tileSelector.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
            tileSelector.GetComponent<TileSelector>().square = square;

            square.tileSelector = tileSelector.GetComponent<SpriteRenderer>();

            return squareGameObject;
        }

        public GameObject CreateTile(Square square, int sortingOrder = 0, float height = 0f) {
            SpritePicker sfSpritePicker = GetComponent<SpritePicker>();

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

                tile.GetComponentInParent<Square>().Height += sfSpritePicker.waterOffset;
            } else {
                tile.transform.localPosition = new Vector3(0f, height);
            }

            spriteRenderer.sortingOrder = sortingOrder;

            return tile;
        }

        public GameObject CreateEntity(Square square) {
            SpritePicker sfSpritePicker = GetComponent<SpritePicker>();

            GameObject entity = PrefabUtility.InstantiatePrefab(sfSpritePicker.tileset[sfSpritePicker.selectedIndex]) as GameObject;
            entity.transform.SetParent(square.entityContainer.transform);
            entity.transform.localPosition = Vector3.zero;

            return entity;
        }

        public void FillEmpty() {
            if (GetComponent<SpritePicker>().selectedIndex >= 0 && !useWater) {
                List<GameObject> createdSquares = new List<GameObject>();

                for (int i = 0; i < size.x; i++) {
                    for (int j = 0; j < size.y; j++) {
                        GameObject square = map.transform.Find("Square(" + i + "," + j + ")")?.gameObject;

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
}
