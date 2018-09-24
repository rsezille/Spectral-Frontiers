using DG.Tweening;
using SF;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(SortingGroup))]
public class Square : MonoBehaviour {
    public enum MarkType {
        None, Movement, Attack, Skill, Item, Placing
    };

    private BattleManager battleManager;

    [Header("Dependencies")]
    public BattleState battleState;
    public Board board;
    public BoardCharacterVariable currentFightBoardCharacter;

    [Header("Data")]
    // Positionning
    public int x; // X coordinate of the tile inside the board
    public int y; // Y coordinate of the tile inside the board
    [SerializeField]
    private int height;
    public bool solid = false; // Collision detection
    public BoardCharacter.Direction startingDirection = Globals.DefaultDirection;

    // Colors
    public static float maxAlpha = 0.8f;
    public static Color defaultColor = new Color(1f, 1f, 1f, 0f); // Default should be transparent
    public static Color overingColor = new Color(0f, 0f, 0f, 0.6f); // When hovering a square without any special behavior
    public static Color placingStartColor = new Color(0f, 0.37f, 1f, maxAlpha);
    public static Color movementColor = new Color(0.12f, 0.57f, 0.2f, maxAlpha);
    public static Color attackColor = new Color(0.6f, 0.15f, 0.9f, maxAlpha);
    public static Color itemColor = new Color(1f, 1f, 0.4f, maxAlpha);
    private Sequence colorAnimation;

    public BoardEntity boardEntity;

    public SortingGroup sortingGroup;
    
    private MarkType _markType = MarkType.None;
    public MarkType markType {
        get {
            return _markType;
        }

        set {
            _markType = value;

            if (_markType != MarkType.None) {
                if (colorAnimation != null) return;

                // Don't use a tween with LoopType.Yoyo as we will lose elapsed time and Goto
                colorAnimation = DOTween
                    .Sequence()
                    .Append(tileSelector.DOFade(0.55f, 0.8f).SetEase(Ease.Linear))
                    .Append(tileSelector.DOFade(maxAlpha, 0.8f).SetEase(Ease.Linear))
                    .SetLoops(-1);
                board.markedSquareAnimations.Add(colorAnimation);
            } else {
                if (colorAnimation != null) {
                    colorAnimation.Kill();
                    colorAnimation = null;
                }
            }

            RefreshColor();
        }
    }

    public SpriteRenderer tileSelector;

    private TileContainer _tileContainer;
    private EntityContainer _entityContainer;

    private void Awake() {
        battleManager = BattleManager.instance;

        sortingGroup = GetComponent<SortingGroup>();

        tileSelector.color = defaultColor;
    }

    public void RefreshMark() {
        if (markType != MarkType.None) {
            PlayColorAnimation();
        }
    }

    public void RemoveMark() {
        if (colorAnimation != null) {
            colorAnimation.Kill();
            colorAnimation = null;
        }

        markType = MarkType.None;

        RefreshColor();
    }

    /**
     * Triggered by Board (TileSelector)
     */
    public void MouseEnter() {
        if (battleState.currentBattleStep == BattleState.BattleStep.Cutscene) return;

        if (colorAnimation != null) {
            colorAnimation.Pause();
            board.markedSquareAnimations.Remove(colorAnimation);
        }

        battleManager.fightHUD.SquareHovered(this);
        tileSelector.color = overingColor;

        if (battleState.currentBattleStep == BattleState.BattleStep.Placing && markType == MarkType.Placing) {
            tileSelector.color = new Color(placingStartColor.r, placingStartColor.g, placingStartColor.b, placingStartColor.a + 0.2f);
        } else if (battleState.currentTurnStep == BattleState.TurnStep.Move && markType == MarkType.Movement) {
            tileSelector.color = new Color(movementColor.r, movementColor.g + 0.2f, movementColor.b, movementColor.a);
        } else if (battleState.currentTurnStep == BattleState.TurnStep.Attack && markType == MarkType.Attack) {
            tileSelector.color = new Color(attackColor.r, attackColor.g, attackColor.b, attackColor.a + 0.2f);
        }
    }

    /**
     * Triggered by Board (TileSelector)
     */
    public void MouseLeave() {
        if (battleState.currentBattleStep == BattleState.BattleStep.Cutscene) return;

        battleManager.fightHUD.SquareHovered(null);

        RefreshColor();

        if (markType != MarkType.None) {
            PlayColorAnimation();
        }
    }

    /**
     * Triggered by Board (TileSelector)
     */
    public void Click() {
        switch (battleState.currentBattleStep) {
            case BattleState.BattleStep.Placing:
                if (markType == MarkType.Placing && IsNotBlocking()) {
                    BattleManager.instance.placing.PlaceMapChar(this);
                }

                break;
            case BattleState.BattleStep.Fight:
                if (battleState.currentTurnStep == BattleState.TurnStep.Move && markType == MarkType.Movement) {
                    currentFightBoardCharacter.value.MoveTo(this);
                } else if (battleState.currentTurnStep == BattleState.TurnStep.Attack && markType == MarkType.Attack) {
                    if (boardEntity != null) {
                        currentFightBoardCharacter.value.BasicAttack(boardEntity.GetComponent<BoardCharacter>());
                    }

                    battleManager.EnterTurnStepNone();
                }

                break;
        }
    }

    public void PlayColorAnimation() {
        if (colorAnimation == null) return;

        // We're doing this to keep the current animation frame when leaving an hovering and marked square (which change the color)
        if (board.markedSquareAnimations.Count > 0) {
            float elapsed = board.markedSquareAnimations[0].Elapsed(false);

            colorAnimation.Goto(elapsed);
        }

        colorAnimation.Play();

        if (!board.markedSquareAnimations.Contains(colorAnimation)) {
            board.markedSquareAnimations.Add(colorAnimation);
        }
    }

    public void RefreshColor() {
        tileSelector.color = defaultColor;
        
        switch (markType) {
            case MarkType.Placing:
                tileSelector.color = placingStartColor;
                break;
            case MarkType.Movement:
                tileSelector.color = movementColor;
                break;
            case MarkType.Attack:
                tileSelector.color = attackColor;
                break;
            case MarkType.Item:
                tileSelector.color = itemColor;
                break;
        }
    }

    public bool IsNotBlocking() {
        return boardEntity == null && !solid;
    }

    public bool IsNotBlocking(Side.Type side) {
        if (boardEntity?.side == null) {
            return IsNotBlocking();
        }

        return (boardEntity == null || boardEntity.side.value == side) && !solid;
    }

    public int GetManhattanDistance(Square square) {
        return Mathf.Abs(square.x - x) + Mathf.Abs(square.y - y);
    }

    public TileContainer tileContainer {
        get {
            if (_tileContainer == null) {
                _tileContainer = GetComponentInChildren<TileContainer>();
            }

            return _tileContainer;
        }
    }

    public EntityContainer entityContainer {
        get {
            if (_entityContainer == null) {
                _entityContainer = GetComponentInChildren<EntityContainer>();
            }

            return _entityContainer;
        }
    }

    public int Height {
        get {
            return height;
        }

        set {
            height = value;
            entityContainer.transform.localPosition = new Vector3(0f, (float)Height / Globals.TileHeight);
        }
    }

    public float GetWorldHeight() {
        return (float)Height / Globals.TileHeight;
    }
}
