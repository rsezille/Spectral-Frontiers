using UnityEngine;

[RequireComponent(typeof(BoardEntity), typeof(Movable), typeof(Actionable))]
[RequireComponent(typeof(Side), typeof(BoardCharacter), typeof(AI))]
public class Soldier : MonoBehaviour {
    /**
     * BoardCharacter.character is not available in Awake, but it is in Start
     */
    private void Start() {
        GetComponent<BoardCharacter>().character.SetMaxHP(10);
    }
}
