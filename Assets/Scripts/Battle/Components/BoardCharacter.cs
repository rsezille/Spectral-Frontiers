using DG.Tweening;
using SF;
using SpriteGlow;
using System.Collections;
using UnityEngine;

/**
 * Represent a board character on the board
 * The GameObject is placed in the attached Square inside the EntityContainer
 */
[RequireComponent(typeof(BoardEntity), typeof(Side))]
public class BoardCharacter : MonoBehaviour {
    public enum Direction {
        North, East, South, West // Rotate a 2D plan by 90 degrees clockwise
    };

    private BattleManager battleManager;

    public Character character;
    private GameObject spriteContainer;

    // Components
    private BoardEntity boardEntity;
    private SpriteRenderer sprite;
    private Animator animator;
    [HideInInspector]
    public Side side;
    // Components which can be null
    [HideInInspector]
    public SpriteGlowEffect outline;
    [HideInInspector]
    public Movable movable;
    [HideInInspector]
    public Actionable actionable;
    [HideInInspector]
    public AI AI;

    [Tooltip("Do not touch this")]
    public bool isMoving = false;

    private Direction _direction = Direction.South;
    public Direction direction {
        get {
            return _direction;
        }

        set {
            _direction = value;

            switch (value) {
                case Direction.South:
                    animator.Play("Idle_South");
                    break;
                case Direction.North:
                    animator.Play("Idle_North");
                    break;
                case Direction.East:
                    animator.Play("Idle_East");
                    break;
                case Direction.West:
                    animator.Play("Idle_West");
                    break;
            }
        }
    }

