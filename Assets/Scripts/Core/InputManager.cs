using UnityEngine;

namespace SF {
    /**
     * Define all input bindings
     * 
     * Usage:
     *   if (InputManager.Next.IsKeyDown) { do something }
     * 
     * TODO: Delete this and use a professionnal input manager instead
     */
    public static class InputManager {
        /**
         * Usage: Input.GetAxis(InputManager.Axis.Zoom)
         */
        public static class Axis {
            public const string Zoom = "input.mouse_scrollwheel";
        }

        // Bindings
        public static readonly KeyBind Click = new KeyBind(KeyCode.Mouse0);
        public static readonly KeyBind Previous = new KeyBind(KeyCode.A);
        public static readonly KeyBind Next = new KeyBind(KeyCode.E);
        public static readonly KeyBind Special1 = new KeyBind(KeyCode.Space);
        public static readonly KeyBind Up = new KeyBind(KeyCode.UpArrow);
        public static readonly KeyBind Down = new KeyBind(KeyCode.DownArrow);
        public static readonly KeyBind Left = new KeyBind(KeyCode.LeftArrow);
        public static readonly KeyBind Right = new KeyBind(KeyCode.RightArrow);
        public static readonly KeyBind Confirm = new KeyBind(KeyCode.Return);
        public static readonly KeyBind CameraUp = new KeyBind(KeyCode.Z);
        public static readonly KeyBind CameraDown = new KeyBind(KeyCode.S);
        public static readonly KeyBind CameraLeft = new KeyBind(KeyCode.Q);
        public static readonly KeyBind CameraRight = new KeyBind(KeyCode.D);

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
