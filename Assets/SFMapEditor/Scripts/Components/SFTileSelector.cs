﻿using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(MouseReactive))]
public class SFTileSelector : MonoBehaviour {
    // Components
    private MouseReactive mouseReactive;

    public Square square;

    private void Awake() {
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
        square.MouseEnter();
    }

    /**
     * Triggered by Board
     */
    public void MouseLeave() {
        square.MouseLeave();
    }

    /**
     * Triggered by Board
     */
    public void Click() {
        Debug.Log("click on tileselector");
        square.Click();
    }
}
