using UnityEngine;

/**
 * Can do actions (basic attack, skill, item) by the AI or the player depending on the side
 */
public class Actionable : MonoBehaviour {
    public int actionTokens; // At the beginning of each turn, actionTokens = character.actionTokens

    public bool CanDoAction() {
        return actionTokens > 0;
    }
}
