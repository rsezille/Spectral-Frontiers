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
    public SpriteRenderer outline;
    public Side side;
    public Square square; // The square the character is placed on

    // BoardChar HUD
    public GameObject charHUDTransform;
    public Text characterName;
    public Image healthBar;

    public BattleManager battleManager; // Shortcut for BattleManager.instance

    public int movementPoints; // At the beginning of each turn, movementPoints = character.movementPoints
    public int fightMovements; // At the beginning of each turn, fightMovement = character.movementTokens
    public int fightActions; // At the beginning of each turn, fightAction = character.actionTokens

    void Awake() {
        battleManager = BattleManager.instance;

        sprite = GetComponent<SpriteRenderer>();

        // Disable the glow outline
        outline.gameObject.SetActive(false);

        // Put the HUD above the sprite
        charHUDTransform.transform.position += new Vector3(0f, sprite.bounds.size.y + 0.3f);
    }

    void Start() {
        gameObject.name = character.name; // To find it inside the editor

        characterName.text = character.name;

        healthBar.transform.localScale = new Vector3(
            (float)character.GetCurrentHP() / (float)character.GetMaxHP(),
            healthBar.transform.localScale.y,
            healthBar.transform.localScale.z
        );
    }

    void Update() {
        healthBar.transform.localScale = new Vector3(
            (float)character.GetCurrentHP() / (float)character.GetMaxHP(),
            healthBar.transform.localScale.y,
            healthBar.transform.localScale.z
        );
    }

    /**
     * Called by Board
     */
    void MouseEnter() {
        outline.gameObject.SetActive(true);
    }

    /**
     * Called by Board
     */
    void MouseLeave() {
        if (battleManager.currentBattleStep == BattleManager.BattleStep.Placing && battleManager.placing.GetCurrentPlacingChar().boardChar != this) {
            outline.gameObject.SetActive(false);
        }
    }

    /**
     * Called by Board
     */
    void Click() {
        if (side == Side.Ally) {
            if (battleManager.currentBattleStep == BattleManager.BattleStep.Placing) {
                // Focus the clicked character as the current one to place
                battleManager.placing.SetCurrentPlacingChar(this.character);
            }
        }
    }

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
        outline.sortingOrder = sortingOrder;
        charHUDTransform.transform.GetComponentInChildren<Canvas>().sortingOrder = sortingOrder;
    }
}
