using UnityEngine;
using System.Collections;
using SpriteGlow;
using DG.Tweening;

/**
 * Represent a placed player character on the battlefield
 * Manage graphics, movements, attached components, etc.
 */
[RequireComponent(typeof(BoardEntity), typeof(Movable), typeof(Actionable))]
[RequireComponent(typeof(Side), typeof(MouseReactive), typeof(SpriteGlowEffect))]
[RequireComponent(typeof(BoardCharacter))]
public class PlayerCharacter : MonoBehaviour {
    public BattleManager battleManager; // Shortcut for BattleManager.instance

    // Components
    
    public Side side;
    public BoardCharacter boardCharacter;

    public bool isMoving = false;

    private void Awake() {
        side = GetComponent<Side>();
        boardCharacter = GetComponent<BoardCharacter>();

        battleManager = BattleManager.instance;
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
        if (side.value == Side.Type.Player) {
            if (battleManager.currentBattleStep == BattleManager.BattleStep.Placing) {
                // Focus the clicked character as the current one to place
                battleManager.placing.SetCurrentPlacingChar(boardCharacter.character);
            } else if (battleManager.currentBattleStep == BattleManager.BattleStep.Fight && battleManager.currentTurnStep == BattleManager.TurnStep.None) {
                battleManager.SetSelectedPlayerBoardCharacter(boardCharacter);
            }
        }
    }

    public void Move(Path p, bool cameraFollow = false) {
        if (boardCharacter.movable != null && boardCharacter.movable.CanMove()) {
            StartCoroutine(MoveThroughPath(p));
            boardCharacter.movable.movementTokens--;
        }
    }

    IEnumerator MoveThroughPath(Path path) {
        isMoving = true;
        float duration = 0.5f;

        Tween cameraAnimation = battleManager.battleCamera.SetPosition(boardCharacter, true, duration);
        yield return cameraAnimation.WaitForCompletion();

        // Check at 25% and 75% of each square the sorting order of the BoardChar to set the correct one
        for (int i = 0; i < path.steps.Count; i++) {
            Tween characterAnimation = this.transform.DOMove(path.steps[i].transform.position, duration).SetEase(Ease.Linear);
            cameraAnimation = battleManager.battleCamera.SetPosition(path.steps[i], true, duration, Ease.Linear);

            yield return characterAnimation.WaitForPosition(duration / 4);

            if (path.steps[i].x - boardCharacter.GetSquare().x > 0 || path.steps[i].y - boardCharacter.GetSquare().y > 0) {
                boardCharacter.SetSortingOrder(path.steps[i].sprite.sortingOrder + 1);
            }

            yield return characterAnimation.WaitForPosition(duration * 3 / 4);

            if (path.steps[i].x - boardCharacter.GetSquare().x < 0 || path.steps[i].y - boardCharacter.GetSquare().y < 0) {
                boardCharacter.SetSortingOrder(path.steps[i].sprite.sortingOrder + 1);
            }

            yield return characterAnimation.WaitForCompletion();
        }

        boardCharacter.SetSquare(path.steps[path.steps.Count - 1]);

        isMoving = false;

        if (battleManager.currentTurnStep != BattleManager.TurnStep.Enemy) {
            //bm.fightHUD.SetFightMenuActive(true);

            battleManager.EnterTurnStepNone();
        }
    }
}
