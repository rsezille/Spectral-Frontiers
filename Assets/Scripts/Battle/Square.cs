﻿using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D), typeof(MouseReactive))]
public class Square : MonoBehaviour {
    private BattleManager battleManager;
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
    public static Color placingStartColor = new Color(0.14f, 0.36f, 0.92f, 0.62f);
    public static Color movementColor = new Color(0.12f, 0.57f, 0.2f, 0.63f);

    public bool isMouseOver = false;

    public BoardEntity boardEntity;

    public bool isMovementMarked = false;

    private void Awake() {
        battleManager = BattleManager.instance;

        sprite = GetComponent<SpriteRenderer>();

        battleManager.OnEnterPlacing += OnEnterPlacing;
        battleManager.OnLeavingMove += OnLeavingMove;
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
            StartCoroutine("IsStartingSquare", placingStartColor);
        }
    }

    private void OnLeavingMove() {
        isMovementMarked = false;
        sprite.color = defaultColor;
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
                sprite.color = placingStartColor;
            } else {
                sprite.color = Color.Lerp(new Color(placingStartColor.r, placingStartColor.g, placingStartColor.b, 0f), targetColor, progress);
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
        battleManager.fightHUD.SquareHovered(this);
        isMouseOver = true;
        sprite.color = overingColor;

        if (battleManager.currentTurnStep == BattleManager.TurnStep.Move && isMovementMarked) {
            sprite.color = new Color(movementColor.r, movementColor.g + 0.2f, movementColor.b, movementColor.a);
        }
    }

    /**
     * Triggered by Board
     */
    public void MouseLeave() {
        battleManager.fightHUD.SquareHovered(null);
        isMouseOver = false;
        sprite.color = defaultColor;

        if (battleManager.currentTurnStep == BattleManager.TurnStep.Move && isMovementMarked) {
            sprite.color = movementColor;
        }
    }

    /**
     * Triggered by Board
     */
    public void Click() {
        switch (battleManager.currentBattleStep) {
            case BattleManager.BattleStep.Placing:
                if (start && IsNotBlocking()) {
                    BattleManager.instance.placing.PlaceMapChar(this);
                }

                break;
            case BattleManager.BattleStep.Fight:
                if (isMovementMarked && battleManager.currentTurnStep == BattleManager.TurnStep.Move) {
                    Path p = battleManager.board.pathFinder.FindPath(
                        battleManager.GetSelectedBoardChar().GetSquare().x,
                        battleManager.GetSelectedBoardChar().GetSquare().y,
                        this.x,
                        this.y
                    );

                    if (p != null) {
                        battleManager.GetSelectedBoardChar().Move(p, true);
                    }
                }

                break;
        }
    }

    public void MarkForMovement() {
        isMovementMarked = true;

        sprite.color = movementColor;
    }

    public bool IsNotBlocking() {
        return boardEntity == null;
    }
}
