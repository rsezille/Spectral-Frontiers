using UnityEngine;
using UnityEngine.UI;
using SpriteGlow;

/**
 * Represent a placed character (ally or enemy) on the battlefield
 * Manage graphics, movements, the Character data, etc.
 */
[RequireComponent(typeof(BoardEntity), typeof(Movable), typeof(Actionable))]
[RequireComponent(typeof(Side), typeof(MouseReactive), typeof(SpriteGlowEffect))]
public class BoardChar : MonoBehaviour {
    public Character character;

    public SpriteRenderer sprite;

    public BattleManager battleManager; // Shortcut for BattleManager.instance

    public BoardEntity boardEntity;
    public Side side;
    public SpriteGlowEffect outline;

    private void Awake() {
        boardEntity = GetComponent<BoardEntity>();
        side = GetComponent<Side>();
        outline = GetComponent<SpriteGlowEffect>();

        battleManager = BattleManager.instance;

        sprite = GetComponent<SpriteRenderer>();

        // Disable the glow outline
        outline.enabled = false;
    }

    private void Start() {
        gameObject.name = character.name; // To find it inside the editor
    }

    /**
     * Triggered by Board
     */
    public void MouseEnter() {
        outline.enabled = true;
    }

    /**
     * Triggered by Board
     */
    public void MouseLeave() {
        if (battleManager.currentBattleStep == BattleManager.BattleStep.Placing && battleManager.placing.GetCurrentPlacingChar().boardChar != this) {
            outline.enabled = false;
        }
    }

    /**
     * Triggered by Board
     */
    public void Click() {
        if (side.value == Side.Type.Player) {
            if (battleManager.currentBattleStep == BattleManager.BattleStep.Placing) {
                // Focus the clicked character as the current one to place
                battleManager.placing.SetCurrentPlacingChar(this.character);
            }
        }
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

    private void SetSortingOrder(int sortingOrder) {
        sprite.sortingOrder = sortingOrder;

        Component[] HUDs = transform.GetComponentsInChildren<Canvas>();

        foreach (Canvas canvas in HUDs) {
            canvas.sortingOrder = sortingOrder;
        }
    }
}
