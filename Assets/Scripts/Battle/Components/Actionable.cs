using UnityEngine;

public class Actionable : MonoBehaviour {
    public int actionTokens; // At the beginning of each turn, actionTokens = character.actionTokens

    public bool CanDoAction() {
        return actionTokens > 0;
    }
}
