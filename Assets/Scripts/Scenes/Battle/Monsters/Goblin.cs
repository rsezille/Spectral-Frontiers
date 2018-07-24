using UnityEngine;
using SpriteGlow;
using System.Collections;

[RequireComponent(typeof(BoardEntity), typeof(Movable), typeof(Actionable))]
[RequireComponent(typeof(Side), typeof(MouseReactive), typeof(SpriteGlowEffect))]
[RequireComponent(typeof(BoardCharacter), typeof(AI))]
public class Goblin : MonoBehaviour, ICustomAI {
    private CustomAI customAI;

    private void Awake() {
        //customAI = gameObject.AddComponent<CustomAI>() as CustomAI;
        //customAI.Use(this);
    }

    /**
    * BoardCharacter.character is not available in Awake(), but is in Start()
    */
    private void Start() {
        GetComponent<BoardCharacter>().character.SetMaxHP(10);
    }

    public IEnumerator ProcessAI() {
        yield return null;
    }
}
