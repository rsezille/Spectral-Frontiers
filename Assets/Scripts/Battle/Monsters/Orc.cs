using UnityEngine;
using SpriteGlow;

[RequireComponent(typeof(BoardEntity), typeof(Movable), typeof(Actionable))]
[RequireComponent(typeof(Side), typeof(MouseReactive), typeof(SpriteGlowEffect))]
[RequireComponent(typeof(BoardCharacter))]
public class Orc : MonoBehaviour {
    public SpriteRenderer sprite;

    public BattleManager battleManager; // Shortcut for BattleManager.instance

    // Components
    private BoardEntity boardEntity;
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
        
        outline.enabled = false;
    }
}
