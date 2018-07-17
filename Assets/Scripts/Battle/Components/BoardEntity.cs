using UnityEngine;

/**
 * Represent an entity which is displayed on a square (or several).
 * A BoardEntity is blocking (ie. only one BoardEntity per Square)
 * TODO [ALPHA] Change "Square square" to "List Square" for multiple squares and size > 1
 */
public class BoardEntity : MonoBehaviour {
    public int size = 1; // Number of squares the entity is taking (a fat boss not moving could take 10 squares)
    public Square square; // The square the entity is placed on
}
