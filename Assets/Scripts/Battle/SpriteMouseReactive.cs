using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(MouseReactive))]
public class SpriteMouseReactive : MonoBehaviour {
    // Components
    private MouseReactive mouseReactive;

    private BoardCharacter boardCharacter;

    private void Awake() {
        boardCharacter = GetComponentInParent<BoardCharacter>();

        mouseReactive = GetComponent<MouseReactive>();
        mouseReactive.MouseEnter = new UnityEvent();
        mouseReactive.MouseEnter.AddListener(MouseEnter);
        mouseReactive.MouseLeave = new UnityEvent();
        mouseReactive.MouseLeave.AddListener(MouseLeave);
        mouseReactive.Click = new UnityEvent();
        mouseReactive.Click.AddListener(Click);
    }

    /**
     * Triggered by Board
     */
    public void MouseEnter() {
        if (boardCharacter != null) {
            boardCharacter.MouseEnter();
        }
    }

    /**
     * Triggered by Board
     */
    public void MouseLeave() {
        if (boardCharacter != null) {
            boardCharacter.MouseLeave();
        }
    }

    /**
     * Triggered by Board
     */
    public void Click() {
        if (boardCharacter != null) {
            boardCharacter.Click();
        }
    }
}
