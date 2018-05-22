using UnityEngine;
using System;
using RawSquare = RawMap.RawSquare;

/**
 * Load the map
 */
public class Board : MonoBehaviour {
    private RawMap rawMap;

    public Square[,] squares;
    public Transform boardSquares;

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
