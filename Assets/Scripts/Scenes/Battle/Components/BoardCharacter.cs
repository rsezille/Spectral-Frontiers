using DG.Tweening;
using SF;
using System.Collections;
using UnityEngine;

/**
 * Represent a character on the board
 * The GameObject is placed in the attached Square inside the EntityContainer
 */
[RequireComponent(typeof(BoardEntity), typeof(Side))]
public class BoardCharacter : MonoBehaviour {
    public enum Direction {
        North, East, South, West // Rotate a 2D plan by 90 degrees clockwise
    };
    
    public Character character;
    private GameObject spriteContainer;

    [Header("Dependencies")]
    public BattleState battleState;
    public BattleCharacters battleCharacters;
    public Board board;
    public CharacterVariable currentPartyCharacter;
    public BoardCharacterVariable currentFightBoardCharacter;
    public CameraPosition mainCameraPosition;
    public Party party;

    [Header("Events")]
    public GameEvent checkSemiTransparent;
    public SquareEvent hoverSquare;
    public GameEvent killCharacter;

    [Header("Prefabs")]
    public HealthBarHUDController healthBarHUD;
    public CharacterNameHUDController characterNameHUD;
    public float offset = 0.2f; // TODO: Do it in Character SO instead?
    public FloatingText floatingText;

    [Header("Data")]
    public int movementPoints; // At the beginning of each turn, movementPoints = character.movementPoints
    public int movementTokens; // At the beginning of each turn, movementTokens = character.movementTokens
    public int actionTokens; // At the beginning of each turn, actionTokens = character.actionTokens

    // Components
    private BoardEntity boardEntity;
    [HideInInspector]
    public SpriteRenderer sprite;
    private Animator animator;
    [HideInInspector]
    public Side side;
    // Components which can be null
    [HideInInspector]
    public Glow glow;
    [HideInInspector]
    public AI AI;
    [HideInInspector]
    public ShadowController shadow;

