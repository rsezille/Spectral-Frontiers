using UnityEngine;
using System;
using RawSquare = RawMap.RawSquare;

/**
 * Load the map
 * Compute the mouse position and dispatch events to the first object hit
 */
public class Board : MonoBehaviour {
    private RawMap rawMap;

    private GameObject previousMapObject = null; // Used to detect a mouse leave

    public Transform boardSquares;
    public Square[,] squares;

    /**
     * Compute the current mouse position and dispatch events to the first object hit
     */
    void Update() {
        if (BattleManager.instance.currentTurnStep != BattleManager.TurnStep.Status) {
            Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            GameObject mapObject = GetTouchedGameObject(position);

            if (previousMapObject != null && previousMapObject != mapObject) {
                previousMapObject.SendMessage("MouseLeave");
            }

            if (mapObject != null) {
                if (Input.GetButtonDown(InputBinds.Click)) {
                    mapObject.SendMessage("Click");
                }

                if (mapObject == previousMapObject) {
                    //mapObject.SendMessage("MouseOver");
                } else {
                    mapObject.SendMessage("MouseEnter");
                }
            }

            previousMapObject = mapObject;
        }
    }

    private GameObject GetTouchedGameObject(Vector3 position) {
        GameObject boardItem = null;

        foreach (RaycastHit2D hit in Physics2D.RaycastAll(position, Vector2.zero)) {
            if (hit.collider.tag == "HUD") {
                return null;
            }

            if (hit.collider.tag == "BoardItem") {
                int raySo = hit.collider.gameObject.GetComponent<SpriteRenderer>().sortingOrder;

                // Retrieve the closiest map object, the one we are seeing
                if (boardItem == null || raySo > boardItem.GetComponent<SpriteRenderer>().sortingOrder) {
                    boardItem = hit.collider.gameObject;
                }
            }
        }

        return boardItem;
    }

    public void loadMap(string map) {
        boardSquares = new GameObject("BoardSquares").transform;

        TextAsset jsonMap = Resources.Load("Maps/" + map) as TextAsset;

        if (jsonMap != null) {
            rawMap = JsonUtility.FromJson<RawMap>(jsonMap.text);

            squares = new Square[rawMap.width, rawMap.height];

            foreach (RawSquare rawSquare in rawMap.squares) {
                float x = rawSquare.x_map - rawSquare.y_map; //TODO: test with ymap - xmap
                float y = -(rawSquare.x_map + rawSquare.y_map) / 2f;

                if (!String.IsNullOrEmpty(rawSquare.tile)) {
                    GameObject tile = Resources.Load("Tiles/" + rawSquare.tile) as GameObject;

                    if (tile == null)
                        Debug.LogError("Tile GameObject not found");

                    Square square = Instantiate(tile.GetComponent<Square>(), new Vector3(x, y, 0f), Quaternion.identity) as Square;
                    squares[rawSquare.x_map, rawSquare.y_map] = square;
                    square.Init(rawSquare, rawMap.width);
                    square.transform.SetParent(boardSquares);
                }
            }
        } else {
            Debug.LogError("Map not found! " + map);
        }
    }
}
