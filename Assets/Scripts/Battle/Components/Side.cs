using UnityEngine;

/**
 * Player: Can be played by the player and can attack Neutral & Enemy
 * Enemy: Can be played by the AI and can attack Player & Neutral
 * Neutral: Can be played by the AI and can attack Player & Enemy (situational)
 */
[RequireComponent(typeof(BoardEntity))]
public class Side : MonoBehaviour {
    public enum Type {
        Player, Enemy, Neutral
    };

    public Type value;
}
