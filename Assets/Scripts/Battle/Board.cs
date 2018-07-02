using UnityEngine;

/**
 * Load the map
 * Compute the mouse position and dispatch events to the first object hit
 */
public class Board : MonoBehaviour {
    private RawMap rawMap;
    private Square[] squares;

    private MouseReactive previousMouseEntity = null; // Used to detect a mouse leave

    public Transform boardSquaresTransform;
    public PathFinder pathFinder;

    public string mapName;
    public int width { get; private set; }
    public int height { get; private set; }

    private void Awake() {
        width = 0;
        height = 0;
    }

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
            if (mr && entity == null) {
                entity = mr;
                break;
            }
        }

        return entity;
    }

    public void loadMap(string mapName) {
        GameObject mapGameObject = Resources.Load("Maps/" + mapName) as GameObject;

        if (mapGameObject != null) {
            GameObject map = Instantiate(mapGameObject, Vector3.zero, Quaternion.identity) as GameObject;

            Square[] mapSquares = map.GetComponentsInChildren<Square>();

            width = 0;
            height = 0;

            foreach (Square mapSquare in mapSquares) {
                if (mapSquare.x > width) width = mapSquare.x;
                if (mapSquare.y > height) height = mapSquare.y;
            }

            width++; // Squares start from index 0
            height++; // Squares start from index 0

            squares = new Square[width * height];

            foreach (Square mapSquare in mapSquares) {
                int squareIndex = mapSquare.x + (mapSquare.y * width);

                squares[squareIndex] = mapSquare;
            }

            squares[PositionToIndexSquare(5, 7)].markType = Square.MarkType.Placing; // TODO
            squares[PositionToIndexSquare(0, 9)].markType = Square.MarkType.Placing; // TODO
            squares[PositionToIndexSquare(0, 8)].markType = Square.MarkType.Placing; // TODO

            pathFinder = new PathFinder(this, this.width + this.height);
        } else {
            Debug.LogError("Map not found! " + mapName);
        }
    }

    /**
     * Return the square according to x and y coordinates ; null if coordinates are outside the board
     */
    public Square GetSquare(int x, int y) {
        if (x > width - 1 || x < 0 || y > height - 1 || y < 0) {
            return null;
        }

        return squares[PositionToIndexSquare(x, y)];
    }

    public int PositionToIndexSquare(int x, int y) {
        return x + (y * width);
    }
}
