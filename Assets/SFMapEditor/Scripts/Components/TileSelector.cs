﻿using UnityEngine;
using UnityEngine.Events;

namespace SF {
    /**
     * The game object this script is attached to, is added dynamically when adding tiles in the SF Map Editor
     */
    [RequireComponent(typeof(MouseReactive), typeof(SpriteRenderer))]
    public class TileSelector : MonoBehaviour {
        // Components
        private MouseReactive mouseReactive;
        
        [HideInInspector]
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
            square.Click();
        }
    }
}
