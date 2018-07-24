using System.Collections.Generic;
using UnityEngine;

namespace SF {
    /**
     * Define all input bindings
     * Work closely with PlayerOptions
     * 
     * Usage: if (InputManager.Next.IsKeyDown) { do something }
     * 
     * TODO [FINAL] Delete this and use a professionnal input manager instead
     */
    public static class InputManager {
        /**
         * Usage: Input.GetAxis(InputManager.Axis.Zoom)
         */
        public static class Axis {
            public const string Zoom = "input.mouse_scrollwheel";
        }

        // Bindings
        public static readonly KeyBind Click = new KeyBind(KeyCode.Mouse0, "keybind.click.name", false);
        public static readonly KeyBind Previous = new KeyBind(KeyCode.A, "keybind.previous.name");
        public static readonly KeyBind Next = new KeyBind(KeyCode.E, "keybind.next.name");
        public static readonly KeyBind Special1 = new KeyBind(KeyCode.Space, "keybind.special1.name");
        public static readonly KeyBind Up = new KeyBind(KeyCode.UpArrow, "keybind.up.name");
        public static readonly KeyBind Down = new KeyBind(KeyCode.DownArrow, "keybind.down.name");
        public static readonly KeyBind Left = new KeyBind(KeyCode.LeftArrow, "keybind.left.name");
        public static readonly KeyBind Right = new KeyBind(KeyCode.RightArrow, "keybind.right.name");
        public static readonly KeyBind Confirm = new KeyBind(KeyCode.Return, "keybind.confirm.name");
        public static readonly KeyBind CameraUp = new KeyBind(KeyCode.Z, "keybind.cameraup.name");
        public static readonly KeyBind CameraDown = new KeyBind(KeyCode.S, "keybind.cameradown.name");
        public static readonly KeyBind CameraLeft = new KeyBind(KeyCode.Q, "keybind.cameraleft.name");
        public static readonly KeyBind CameraRight = new KeyBind(KeyCode.D, "keybind.cameraright.name");
        public static readonly KeyBind Pause = new KeyBind(KeyCode.Escape, "keybind.pause.name");

        public static readonly List<KeyBind> allKeyBinds = new List<KeyBind>() {
            Click, Previous, Next, Special1, Up, Down, Left, Right, Confirm, CameraUp, CameraDown, CameraLeft, CameraRight, Pause
        };

        // Shortcuts
        public static int CameraHorizontalAxis() { return GetAxis(CameraRight, CameraLeft); }
        public static int CameraVerticalAxis() { return GetAxis(CameraUp, CameraDown); }

        /**
         * Simulate an axis and return an integer between -1 and 1 given two inputs
         */
        public static int GetAxis(KeyBind positiveKey, KeyBind negativeKey) {
            int axisValue = 0;

            if (positiveKey.IsKeyHeld) axisValue++;
            if (negativeKey.IsKeyHeld) axisValue--;

            return axisValue;
        }
    }
}
