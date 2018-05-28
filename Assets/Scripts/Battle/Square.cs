using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D), typeof(MouseReactive))]
public class Square : MonoBehaviour {
    public SpriteRenderer sprite;

    // Positionning
    public int x; // X coordinate of the tile inside the board
    public int y; // Y coordinate of the tile inside the board
    public int vOffset;
    public bool start = false; // Starting tile
    public bool solid = false; // Collision detection

    // Colors
    public static Color defaultColor = new Color(1f, 1f, 1f, 0f);
    public static Color overingColor = new Color(0f, 0f, 0f, 0.2f);
    public static Color placingStartMouseOverColor = new Color(0.14f, 0.36f, 0.92f, 0.62f);

    public bool isMouseOver = false;

    public BoardEntity boardEntity;

    private void Awake() {
        sprite = GetComponent<SpriteRenderer>();

        BattleManager.instance.OnEnterPlacing += OnEnterPlacing;
    }

    // Must be called when the tiles GameObjects are created
    public void Init(RawMap.RawSquare rawSquare, int mapWidth) {
        x = rawSquare.x_map;
        y = rawSquare.y_map;
        vOffset = rawSquare.v_offset;
        start = rawSquare.start;
        solid = rawSquare.solid;

        sprite.sortingOrder = (x + y * mapWidth) * 10;
        sprite.color = defaultColor;

        transform.position = new Vector3(
            x - y,
            (-(y + x) / 2f) + (vOffset / (sprite.bounds.size.y * Globals.TileHeight / 2)),
            0f
        );
    }

    private void OnEnterPlacing() {
        if (start) {
            StartCoroutine("IsStartingSquare", placingStartMouseOverColor);
        }
    }

    IEnumerator IsStartingSquare(Color targetColor) {
        float initialFade = 0.30f;
        float maxFade = 0.78f;
        float smoothness = 0.02f;
        float duration = 1f;
        float progress = initialFade;
        float increment = smoothness / duration;

        while (BattleManager.instance.currentBattleStep == BattleManager.BattleStep.Placing) {
            if (isMouseOver) {
                sprite.color = placingStartMouseOverColor;
            } else {
                sprite.color = Color.Lerp(new Color(placingStartMouseOverColor.r, placingStartMouseOverColor.g, placingStartMouseOverColor.b, 0f), targetColor, progress);
            }

            if (progress > maxFade || progress < initialFade) {
                increment = -increment;
            }

            progress += increment;

            yield return new WaitForSeconds(smoothness);
        }

        sprite.color = defaultColor;
    }

    /**
     * Triggered by Board
     */
    public void MouseEnter() {
        isMouseOver = true;
        sprite.color = overingColor;
    }

    /**
     * Triggered by Board
     */
    public void MouseLeave() {
        isMouseOver = false;
        sprite.color = defaultColor;
    }

    /**
     * Triggered by Board
     */
    public void Click() {
        switch (BattleManager.instance.currentBattleStep) {
            case BattleManager.BattleStep.Placing:
                if (start && boardEntity == null) {
                    BattleManager.instance.placing.PlaceMapChar(this);
                }

                break;
        }
    }
}
