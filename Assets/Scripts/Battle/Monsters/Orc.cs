using UnityEngine;
using SpriteGlow;

[RequireComponent(typeof(BoardEntity), typeof(Movable), typeof(Actionable))]
[RequireComponent(typeof(Side), typeof(MouseReactive), typeof(SpriteGlowEffect))]
[RequireComponent(typeof(BoardCharacter), typeof(AI))]
public class Orc : MonoBehaviour {
    /**
     * BoardCharacter.character is not available in Awake(), but is in Start()
     */
    private void Start() {
        GetComponent<BoardCharacter>().character.SetMaxHP(20);
    }
}
