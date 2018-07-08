using UnityEngine;

namespace SF {
    /**
     * TODO: Delete this and use a professionnal input manager instead
     */
    public class KeyBind {
        public KeyCode defaultKey { get; private set; }
        public KeyCode bindedKey { get; private set; }
        private bool bindable = false;

        public KeyBind(KeyCode defaultKey, bool bindable = false) {
            this.defaultKey = defaultKey;
            bindedKey = defaultKey;
            this.bindable = bindable;
        }

        public bool IsKeyDown {
            get {
                return Input.GetKeyDown(bindedKey);
            }
        }

        public bool IsKeyHeld {
            get {
                return Input.GetKey(bindedKey);
            }
        }

        public void Bind(KeyCode key) {
            if (bindable) {
                bindedKey = key;
            }
        }

        public void ResetKey() {
            bindedKey = defaultKey;
        }
    }
}
