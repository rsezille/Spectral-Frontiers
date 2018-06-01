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
    public SpriteRenderer sprite;

    public BattleManager battleManager; // Shortcut for BattleManager.instance

    // Components
    private BoardEntity boardEntity;
    public Side side;
    public SpriteGlowEffect outline;
    public Movable movable;
    public Actionable actionable;
    public BoardCharacter boardCharacter;

    public bool isMoving = false;

    private void Awake() {
        boardEntity = GetComponent<BoardEntity>();
        side = GetComponent<Side>();
        outline = GetComponent<SpriteGlowEffect>();
        movable = GetComponent<Movable>();
        actionable = GetComponent<Actionable>();
        boardCharacter = GetComponent<BoardCharacter>();

        battleManager = BattleManager.instance;

        sprite = GetComponent<SpriteRenderer>();
        
        outline.enabled = false;
    }

    private void Start() {
        gameObject.name = boardCharacter.character.name; // To find it inside the editor
    }

    /**
     * Triggered by Board
     */
    public void MouseEnter() {
        battleManager.fightHUD.SquareHovered(this.boardEntity.square);
        outline.enabled = true;
    }

    /**
     * Triggered by Board
     */
    public void MouseLeave() {
        battleManager.fightHUD.SquareHovered(null);

        if (battleManager.currentBattleStep == BattleManager.BattleStep.Placing && battleManager.placing.GetCurrentPlacingChar().playerCharacter != this
                || battleManager.currentBattleStep == BattleManager.BattleStep.Fight && battleManager.GetSelectedPlayerCharacter() != this) {
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
                battleManager.placing.SetCurrentPlacingChar(boardCharacter.character);
            } else if (battleManager.currentBattleStep == BattleManager.BattleStep.Fight && battleManager.currentTurnStep == BattleManager.TurnStep.None) {
                battleManager.SetSelectedPlayerCharacter(this);
            }
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

    private void SetSortingOrder(int sortingOrder) {
        sprite.sortingOrder = sortingOrder;

        Component[] HUDs = transform.GetComponentsInChildren<Canvas>();

        foreach (Canvas canvas in HUDs) {
            canvas.sortingOrder = sortingOrder;
        }
    }

    public void NewTurn() {
        actionable.actionTokens = boardCharacter.character.actionTokens;
        movable.movementTokens = boardCharacter.character.movementTokens;
        movable.movementPoints = boardCharacter.character.movementPoints;
    }

    public void Move(Path p, bool cameraFollow = false) {
        if (movable.CanMove()) {
            StartCoroutine(MoveThroughPath(p));
            movable.movementTokens--;
        }
    }

    IEnumerator MoveThroughPath(Path path) {
        isMoving = true;
        float duration = 0.5f;

        Tween cameraAnimation = battleManager.battleCamera.SetPosition(this, true, duration);
        yield return cameraAnimation.WaitForCompletion();

        // Check at 25% and 75% of each square the sorting order of the BoardChar to set the correct one
        for (int i = 0; i < path.steps.Count; i++) {
            Tween characterAnimation = this.transform.DOMove(path.steps[i].transform.position, duration).SetEase(Ease.Linear);
            cameraAnimation = battleManager.battleCamera.SetPosition(path.steps[i], true, duration, Ease.Linear);

            yield return characterAnimation.WaitForPosition(duration / 4);

            if (path.steps[i].x - boardEntity.square.x > 0 || path.steps[i].y - boardEntity.square.y > 0) {
                SetSortingOrder(path.steps[i].sprite.sortingOrder + 1);
            }

            yield return characterAnimation.WaitForPosition(duration * 3 / 4);

            if (path.steps[i].x - boardEntity.square.x < 0 || path.steps[i].y - boardEntity.square.y < 0) {
                SetSortingOrder(path.steps[i].sprite.sortingOrder + 1);
            }

            yield return characterAnimation.WaitForCompletion();
        }

        SetSquare(path.steps[path.steps.Count - 1]);

        isMoving = false;

        if (battleManager.currentTurnStep != BattleManager.TurnStep.Enemy) {
            //bm.fightHUD.SetFightMenuActive(true);

            battleManager.EnterTurnStepNone();
        }
    }

    public bool IsDead() {
        return boardCharacter.character.GetCurrentHP() <= 0;
    }
}
