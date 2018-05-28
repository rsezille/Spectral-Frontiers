using UnityEngine;

public class Movable : MonoBehaviour {
    public int movementPoints; // At the beginning of each turn, movementPoints = character.movementPoints
    public int movementTokens; // At the beginning of each turn, movementTokens = character.movementTokens

    public bool CanMove() {
        return movementTokens > 0;
    }
}
