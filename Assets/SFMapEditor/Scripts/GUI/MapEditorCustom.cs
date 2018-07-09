using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace SF {
    [CustomEditor(typeof(MapEditor))]
    public class MapEditorCustom : Editor {
        private MapEditor sfMapEditor;

        private SpritePicker sfSpritePicker; // Shortcut

        private void OnEnable() {
            sfMapEditor = (MapEditor)target;
            sfSpritePicker = sfMapEditor.GetComponent<SpritePicker>();
        }

        public override void OnInspectorGUI() {
            Event e = Event.current;
            serializedObject.Update();

            GUILayout.Label("/!\\ Do NOT touch the Map GameObject and its children", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("map"), new GUIContent("Map GameObject"));

            GUILayout.Label("Grid", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("size"), new GUIContent("Size"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("gridColor"), new GUIContent("Color"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("scrollStep"), new GUIContent("Scroll Step"));

            serializedObject.ApplyModifiedProperties();
        }

        private void OnSceneGUI() {
            if (sfMapEditor.currentMode != MapEditor.Mode.Selection) {
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive)); // Disable selection when holding the left mouse click
            }

            Event e = Event.current;

            // Shortcuts
            if (e.type == EventType.KeyDown) {
                switch (e.keyCode) {
                    case KeyCode.R:
                        e.Use();

                        if (sfMapEditor.currentMode == MapEditor.Mode.Draw) sfMapEditor.currentMode = MapEditor.Mode.Selection;
                        else if (sfMapEditor.currentMode == MapEditor.Mode.Selection) sfMapEditor.currentMode = MapEditor.Mode.Height;
                        else if (sfMapEditor.currentMode == MapEditor.Mode.Height) sfMapEditor.currentMode = MapEditor.Mode.Delete;
                        else if (sfMapEditor.currentMode == MapEditor.Mode.Delete) sfMapEditor.currentMode = MapEditor.Mode.Block;
                        else if (sfMapEditor.currentMode == MapEditor.Mode.Block) sfMapEditor.currentMode = MapEditor.Mode.Draw;

                        break;
                    case KeyCode.G:
                        e.Use();
                        sfMapEditor.showGrid = !sfMapEditor.showGrid;
                        break;
                    case KeyCode.W:
                        e.Use();

                        if (sfMapEditor.currentMode == MapEditor.Mode.Draw) {
                            sfMapEditor.useWater = !sfMapEditor.useWater;
                        }

                        break;
                    case KeyCode.T:
                        e.Use();

                        if (sfMapEditor.currentSelectMode == MapEditor.SelectMode.Grid) sfMapEditor.currentSelectMode = MapEditor.SelectMode.Tile;
                        else if (sfMapEditor.currentSelectMode == MapEditor.SelectMode.Tile) sfMapEditor.currentSelectMode = MapEditor.SelectMode.Grid;

                        break;
                    case KeyCode.LeftControl:
                        e.Use();

                        if (sfMapEditor.currentMode == MapEditor.Mode.Block) {
                            if (sfMapEditor.map.activeSelf) {
                                sfMapEditor.map.SetActive(false);
                            }
                        }

                        break;
                }
            } else if (e.type == EventType.KeyUp) {
                switch (e.keyCode) {
                    case KeyCode.LeftControl:
                        e.Use();

                        sfMapEditor.map.SetActive(true);

                        break;
                }
            }

            sfMapEditor.hoveredSquare = null;

            // Selection
            if (sfMapEditor.currentSelectMode == MapEditor.SelectMode.Tile) {
                sfMapEditor.hoveredSquare = GetSquareHit();
            } else if (sfMapEditor.currentSelectMode == MapEditor.SelectMode.Grid) {
                Vector2Int squarePosition = GetSquarePosition(e.mousePosition);

                if (squarePosition.x < 0 || squarePosition.x >= sfMapEditor.size.x || squarePosition.y < 0 || squarePosition.y >= sfMapEditor.size.y) return;

                GameObject square = GameObject.Find("Square(" + squarePosition.x + "," + squarePosition.y + ")");

                if (square) {
                    sfMapEditor.hoveredSquare = square.GetComponent<Square>();
                }
            }

            HandleUtility.Repaint(); // Faster OnSceneGUI calls

            // Height
            if (sfMapEditor.currentMode == MapEditor.Mode.Height) {
                if (e.isScrollWheel && e.type == EventType.ScrollWheel) {
                    e.Use();

                    if (sfMapEditor.currentSelectMode == MapEditor.SelectMode.Tile) {
                        GameObject tileHit = GetTileHit();

                        if (tileHit != null) {
                            float delta = e.delta.y < 0 ? 1f : -1f;

                            tileHit.transform.Translate(new Vector3(0f, (delta * sfMapEditor.scrollStep) / Globals.TileHeight));

                            SpriteRenderer highestTile = GetHighestTile(tileHit.GetComponentInParent<Square>());

                            // If the tile hit is the highest of the square, we need to update the square height
                            if (highestTile != null && highestTile.gameObject == tileHit) {
                                Square sfSquare = highestTile.GetComponentInParent<Square>();
                                sfSquare.Height += (int)delta * sfMapEditor.scrollStep;
                            }
                        }
                    } else if (sfMapEditor.currentSelectMode == MapEditor.SelectMode.Grid) { // Update height of the last tile of the square
                        Vector2Int squarePosition = GetSquarePosition(e.mousePosition);

                        if (squarePosition.x < 0 || squarePosition.x >= sfMapEditor.size.x || squarePosition.y < 0 || squarePosition.y >= sfMapEditor.size.y) return;

                        GameObject square = GameObject.Find("Square(" + squarePosition.x + "," + squarePosition.y + ")");

                        if (square) {
                            SpriteRenderer highestTile = GetHighestTile(square.GetComponent<Square>());

                            if (highestTile != null) {
                                float delta = e.delta.y < 0 ? 1f : -1f;

                                highestTile.transform.Translate(new Vector3(0f, (delta * sfMapEditor.scrollStep) / Globals.TileHeight));

                                Square sfSquare = highestTile.GetComponentInParent<Square>();
                                sfSquare.Height += (int)delta * sfMapEditor.scrollStep;
                            }
                        }
                    }
                }
            }

            // Draw
            if (sfMapEditor.currentMode == MapEditor.Mode.Draw) {
                if (e.isMouse && e.type == EventType.MouseDrag && e.button == 0 && sfSpritePicker.selectedIndex >= 0 && sfMapEditor.currentSelectMode == MapEditor.SelectMode.Grid) {
                    e.Use();

                    if (sfSpritePicker.isEntity) {
                        Debug.LogWarning("Can't draw an entity on an empty square ; at least one tile is required");
                    } else {
                        Vector2Int squarePosition = GetSquarePosition(e.mousePosition);

                        if (squarePosition.x < 0 || squarePosition.x >= sfMapEditor.size.x || squarePosition.y < 0 || squarePosition.y >= sfMapEditor.size.y) return;

                        GameObject square = GameObject.Find("Square(" + squarePosition.x + "," + squarePosition.y + ")");

                        if (!square) {
                            DrawSquareAndTile(squarePosition.x, squarePosition.y);
                        }
                    }
                } else if (e.isMouse && e.type == EventType.MouseDown && e.button == 0 && (sfSpritePicker.selectedIndex >= 0 || sfMapEditor.useWater)) {
                    e.Use();

                    if (sfMapEditor.useWater && !sfSpritePicker.waterPrefab) {
                        Debug.LogWarning("Trying to draw water but the sprite is null");
                    } else {
                        Square sfSquare = null;

                        if (sfMapEditor.currentSelectMode == MapEditor.SelectMode.Tile) {
                            sfSquare = GetSquareHit();

                            if (sfSquare != null) {
                                DrawOnSquare(sfSquare.gameObject);
                            }
                        }

                        // Will be null if SelectMode.Grid or if the ray hit nothing (meaning targeting an empty square)
                        if (sfSquare == null) {
                            if (sfSpritePicker.isEntity) {
                                Debug.LogWarning("Can't draw an entity on an empty square ; at least one tile is required");
                            } else {
                                Vector2Int squarePosition = GetSquarePosition(e.mousePosition);

                                if (squarePosition.x < 0 || squarePosition.x >= sfMapEditor.size.x || squarePosition.y < 0 || squarePosition.y >= sfMapEditor.size.y) return;

                                GameObject square = GameObject.Find("Square(" + squarePosition.x + "," + squarePosition.y + ")");

                                if (!square) {
                                    DrawSquareAndTile(squarePosition.x, squarePosition.y);
                                } else {
                                    DrawOnSquare(square);
                                }
                            }
                        }
                    }
                }
            }

            // Delete
            if (sfMapEditor.currentMode == MapEditor.Mode.Delete) {
                if (e.isMouse && e.type == EventType.MouseDown && e.button == 0) {
                    e.Use();

                    GameObject tileHit = GetTileHit();

                    if (tileHit.GetComponentInParent<Square>().transform.childCount == 1) {
                        DestroyImmediate(tileHit.GetComponentInParent<Square>().gameObject);
                    } else {
                        DestroyImmediate(tileHit);
                    }
                }
            }

            // Block
            if (sfMapEditor.currentMode == MapEditor.Mode.Block) {
                if (e.isMouse && e.type == EventType.MouseDown && e.button == 0) {
                    e.Use();

                    if (sfMapEditor.currentSelectMode == MapEditor.SelectMode.Tile) {
                        Square squareHit = GetSquareHit();

                        if (squareHit != null) {
                            squareHit.solid = !squareHit.solid;
                        }
                    } else if (sfMapEditor.currentSelectMode == MapEditor.SelectMode.Grid) {
                        Vector2Int squarePosition = GetSquarePosition(e.mousePosition);

                        if (squarePosition.x < 0 || squarePosition.x >= sfMapEditor.size.x || squarePosition.y < 0 || squarePosition.y >= sfMapEditor.size.y) return;

                        GameObject squareHit = GameObject.Find("Square(" + squarePosition.x + "," + squarePosition.y + ")");

                        if (squareHit) {
                            squareHit.GetComponent<Square>().solid = !squareHit.GetComponent<Square>().solid;
                        }
                    }
                }
            }

            if (sfMapEditor.currentMode == MapEditor.Mode.Draw || sfMapEditor.currentMode == MapEditor.Mode.Height || sfMapEditor.currentMode == MapEditor.Mode.Delete
                    || sfMapEditor.currentMode == MapEditor.Mode.Block) {
                Selection.activeGameObject = sfMapEditor.gameObject;
            }
        }

        private Square GetSquareHit() {
            GameObject tileHit = GetTileHit();

            return tileHit ? tileHit.GetComponentInParent<Square>() : null;
        }

        private GameObject GetTileHit() {
            GameObject visibleTileHit = null;

            foreach (RaycastHit2D hit in Physics2D.RaycastAll(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin, Vector2.zero)) {
                GameObject tileOrEntity = hit.collider.gameObject;
                TileContainer tileContainer = tileOrEntity.GetComponentInParent<TileContainer>();

                // Ignore entities
                if (tileContainer == null) continue;

                int hitSortingOrder = tileContainer.GetComponentInParent<Square>().GetComponent<SortingGroup>().sortingOrder;

                // Retrieve the closiest map object, the one we are seeing
                if (visibleTileHit == null || hitSortingOrder > visibleTileHit.GetComponentInParent<Square>().GetComponent<SortingGroup>().sortingOrder) {
                    visibleTileHit = tileOrEntity;
                }
            }

            return visibleTileHit;
        }

        private SpriteRenderer GetHighestTile(Square square) {
            SpriteRenderer highestTile = null;

            SpriteRenderer[] sprites = square.tileContainer.GetComponentsInChildren<SpriteRenderer>();

            foreach (SpriteRenderer sprite in sprites) {
                if (highestTile == null || sprite.sortingOrder > highestTile.sortingOrder) {
                    highestTile = sprite;
                }
            }

            return highestTile;
        }

        private void DrawOnSquare(GameObject square) {
            int highestSortingOrder = 0;
            SpriteRenderer[] sprites = square.GetComponentsInChildren<SpriteRenderer>();

            foreach (SpriteRenderer sprite in sprites) {
                if (sprite.sortingOrder > highestSortingOrder) {
                    highestSortingOrder = sprite.sortingOrder;
                }
            }

            highestSortingOrder++;

            GameObject mapObject = null;

            if (sfSpritePicker.isEntity) {
                mapObject = sfMapEditor.CreateEntity(square.GetComponent<Square>());
            } else {
                mapObject = sfMapEditor.CreateTile(square.GetComponent<Square>(), highestSortingOrder, (float)square.GetComponent<Square>().Height / Globals.PixelsPerUnit);

                if (sfMapEditor.useWater) {
                    square.GetComponent<Square>().solid = true;
                }
            }

            sfMapEditor.undoStack.Push(() => {
                DestroyImmediate(mapObject);

                if (sfMapEditor.useWater && !sfSpritePicker.isEntity) {
                    SpriteRenderer[] underwaterSprites = square.GetComponentsInChildren<SpriteRenderer>();

                    foreach (SpriteRenderer underwaterSprite in underwaterSprites) {
                        underwaterSprite.color = Color.white;
                    }
                }
            });
        }

        private void DrawSquareAndTile(int squareX, int squareY) {
            if (sfMapEditor.useWater) return;

            GameObject square = sfMapEditor.CreateSquare(squareX, squareY);

            sfMapEditor.CreateTile(square.GetComponent<Square>(), 0);

            sfMapEditor.undoStack.Push(() => {
                DestroyImmediate(square);
            });
        }

        private Vector2Int GetSquarePosition(Vector2 mousePosition) {
            Vector3 screenMousePos = new Vector3(mousePosition.x, -mousePosition.y + Camera.current.pixelHeight);
            Vector3 localMousePos = Camera.current.ScreenToWorldPoint(screenMousePos);

            return new Vector2Int(
                Mathf.FloorToInt((localMousePos.x / 2) + localMousePos.y),
                Mathf.FloorToInt(localMousePos.y - (localMousePos.x / 2))
            );
        }
    }
}
