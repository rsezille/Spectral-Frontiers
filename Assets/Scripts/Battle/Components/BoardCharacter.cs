using DG.Tweening;
using SpriteGlow;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/**
 * Represent a board character on the board
 */
[RequireComponent(typeof(BoardEntity), typeof(SpriteRenderer), typeof(Side))]
[RequireComponent(typeof(MouseReactive))]
public class BoardCharacter : MonoBehaviour {
    private BattleManager battleManager;

    public Character character;

    // Components
    private BoardEntity boardEntity;
    private SpriteRenderer sprite;
    private MouseReactive mouseReactive;
    public Side side;
    // Components which can be null
    public SpriteGlowEffect outline;
    public Movable movable;
    public Actionable actionable;
    public AI AI;

    public bool isMoving = false;

    private void Awake() {
        battleManager = BattleManager.instance;

        boardEntity = GetComponent<BoardEntity>();
        sprite = GetComponent<SpriteRenderer>();
        mouseReactive = GetComponent<MouseReactive>();
        side = GetComponent<Side>();
        outline = GetComponent<SpriteGlowEffect>();
        movable = GetComponent<Movable>();
        actionable = GetComponent<Actionable>();
        AI = GetComponent<AI>();


        if (outline) {
            outline.enabled = false;
        }

        mouseReactive.MouseEnter = new UnityEvent();
        mouseReactive.MouseEnter.AddListener(MouseEnter);
        mouseReactive.MouseLeave = new UnityEvent();
        mouseReactive.MouseLeave.AddListener(MouseLeave);
        mouseReactive.Click = new UnityEvent();
        mouseReactive.Click.AddListener(Click);
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

    private void Start() {
        gameObject.name = character.name; // To find it inside the editor
    }

    /**
     * Triggered by Board
     */
    public void MouseEnter() {
        battleManager.fightHUD.SquareHovered(GetSquare());

        if (outline != null) {
            outline.enabled = true;
        }
    }

    /**
     * Triggered by Board
     */
    public void MouseLeave() {
        battleManager.fightHUD.SquareHovered(null);

        if ((battleManager.currentBattleStep == BattleManager.BattleStep.Placing && battleManager.placing.GetCurrentPlacingChar().boardCharacter != this
                || battleManager.currentBattleStep == BattleManager.BattleStep.Fight && battleManager.GetSelectedPlayerBoardCharacter() != this) && outline != null) {
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
                battleManager.placing.SetCurrentPlacingChar(character);
            } else if (battleManager.currentBattleStep == BattleManager.BattleStep.Fight && battleManager.currentTurnStep == BattleManager.TurnStep.None) {
                battleManager.SetSelectedPlayerBoardCharacter(this);
            }
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
            if (movable.movementPoints <= 0) break;
            if (!path.steps[i].IsNotBlocking()) break;

            movable.movementPoints--;

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

            SetSquare(path.steps[i]);
        }

        isMoving = false;

        if (battleManager.currentTurnStep != BattleManager.TurnStep.Enemy) {
            //bm.fightHUD.SetFightMenuActive(true);

            battleManager.EnterTurnStepNone();
        }
    }
}
