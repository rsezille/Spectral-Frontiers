using UnityEngine;

namespace SF {
    /**
     * TODO [FINAL] Delete this and use a professionnal input manager instead
     */
    public class KeyBind {
        public KeyCode defaultKey { get; private set; }
        public KeyCode bindedKey { get; private set; }
        public string name { get; private set; } // Mainly used to display the key in options
        private bool bindable = false;
        public bool enabled = true;

        public KeyBind(KeyCode defaultKey, string name, bool bindable = true) {
            this.defaultKey = defaultKey;
            bindedKey = defaultKey;
            this.bindable = bindable;
            this.name = name;
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

        public void Bind(KeyCode key, bool toPlayerPrefs = true) {
            if (bindable) {
                bindedKey = key;

                if (toPlayerPrefs) {
                    PlayerPrefs.SetString(name, bindedKey.ToString());
                }
            }
        }

        public void ResetKey(bool toPlayerPrefs = true) {
            bindedKey = defaultKey;

            if (toPlayerPrefs) {
                PlayerPrefs.SetString(name, bindedKey.ToString());
            }
        }
    }
}
