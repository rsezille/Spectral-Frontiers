using UnityEngine;
using System;
using RawSquare = RawMap.RawSquare;

/**
 * Load the map
 * Compute the mouse position and dispatch events to the first object hit
 */
public class Board : MonoBehaviour {
    private RawMap rawMap;

    private MouseReactive previousMouseEntity = null; // Used to detect a mouse leave

    public Transform boardSquaresTransform;
    public Square[,] squares;
    public PathFinder pathFinder;

    public string mapName;
    public int width { get; private set; }
    public int height { get; private set; }

    /**
     * Compute the current mouse position and dispatch events to the first object hit
     */
    private void Update() {
        if (BattleManager.instance.currentTurnStep != BattleManager.TurnStep.Status) {
            Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            MouseReactive entity = GetTouchedEntity(position);

            if (previousMouseEntity != null && previousMouseEntity != entity) {
                previousMouseEntity.MouseLeave.Invoke();
            }

            if (entity != null) {
                if (Input.GetButtonDown(InputBinds.Click)) {
                    entity.Click.Invoke();
                }

                if (entity == previousMouseEntity) {
                    //entity.MouseOver.Invoke();
                } else {
                    entity.MouseEnter.Invoke();
                }
            }

            previousMouseEntity = entity;
        }
    }

    private MouseReactive GetTouchedEntity(Vector3 position) {
        MouseReactive entity = null;

        foreach (RaycastHit2D hit in Physics2D.RaycastAll(position, Vector2.zero)) {
            if (hit.collider.tag == "HUD") {
                return null;
            }

            MouseReactive mr = hit.collider.gameObject.GetComponent<MouseReactive>();

            // Only trigger game objects that react to the mouse
            if (mr) {
                int raySo = hit.collider.gameObject.GetComponent<SpriteRenderer>().sortingOrder;

                // Retrieve the closiest map object, the one we are seeing
                if (entity == null || raySo > entity.GetComponent<SpriteRenderer>().sortingOrder) {
                    entity = mr;
                }
            }
        }

        return entity;
    }

    public void loadMap(string map) {
        boardSquaresTransform = new GameObject("Squares").transform;
        boardSquaresTransform.SetParent(this.transform);

        TextAsset jsonMap = Resources.Load("Maps/" + map) as TextAsset;

        if (jsonMap != null) {
            rawMap = JsonUtility.FromJson<RawMap>(jsonMap.text);
            mapName = rawMap.name;
            width = rawMap.width;
            height = rawMap.height;

            squares = new Square[width, height];

            foreach (RawSquare rawSquare in rawMap.squares) {
                float x = rawSquare.x_map - rawSquare.y_map; //TODO: test with ymap - xmap
                float y = -(rawSquare.x_map + rawSquare.y_map) / 2f;

                if (!String.IsNullOrEmpty(rawSquare.tile)) {
                    GameObject tile = Resources.Load("Tiles/" + rawSquare.tile) as GameObject; //TODO: Maybe preload all resources when launching the game rather than at runtime?

                    if (tile == null)
                        Debug.LogError("Tile GameObject not found");

                    Square square = Instantiate(tile.GetComponent<Square>(), new Vector3(x, y, 0f), Quaternion.identity) as Square;
                    squares[rawSquare.x_map, rawSquare.y_map] = square;
                    square.Init(rawSquare, width);
                    square.transform.SetParent(boardSquaresTransform);
                }
            }

            pathFinder = new PathFinder(this, this.width + this.height);
        } else {
            Debug.LogError("Map not found! " + map);
        }
    }

    /**
     * Return the square according to x and y coordinates ; null if coordinates are outside the board
     */
    public Square GetSquare(int x, int y) {
        if (x > width - 1 || x < 0 || y > height - 1 || y < 0) {
            return null;
        }

        return squares[x, y];
    }
}
