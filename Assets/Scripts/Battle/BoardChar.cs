using UnityEngine;
using System.Collections;
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
    public Movable movable;
    public Actionable actionable;

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

    public void NewTurn() {
        actionable.actionTokens = character.actionTokens;
        movable.movementTokens = character.movementTokens;
        movable.movementPoints = character.movementPoints;
    }

    public void Move(Path p, bool cameraFollow = false) {
        if (movable.CanMove()) {
            StartCoroutine(MoveThroughPath(p, cameraFollow));
            movable.movementTokens--;
        }
    }

    IEnumerator MoveThroughPath(Path path, bool cameraFollow = false) {
        if (cameraFollow) {
            //battleManager.battleCamera.forceMoving = true;
        }

        //isMoving = true;
        int current = 0;
        Vector3 initialPos = this.transform.position;
        float frac = 0f;

        if (battleManager.currentTurnStep != BattleManager.TurnStep.Enemy) {
            //battleManager.fightHUD.SetFightMenuActive(false);
        }

        while (this.transform.position != path.GetStep(path.steps.Count - 1).transform.position
               && movable.movementPoints > 0
               && path.GetStep(current).IsNotBlocking()) {
            this.transform.position = Vector3.Lerp(initialPos, path.GetStep(current).transform.position, frac);

            if (cameraFollow) {
                /*battleManager.battleCamera.SetTarget(new Vector3(
                    this.transform.position.x,
                    this.transform.position.y,
                    battleManager.battleCamera.transform.position.z));*/
                //battleManager.battleCamera.isAutoMoving = true;
            }

            if ((frac >= 0.25 && (path.GetStep(current).x - boardEntity.square.x > 0 || path.GetStep(current).y - boardEntity.square.y > 0)) ||
                    (frac >= 0.75 && (path.GetStep(current).x - boardEntity.square.x < 0 || path.GetStep(current).y - boardEntity.square.y < 0))) {
                SetSortingOrder(path.GetStep(current).sprite.sortingOrder + 1);
            }

            if (frac >= 1) {
                this.SetSquare(path.GetStep(current));
                initialPos = this.transform.position;
                current++;
                frac = 0f;
                movable.movementPoints--;
            }

            frac += 1f * Time.deltaTime;
            yield return null;
        }

        if (cameraFollow) {
            //BattleManager.cam.forceMoving = false;
        }

        //isMoving = false;
        if (battleManager.currentTurnStep != BattleManager.TurnStep.Enemy) {
            //bm.fightHUD.SetFightMenuActive(true);

            battleManager.EnterTurnStepNone();
        }
    }
}
