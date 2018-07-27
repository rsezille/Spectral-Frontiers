using System.Collections.Generic;
using UnityEngine;

namespace SF {
    /**
     * TODO [FINAL] Delete this and use a professionnal input manager instead
     */
    public class KeyBind {
        private KeyCode defaultKey;
        public KeyCode bindedKey { get; private set; }
        public string name { get; private set; } // Mainly used to display the key in options
        private bool bindable = false;
        public bool enabled = true;
        private Dictionary<string, KeyCode> languageDefaults = null;

        public KeyBind(KeyCode defaultKey, string name, bool bindable = true, Dictionary<string, KeyCode> languageDefaults = null) {
            this.defaultKey = defaultKey;
            bindedKey = defaultKey;
            this.bindable = bindable;
            this.name = name;
            this.languageDefaults = languageDefaults;
        }

        public KeyCode GetDefaultKey() {
            if (languageDefaults != null && languageDefaults.ContainsKey(PlayerOptions.GetString(PlayerOptions.Language)) && PlayerOptions.HasKey(PlayerOptions.Language)) {
                return languageDefaults[PlayerOptions.GetString(PlayerOptions.Language)];
            }

            return defaultKey;
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
            bindedKey = GetDefaultKey();

            if (toPlayerPrefs) {
                PlayerPrefs.SetString(name, bindedKey.ToString());
            }
        }
    }
}
