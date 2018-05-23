using UnityEngine;

/**
 * Represent an entity which is displayed on a square (or several).
 * A BoardEntity is blocking (ie. only one BoardEntity per Square)
 */
 
public class BoardEntity : MonoBehaviour {
    public enum Side {
        Player, Enemy, Neutral
    };
    public Side side;

    public Square[] squares; // The square(s) the entity is placed on
}
