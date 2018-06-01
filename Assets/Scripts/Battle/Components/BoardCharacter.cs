using DG.Tweening;
using SpriteGlow;
using System.Collections;
using UnityEngine;

/**
 * Represent a board character on the board
 */
[RequireComponent(typeof(BoardEntity), typeof(SpriteRenderer), typeof(Side))]
public class BoardCharacter : MonoBehaviour {
    private BattleManager battleManager;

    public Character character;

    // Components
    private BoardEntity boardEntity;
    private SpriteRenderer sprite;
    public Side side;
    // Components which can be null
    public SpriteGlowEffect outline;
    public Movable movable;
    public Actionable actionable;

    private bool isMoving = false;

    private void Awake() {
        battleManager = BattleManager.instance;

        boardEntity = GetComponent<BoardEntity>();
        sprite = GetComponent<SpriteRenderer>();
        side = GetComponent<Side>();
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

    public void Move(Path p, bool cameraFollow = false) {
        if (movable != null && movable.CanMove()) {
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

            if (path.steps[i].x - GetSquare().x > 0 || path.steps[i].y - GetSquare().y > 0) {
                SetSortingOrder(path.steps[i].sprite.sortingOrder + 1);
            }

            yield return characterAnimation.WaitForPosition(duration * 3 / 4);

            if (path.steps[i].x - GetSquare().x < 0 || path.steps[i].y - GetSquare().y < 0) {
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
