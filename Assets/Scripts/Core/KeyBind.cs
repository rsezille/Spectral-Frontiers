using UnityEngine;

namespace SF {
    /**
     * TODO [FINAL] Delete this and use a professionnal input manager instead
     */
    public class KeyBind {
        public KeyCode defaultKey { get; private set; }
        public KeyCode bindedKey { get; private set; }
        private bool bindable = false;
        public bool enabled = true;

        public KeyBind(KeyCode defaultKey, bool bindable = false) {
            this.defaultKey = defaultKey;
            bindedKey = defaultKey;
            this.bindable = bindable;
        }

        public bool IsKeyDown {
            get {
                return Input.GetKeyDown(bindedKey) && enabled;
            }
        }

        public bool IsKeyHeld {
            get {
                return Input.GetKey(bindedKey) && enabled;
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
