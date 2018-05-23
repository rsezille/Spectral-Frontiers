﻿using UnityEngine;
using UnityEngine.UI;

/**
 * Represent a placed character (ally or enemy) on the battlefield
 * Manage graphics, movements, the Character data, etc.
 */
[RequireComponent(typeof(BoardEntity), typeof(Movable), typeof(Actionable))]
[RequireComponent(typeof(Side))]
public class BoardChar : MonoBehaviour {
    public Character character;

    public SpriteRenderer sprite;
    public SpriteRenderer outline;

    // BoardChar HUD
    public GameObject charHUDTransform;
    public Text characterName;
    public Image healthBar;

    public BattleManager battleManager; // Shortcut for BattleManager.instance

    public BoardEntity boardEntity;
    public Side side;

    void Awake() {
        boardEntity = GetComponent<BoardEntity>();
        side = GetComponent<Side>();

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
        outline.sortingOrder = sortingOrder;
        charHUDTransform.transform.GetComponentInChildren<Canvas>().sortingOrder = sortingOrder;
    }
}
