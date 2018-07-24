using UnityEngine;

/**
 * Can be moved by the AI or the player, depending on the side
 */
public class Movable : MonoBehaviour {
    public int movementPoints; // At the beginning of each turn, movementPoints = character.movementPoints
    public int movementTokens; // At the beginning of each turn, movementTokens = character.movementTokens

    public bool CanMove() {
        return movementTokens > 0;
    }
}