    private void Awake() {
        battleManager = BattleManager.instance;

        battleManager.OnCheckSemiTransparent += OnCheckSemiTransparent;

        side = GetComponent<Side>();

        // Enemies have their spriteContainer already linked to the GameObject
        if (side.value == Side.Type.Player) {
            // TODO [ALPHA] Replace "Hero" by the main character or the job
            spriteContainer = Instantiate(Resources.Load<GameObject>("CharacterSprites/Hero"), transform.position, Quaternion.identity) as GameObject;
            spriteContainer.transform.SetParent(transform);
        } else {
            // TODO [ALPHA] Replace this by spriteContainer = GetComponentInChildren<Transform>().gameObject;
            spriteContainer = gameObject;
        }

        animator = spriteContainer.GetComponent<Animator>();
        sprite = spriteContainer.GetComponent<SpriteRenderer>();
        sprite.sortingOrder = 1;
        outline = spriteContainer.GetComponent<SpriteGlowEffect>();

        boardEntity = GetComponent<BoardEntity>();
        movable = GetComponent<Movable>();
        actionable = GetComponent<Actionable>();
        AI = GetComponent<AI>();

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
            SetSortingParent(targetedSquare);
        }
    }

    public void SetSortingParent(Square square) {
        transform.SetParent(square.entityContainer.transform);
        transform.localPosition = Vector3.zero;
    }

    private void Start() {
        gameObject.name = character.name; // To find it inside the editor
    }

    private void OnCheckSemiTransparent() {
        // A sprite can have several colliders depending on its animations
        Collider2D[] spriteColliders = spriteContainer.GetComponents<Collider2D>();

        Collider2D spriteCollider = null;

        foreach (Collider2D spriteCol in spriteColliders) {
            if (spriteCol.isActiveAndEnabled) {
                spriteCollider = spriteCol;
                break;
            }
        }

        if (spriteCollider == null) return;

        Collider2D[] collidersHit = new Collider2D[20];
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(LayerMask.GetMask("SemiTransparent"));

        int collidersNb = spriteCollider.OverlapCollider(contactFilter, collidersHit);

        if (collidersNb > 0) {
            for (int i = 0; i < collidersNb; i++) {
                if (GetSquare().sortingGroup.sortingOrder > collidersHit[i].GetComponentInParent<Square>().sortingGroup.sortingOrder) {
                    continue;
                }
                
                collidersHit[i].GetComponent<SemiTransparent>().CharacterHiding();
            }
        }
    }

    /**
     * Triggered by Board (SpriteContainer)
     */
    public void MouseEnter() {
        battleManager.fightHUD.SquareHovered(GetSquare());

        if (outline != null) {
            outline.enabled = true;
        }
    }

    /**
     * Triggered by Board (SpriteContainer)
     */
    public void MouseLeave() {
        battleManager.fightHUD.SquareHovered(null);

        if ((battleManager.currentBattleStep == BattleManager.BattleStep.Placing && battleManager.placing.GetCurrentPlacingChar().boardCharacter != this
                || battleManager.currentBattleStep == BattleManager.BattleStep.Fight && battleManager.fight.selectedPlayerCharacter != this) && outline != null) {
            outline.enabled = false;
        }
    }

    /**
     * Triggered by Board (SpriteContainer)
     */
    public void Click() {
        if (side.value == Side.Type.Player) {
            if (battleManager.currentBattleStep == BattleManager.BattleStep.Placing) {
                // Focus the clicked character as the current one to place
                battleManager.placing.SetCurrentPlacingChar(character);
            } else if (battleManager.currentBattleStep == BattleManager.BattleStep.Fight && battleManager.currentTurnStep == BattleManager.TurnStep.None) {
                battleManager.fight.selectedPlayerCharacter = this;
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

    public void BasicAttack(BoardCharacter target) {
        if (actionable.CanDoAction()) {
            int dmgDone = character.BasicAttack(target.character);

            FloatingText floatingText = Instantiate(battleManager.floatingText, target.transform.position, Quaternion.identity);
            floatingText.text = "-" + dmgDone;
            
            actionable.actionTokens--;
            
            battleManager.CheckEndBattle();
        }
    }

    public void MoveThroughPath(Path p, bool cameraFollow = false) {
        if (movable != null && movable.CanMove()) {
            StartCoroutine(MoveCoroutine(p));
            movable.movementTokens--;
        }
    }

    private IEnumerator MoveCoroutine(Path path) {
        isMoving = true;
        float duration = 0.5f;
        Tween cameraAnimation;

        if (!battleManager.battleCamera.IsOnSquare(GetSquare())) {
            cameraAnimation = battleManager.battleCamera.SetPosition(this, true, duration);
            yield return cameraAnimation.WaitForCompletion();
        }

        Square targetedSquare = null;

        Square previousSquare = GetSquare();

        // Check at 25% and 75% of each square the sorting order of the BoardChar to set the correct one
        for (int i = 0; i < path.steps.Count; i++) {
            if (movable.movementPoints <= 0) break;
            //if (!path.steps[i].IsNotBlocking()) break;

            if (i > 0) {
                previousSquare = path.steps[i - 1];
            }

            movable.movementPoints--;

            Tween characterAnimation = transform.DOMove(path.steps[i].transform.position, duration).SetEase(Ease.Linear);
            cameraAnimation = battleManager.battleCamera.SetPosition(path.steps[i], true, duration, Ease.Linear);

            yield return characterAnimation.WaitForPosition(duration / 4);

            if (path.steps[i].x - previousSquare.x < 0 || path.steps[i].y - previousSquare.y < 0) {
                transform.SetParent(path.steps[i].entityContainer.transform);
            }

            yield return characterAnimation.WaitForPosition(duration * 3 / 4);

            if (path.steps[i].x - previousSquare.x > 0 || path.steps[i].y - previousSquare.y > 0) {
                transform.SetParent(path.steps[i].entityContainer.transform);
            }

            yield return characterAnimation.WaitForCompletion();

            targetedSquare = path.steps[i];
        }

        SetSquare(targetedSquare);

        isMoving = false;

        if (battleManager.currentTurnStep != BattleManager.TurnStep.Enemy) {
            battleManager.EventOnLeavingMarkStep();
            battleManager.fight.EnterTurnStepDirection();
            battleManager.EventOnSemiTransparentReset();
        }
    }

    private void OnDestroy() {
        battleManager.OnCheckSemiTransparent -= OnCheckSemiTransparent;
    }
}