    [HideInInspector]
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
        side = GetComponent<Side>();
        boardEntity = GetComponent<BoardEntity>();
        AI = GetComponent<AI>();
    }

    private void Start() {
        HealthBarHUDController healthBarHUDInstance = Instantiate(healthBarHUD, transform);
        healthBarHUDInstance.character = character;
        healthBarHUDInstance.transform.position += new Vector3(0f, sprite.bounds.size.y + offset);

        CharacterNameHUDController characterNameHUDInstance = Instantiate(characterNameHUD, transform);
        characterNameHUDInstance.character = character;
        float offsetName = offset;
        
        //if (healthBarHUD) {
            offsetName = 0.3f + offset;
        //}

        characterNameHUDInstance.transform.position += new Vector3(0f, sprite.bounds.size.y + offsetName);
    }

    public void Init(Character character, Side.Type side, Direction direction) {
        this.character = character;

        spriteContainer = Instantiate(character.template.spritePrefab, transform);

        animator = spriteContainer.GetComponent<Animator>();
        sprite = spriteContainer.GetComponent<SpriteRenderer>();
        sprite.sortingOrder = 1;
        glow = spriteContainer.GetComponent<Glow>();
        shadow = spriteContainer.GetComponentInChildren<ShadowController>();

        gameObject.name = character.characterName;

        if (glow) {
            glow.Disable();
        }

        this.side.value = side;
        this.direction = direction;
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

    public bool CanMove() {
        return movementTokens > 0;
    }

    public bool CanDoAction() {
        return actionTokens > 0;
    }

    public void SetSortingParent(Square square) {
        transform.SetParent(square.entityContainer.transform);
        transform.localPosition = Vector3.zero;
    }

    public void CheckSemiTransparent() {
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
     * Triggered by Board (SpriteManager)
     */
    public void MouseEnter() {
        hoverSquare.Raise(GetSquare());

        if (glow != null) {
            glow.Enable();
        }
    }

    /**
     * Triggered by Board (SpriteManager)
     */
    public void MouseLeave() {
        hoverSquare.Raise(null);

        if ((battleState.currentBattleStep == BattleState.BattleStep.Placing && currentPartyCharacter.value.boardCharacter != this
                || battleState.currentBattleStep == BattleState.BattleStep.Fight && currentFightBoardCharacter.value != this) && glow != null) {
            glow.Disable();
        }
    }

    /**
     * Triggered by Board (SpriteManager)
     */
    public void Click() {
        if (side.value == Side.Type.Player) {
            if (battleState.currentBattleStep == BattleState.BattleStep.Placing) {
                // Focus the clicked character as the current one to place
                SetCurrentPlacingChar();
            } else if (battleState.currentBattleStep == BattleState.BattleStep.Fight && battleState.currentTurnStep == BattleState.TurnStep.None) {
                currentFightBoardCharacter.value = this;
            }
        }

        if (battleState.currentBattleStep == BattleState.BattleStep.Fight && battleState.currentTurnStep == BattleState.TurnStep.Attack) {
            if (GetSquare().markType == Square.MarkType.Attack) {
                currentFightBoardCharacter.value.BasicAttack(this);
                battleState.currentTurnStep = BattleState.TurnStep.None;
            }
        }
    }

    private void SetCurrentPlacingChar() {
        if (!party.characters.Contains(character)) {
            return;
        }

        if (currentPartyCharacter.value.boardCharacter != null && currentPartyCharacter.value.boardCharacter.glow != null) {
            currentPartyCharacter.value.boardCharacter.glow.Disable();
        }

        currentPartyCharacter.value = character;

        if (currentPartyCharacter.value.boardCharacter != null) {
            if (currentPartyCharacter.value.boardCharacter.glow != null) {
                currentPartyCharacter.value.boardCharacter.glow.Enable();
            }

            mainCameraPosition.SetPosition(currentPartyCharacter.value.boardCharacter, true);
        }
    }

    public void NewTurn() {
        actionTokens = character.template.actionTokens;
        
        movementTokens = character.template.movementTokens;
        movementPoints = character.template.movementPoints;
    }

    private bool IsDead() {
        return character.currentHp <= 0;
    }

    public void BasicAttack(BoardCharacter target) {
        if (CanDoAction()) {
            int dmgDone = character.BasicAttack(target.character);

            FloatingText floatingTextInstance = Instantiate(floatingText, target.transform.position, Quaternion.identity);
            floatingTextInstance.text = "-" + dmgDone;
            
            actionTokens--;

            if (target.IsDead()) target.Remove();
            if (IsDead()) Remove();
        }
    }

    public void MoveTo(Square target, bool cameraFollow = true) {
        Path p = board.pathFinder.FindPath(
            GetSquare().x,
            GetSquare().y,
            target.x,
            target.y,
            side.value
        );

        if (p != null) {
            MoveThroughPath(p, cameraFollow);
        }
    }

    public IEnumerator CutsceneMoveTo(Square target, bool cameraFollow = true) {
        Path p = board.pathFinder.FindPath(
            GetSquare().x,
            GetSquare().y,
            target.x,
            target.y,
            side.value
        );

        if (p != null) {
            yield return StartCoroutine(MoveCoroutine(p, true, cameraFollow));
        }

        yield return null;
    }
    
    public void MoveThroughPath(Path p, bool cameraFollow = true) {
        if (CanMove()) {
            StartCoroutine(MoveCoroutine(p, false, cameraFollow));
            
            movementTokens--;
        }
    }

    private IEnumerator MoveCoroutine(Path path, bool inCutscene = false, bool cameraFollow = true) {
        isMoving = true;
        float duration = 0.5f;

        if (battleState.currentTurnStep != BattleState.TurnStep.Enemy && !inCutscene) {
            board.RemoveAllMarks();
        }

        if (cameraFollow) {
            if (!mainCameraPosition.IsOnSquare(GetSquare())) {
                Tween cameraAnimation = mainCameraPosition.SetPosition(this, true, duration);

                yield return cameraAnimation.WaitForCompletion();
            }
        }

        Square targetedSquare = null;
        Square previousSquare = GetSquare();

        // Check at 25% and 75% of each square the sorting order of the BoardChar to set the correct one
        for (int i = 0; i < path.steps.Count; i++) {
            if (movementPoints <= 0 && !inCutscene) break;
            //if (!path.steps[i].IsNotBlocking()) break;

            if (i > 0) {
                previousSquare = path.steps[i - 1];
            }

            if (!inCutscene) {
                movementPoints--;
            }

            Tween characterAnimation;
            
            float absoluteHeightDifference = Mathf.Abs(previousSquare.Height - path.steps[i].Height);

            if (absoluteHeightDifference > 6) {
                float clamped = Mathf.Clamp(absoluteHeightDifference, 0f, 50f);
                float jumpPower = Mathf.Lerp(0f, 0.8f, clamped / 50f);

                characterAnimation = transform.DOJump(BoardUtil.CoordToWorldPosition(path.steps[i]), jumpPower, 1, duration);
            } else {
                characterAnimation = transform.DOMove(BoardUtil.CoordToWorldPosition(path.steps[i]), duration).SetEase(Ease.Linear);
            }

            if (cameraFollow) {
                mainCameraPosition.SetPosition(path.steps[i], true, duration, Ease.Linear);
            }

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

        if (battleState.currentTurnStep != BattleState.TurnStep.Enemy && !inCutscene) {
            battleState.currentTurnStep = BattleState.TurnStep.Direction;
            checkSemiTransparent.Raise();
        }
    }

    public void Remove() {
        SetSquare(null);
        character.boardCharacter = null;

        StopAllCoroutines();

        if (side.value == Side.Type.Player && (battleState.currentBattleStep == BattleState.BattleStep.Placing || battleState.currentBattleStep == BattleState.BattleStep.Fight)) {
            battleCharacters.player.Remove(this);
        } else if (side.value == Side.Type.Enemy && battleState.currentBattleStep == BattleState.BattleStep.Fight) {
            battleCharacters.enemy.Remove(this);
        }

        if (battleState.currentBattleStep == BattleState.BattleStep.Fight) {
            killCharacter.Raise();
        }

        Destroy(gameObject);
    }
}
