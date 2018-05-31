using UnityEngine;
using System.Collections;
using SpriteGlow;
using DG.Tweening;

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

    // Components
    private BoardEntity boardEntity;
    public Side side;
    public SpriteGlowEffect outline;
    public Movable movable;
    public Actionable actionable;

    public bool isMoving = false;

    private void Awake() {
        boardEntity = GetComponent<BoardEntity>();
        side = GetComponent<Side>();
        outline = GetComponent<SpriteGlowEffect>();
        movable = GetComponent<Movable>();
        actionable = GetComponent<Actionable>();

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
        battleManager.fightHUD.SetSelectedSquare(this.boardEntity.square);
        outline.enabled = true;
    }

    /**
     * Triggered by Board
     */
    public void MouseLeave() {
        battleManager.fightHUD.SetSelectedSquare(null);

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
        actionable.actionTokens = character.actionTokens;
        movable.movementTokens = character.movementTokens;
        movable.movementPoints = character.movementPoints;
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

        Tween cameraAnimation = battleManager.battleCamera.transform.DOMove(new Vector3(transform.position.x, transform.position.y, battleManager.battleCamera.transform.position.z), 0.5f);
        yield return cameraAnimation.WaitForCompletion();

        for (int i = 0; i < path.steps.Count; i++) {
            Tween characterAnimation = this.transform.DOMove(path.steps[i].transform.position, duration).SetEase(Ease.Linear);
            cameraAnimation = battleManager.battleCamera.transform.DOMove(new Vector3(
                path.steps[i].transform.position.x,
                path.steps[i].transform.position.y,
                battleManager.battleCamera.transform.position.z), duration).SetEase(Ease.Linear);

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
}
