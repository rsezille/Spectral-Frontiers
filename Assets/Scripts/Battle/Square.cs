using DG.Tweening;
using UnityEngine;

public class Square : MonoBehaviour {
    private BattleManager battleManager;

    public enum MarkType {
        None, Movement, Attack, Skill, Item, Placing
    };

    // Positionning
    public int x; // X coordinate of the tile inside the board
    public int y; // Y coordinate of the tile inside the board
    [SerializeField]
    private int height;
    public bool solid = false; // Collision detection

    // Colors
    public static Color defaultColor = new Color(1f, 1f, 1f, 0f); // Default should be transparent
    public static Color overingColor = new Color(0f, 0f, 0f, 0.6f); // When hovering a square without any special behavior
    public static Color placingStartColor = new Color(0f, 0.37f, 1f, 0.65f);
    public static Color movementColor = new Color(0.12f, 0.57f, 0.2f, 0.63f);
    public static Color attackColor = new Color(0.6f, 0.15f, 0.9f, 0.5f);
    public static Color itemColor = new Color(1f, 1f, 0.4f, 0.5f);
    private Tween colorAnimation;

    public BoardEntity boardEntity;

    private MarkType _markType = MarkType.None;
    public MarkType markType {
        get {
            return _markType;
        }

        set {
            _markType = value;
            RefreshColor();
        }
    }

    public SpriteRenderer tileSelector;

    private SFTileContainer _tileContainer;
    private SFEntityContainer _entityContainer;

    private void Awake() {
        battleManager = BattleManager.instance;

        tileSelector.color = defaultColor;

        battleManager.OnEnterPlacing += OnEnterPlacing;
        battleManager.OnLeavingMarkStep += OnLeavingMarkStep;

        colorAnimation = tileSelector.DOFade(0.3f, 0.8f).SetLoops(-1, LoopType.Yoyo);
        colorAnimation.Pause();
    }

    private void OnEnterPlacing() {
        colorAnimation.Play();
    }

    private void OnLeavingMarkStep() {
        markType = MarkType.None;
        colorAnimation.Pause();

        RefreshColor();
    }

    /**
     * Triggered by Board (SFTileSelector)
     */
    public void MouseEnter() {
        if (colorAnimation != null) colorAnimation.Pause();

        battleManager.fightHUD.SquareHovered(this);
        tileSelector.color = overingColor;

        if (battleManager.currentBattleStep == BattleManager.BattleStep.Placing && markType == MarkType.Placing) {
            tileSelector.color = new Color(placingStartColor.r, placingStartColor.g, placingStartColor.b, placingStartColor.a + 0.2f);
        } else if (battleManager.currentTurnStep == BattleManager.TurnStep.Move && markType == MarkType.Movement) {
            tileSelector.color = new Color(movementColor.r, movementColor.g + 0.2f, movementColor.b, movementColor.a);
        } else if (battleManager.currentTurnStep == BattleManager.TurnStep.Attack && markType == MarkType.Attack) {
            tileSelector.color = new Color(attackColor.r, attackColor.g, attackColor.b, attackColor.a + 0.2f);
        }
    }

    /**
     * Triggered by Board (SFTileSelector)
     */
    public void MouseLeave() {
        battleManager.fightHUD.SquareHovered(null);

        RefreshColor();

        if (colorAnimation != null) colorAnimation.Play();
    }

    /**
     * Triggered by Board (SFTileSelector)
     */
    public void Click() {
        switch (battleManager.currentBattleStep) {
            case BattleManager.BattleStep.Placing:
                if (markType == MarkType.Placing && IsNotBlocking()) {
                    BattleManager.instance.placing.PlaceMapChar(this);
                }

                break;
            case BattleManager.BattleStep.Fight:
                if (battleManager.currentTurnStep == BattleManager.TurnStep.Move && markType == MarkType.Movement) {
                    Path p = battleManager.board.pathFinder.FindPath(
                        battleManager.GetSelectedPlayerBoardCharacter().GetSquare().x,
                        battleManager.GetSelectedPlayerBoardCharacter().GetSquare().y,
                        this.x,
                        this.y
                    );

                    if (p != null) {
                        battleManager.GetSelectedPlayerBoardCharacter().Move(p, true);
                    }
                } else if (battleManager.currentTurnStep == BattleManager.TurnStep.Attack && markType == MarkType.Attack) {
                    battleManager.GetSelectedPlayerBoardCharacter().BasicAttack(boardEntity.GetComponent<BoardCharacter>());
                    battleManager.EnterTurnStepNone();
                }

                break;
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
        return boardEntity == null;
    }

    public int GetManhattanDistance(Square square) {
        return Mathf.Abs(square.x - this.x) + Mathf.Abs(square.y - this.y);
    }

    public SFTileContainer tileContainer {
        get {
            if (_tileContainer == null) {
                _tileContainer = GetComponentInChildren<SFTileContainer>();
            }

            return _tileContainer;
        }
    }

    public SFEntityContainer entityContainer {
        get {
            if (_entityContainer == null) {
                _entityContainer = GetComponentInChildren<SFEntityContainer>();
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
}
