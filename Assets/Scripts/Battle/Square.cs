using UnityEngine;

public class Square : MonoBehaviour {
    public SpriteRenderer sprite;

    // Positionning
    public int x; // X coordinate of the tile inside the board
    public int y; // Y coordinate of the tile inside the board
    public int vOffset;
    public bool start = false; // Starting tile
    public bool solid = false; // Collision detection

    public bool isMouseOver = false;

    void Awake() {
        sprite = GetComponent<SpriteRenderer>();
    }

    // Must be called when the tiles GameObjects are created
    public void Init(RawMap.RawSquare rawSquare, int mapWidth) {
        x = rawSquare.x_map;
        y = rawSquare.y_map;
        vOffset = rawSquare.v_offset;
        start = rawSquare.start;
        solid = rawSquare.solid;

        sprite.sortingOrder = (x + y * mapWidth) * 10;

        transform.position = new Vector3(
            x - y,
            (-(y + x) / 2f) + (vOffset / (sprite.bounds.size.y * Globals.TileHeight / 2)),
            0f
        );
    }

    /**
     * Called by Board
     */
    void MouseEnter() {
        isMouseOver = true;
    }

    /**
     * Called by Board
     */
    void MouseLeave() {
        isMouseOver = false;
    }

    /**
     * Called by Board
     */
    void Click() {}
}
