using UnityEngine;
using SpriteGlow;

[RequireComponent(typeof(BoardEntity), typeof(Movable), typeof(Actionable))]
[RequireComponent(typeof(Side), typeof(MouseReactive), typeof(SpriteGlowEffect))]
[RequireComponent(typeof(BoardCharacter))]
public class Orc : MonoBehaviour {
    private BattleManager battleManager; // Shortcut for BattleManager.instance

    // Components
    public Side side;
    public BoardCharacter boardCharacter;

    private void Awake() {
        side = GetComponent<Side>();
        boardCharacter = GetComponent<BoardCharacter>();

        battleManager = BattleManager.instance;
    }
}