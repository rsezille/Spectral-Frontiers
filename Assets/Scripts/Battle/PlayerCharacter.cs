using UnityEngine;

/**
 * Represent a placed player character on the battlefield
 * Manage graphics and attached components
 */
[RequireComponent(typeof(BoardEntity), typeof(Movable), typeof(Actionable))]
[RequireComponent(typeof(Side), typeof(BoardCharacter))]
public class PlayerCharacter : MonoBehaviour {
    // Components
    public BoardCharacter boardCharacter;

    private void Awake() {
        boardCharacter = GetComponent<BoardCharacter>();
    }
}
