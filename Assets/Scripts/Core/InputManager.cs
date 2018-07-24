using UnityEngine;

namespace SF {
    /**
     * Define all input bindings
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
        public static readonly KeyBind Click = new KeyBind(KeyCode.Mouse0, "Left click");
        public static readonly KeyBind Previous = new KeyBind(KeyCode.A, "Previous");
        public static readonly KeyBind Next = new KeyBind(KeyCode.E, "Next");
        public static readonly KeyBind Special1 = new KeyBind(KeyCode.Space, "Special 1");
        public static readonly KeyBind Up = new KeyBind(KeyCode.UpArrow, "Up");
        public static readonly KeyBind Down = new KeyBind(KeyCode.DownArrow, "Down");
        public static readonly KeyBind Left = new KeyBind(KeyCode.LeftArrow, "Left");
        public static readonly KeyBind Right = new KeyBind(KeyCode.RightArrow, "Right");
        public static readonly KeyBind Confirm = new KeyBind(KeyCode.Return, "Confirm");
        public static readonly KeyBind CameraUp = new KeyBind(KeyCode.Z, "Camera Up");
        public static readonly KeyBind CameraDown = new KeyBind(KeyCode.S, "Camera Down");
        public static readonly KeyBind CameraLeft = new KeyBind(KeyCode.Q, "Camera Left");
        public static readonly KeyBind CameraRight = new KeyBind(KeyCode.D, "Camera Right");
        public static readonly KeyBind Pause = new KeyBind(KeyCode.Escape, "Pause");

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
