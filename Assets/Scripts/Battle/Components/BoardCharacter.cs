using SpriteGlow;
using UnityEngine;

[RequireComponent(typeof(BoardEntity), typeof(SpriteRenderer))]
public class BoardCharacter : MonoBehaviour {
    public Character character;

    // Components
    private BoardEntity boardEntity;
    private SpriteRenderer sprite;
    // Components which can be null
    public SpriteGlowEffect outline;
    public Movable movable;
    public Actionable actionable;

    private void Awake() {
        boardEntity = GetComponent<BoardEntity>();
        sprite = GetComponent<SpriteRenderer>();
        outline = GetComponent<SpriteGlowEffect>();
        movable = GetComponent<Movable>();
        actionable = GetComponent<Actionable>();

        if (outline) {
            outline.enabled = false;
        }
    }

    public Square GetSquare() {
        return boardEntity.square;
    }

    public void SetSquare(Square targetedSquare) {
        if (boardEntity.square != null) {
            boardEntity.square.boardEntity = null;
        }

        boardEntity.square = targetedSquare;

        if (targetedSquare != null) {
            targetedSquare.boardEntity = boardEntity;
            transform.position = targetedSquare.transform.position;
            SetSortingOrder(targetedSquare.sprite.sortingOrder + 1);
        }
    }

    public void SetSortingOrder(int sortingOrder) {
        sprite.sortingOrder = sortingOrder;

        Component[] HUDs = transform.GetComponentsInChildren<Canvas>();

        foreach (Canvas canvas in HUDs) {
            canvas.sortingOrder = sortingOrder;
        }
    }

    public void NewTurn() {
        if (actionable != null) {
            actionable.actionTokens = character.actionTokens;
        }

        if (movable != null) {
            movable.movementTokens = character.movementTokens;
            movable.movementPoints = character.movementPoints;
        }
    }

    public bool IsDead() {
        return character.GetCurrentHP() <= 0;
    }
}
