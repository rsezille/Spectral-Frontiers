using UnityEngine;
using SpriteGlow;

/**
 * Represent a placed player character on the battlefield
 * Manage graphics and attached components
 */
[RequireComponent(typeof(BoardEntity), typeof(Movable), typeof(Actionable))]
[RequireComponent(typeof(Side), typeof(MouseReactive), typeof(SpriteGlowEffect))]
[RequireComponent(typeof(BoardCharacter))]
public class PlayerCharacter : MonoBehaviour {
    private BattleManager battleManager;

    // Components
    public BoardCharacter boardCharacter;

    private void Awake() {
        battleManager = BattleManager.instance;
        
        boardCharacter = GetComponent<BoardCharacter>();
    }

    private void Start() {
        gameObject.name = boardCharacter.character.name; // To find it inside the editor
    }

    /**
     * Triggered by Board
     */
    public void MouseEnter() {
        battleManager.fightHUD.SquareHovered(boardCharacter.GetSquare());

        if (boardCharacter.outline != null) {
            boardCharacter.outline.enabled = true;
        }
    }

    /**
     * Triggered by Board
     */
    public void MouseLeave() {
        battleManager.fightHUD.SquareHovered(null);

        if ((battleManager.currentBattleStep == BattleManager.BattleStep.Placing && battleManager.placing.GetCurrentPlacingChar().boardCharacter != this
                || battleManager.currentBattleStep == BattleManager.BattleStep.Fight && battleManager.GetSelectedPlayerBoardCharacter() != this) && boardCharacter.outline != null) {
            boardCharacter.outline.enabled = false;
        }
    }

    /**
     * Triggered by Board
     */
    public void Click() {
        if (boardCharacter.side.value == Side.Type.Player) {
            if (battleManager.currentBattleStep == BattleManager.BattleStep.Placing) {
                // Focus the clicked character as the current one to place
                battleManager.placing.SetCurrentPlacingChar(boardCharacter.character);
            } else if (battleManager.currentBattleStep == BattleManager.BattleStep.Fight && battleManager.currentTurnStep == BattleManager.TurnStep.None) {
                battleManager.SetSelectedPlayerBoardCharacter(boardCharacter);
            }
        }
    }
}
