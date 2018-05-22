using UnityEngine;
using UnityEngine.UI;

/**
 * Represent a placed character (ally or enemy) on the battlefield
 * Manage graphics, movements, the Character data, etc.
 */
public class BoardChar : MonoBehaviour {
    public enum Side {
        Ally, Enemy
    }

    public Character character;

    public SpriteRenderer sprite;
    public Side side;
    public Square square; // The square the character is placed on

    public BattleManager battleManager; // Shortcut for BattleManager.instance

    public int movementPoints; // At the beginning of each turn, movementPoints = character.movementPoints
    public int fightMovements; // At the beginning of each turn, fightMovement = character.movementTokens
    public int fightActions; // At the beginning of each turn, fightAction = character.actionTokens

    void Awake() {
        battleManager = BattleManager.instance;

        sprite = GetComponent<SpriteRenderer>();
    }

    void Start() {
        gameObject.name = character.name; // To find it inside the editor
    }

    void Update() {}

    /**
     * Called by Board
     */
    void MouseEnter() {}

    /**
     * Called by Board
     */
    void MouseLeave() {}

    /**
     * Called by Board
     */
    void Click() {}

    public void SetSquare(Square s) {
        if (square != null) {
            square.boardChar = null;
        }

        square = s;

        if (square != null) {
            square.boardChar = this;
            transform.position = square.transform.position;
            SetSortingOrder(square.sprite.sortingOrder + 1);
        }
    }

    private void SetSortingOrder(int sortingOrder) {
        sprite.sortingOrder = sortingOrder;
    }
}
