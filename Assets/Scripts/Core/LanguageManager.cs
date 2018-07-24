using System;
using System.Collections.Generic;
using UnityEngine;

namespace SF {
    public class LanguageManager {
        [Serializable]
        private struct LangFile {
            [Serializable]
            public struct Pair {
                public string id;
                public string text;
            }

            public Pair[] list;
        }

        public static LanguageManager instance { private set; get; } = new LanguageManager();

        // Load both default language & desired one (default is mainly used for fallback if keys not found)
        private Dictionary<string, string> defaultStrings = new Dictionary<string, string>();
        private Dictionary<string, string> defaultDialogs = new Dictionary<string, string>();
        private Dictionary<string, string> currentStrings = new Dictionary<string, string>();
        private Dictionary<string, string> currentDialogs = new Dictionary<string, string>();

        private LanguageManager() { }

        public void LoadDefaultLanguage() {
            Debug.Log("Loading default language... [" + Globals.DefaultLanguage + "]");
            LoadLanguage(Globals.DefaultLanguage);
            Debug.Log("End of loading default language... [" + Globals.DefaultLanguage + "]");
        }

        /**
         * Load the given language
         * @param languageCode - 'fr', 'en', etc.
         */
        public void LoadLanguage(string languageCode) {
            if (String.IsNullOrEmpty(languageCode)) {
                Debug.LogError("Language code incorrect");

                return;
            }

            currentStrings.Clear();
            currentDialogs.Clear();

            TextAsset jsonStrings = Resources.Load("Languages/" + languageCode + "/strings") as TextAsset;

            if (jsonStrings == null) {
                Debug.LogError("Strings file not found for selected lang: " + languageCode);

                return;
            }

            LangFile strings = JsonUtility.FromJson<LangFile>(jsonStrings.text);

            foreach (LangFile.Pair pair in strings.list) {
                if (Globals.DefaultLanguage.Equals(languageCode)) {
                    if (!defaultStrings.ContainsKey(pair.id)) {
                        defaultStrings.Add(pair.id, pair.text);
                    }
                } else {
                    if (!currentStrings.ContainsKey(pair.id)) {
                        currentStrings.Add(pair.id, pair.text);
                    }
                }
            }

            TextAsset jsonDialogs = Resources.Load("Languages/" + languageCode + "/dialogs") as TextAsset;

            if (jsonDialogs == null) {
                Debug.LogError("Dialogs file not found for selected lang: " + languageCode);

                return;
            }

            LangFile dialogs = JsonUtility.FromJson<LangFile>(jsonDialogs.text);

            foreach (LangFile.Pair pair in dialogs.list) {
                if (Globals.DefaultLanguage.Equals(languageCode)) {
                    if (!defaultDialogs.ContainsKey(pair.id)) {
                        defaultDialogs.Add(pair.id, pair.text);
                    }
                } else {
                    if (!currentDialogs.ContainsKey(pair.id)) {
                        currentDialogs.Add(pair.id, pair.text);
                    }
                }
            }
        }

        /**
         * Return the string corresponding to the id ; return the id if no value is found
         * @param id - The key of the string
         * @return The value of the key (id) in the current language (or the default one if not found)
         */
        public string GetString(string id) {
            if (currentStrings.ContainsKey(id)) {
                return currentStrings[id];
            }

            if (defaultStrings.ContainsKey(id)) {
                return defaultStrings[id];
            }

            Debug.LogWarning("Specified string id doesnt exist: " + id);

            return id;
        }

        /**
         * Should be used only in DialogBox
         * @return The value of the key (id) in the current language (or the default one if not found)
         */
        public string GetDialog(string id) {
            if (currentDialogs.ContainsKey(id)) {
                return currentDialogs[id];
            }

            if (defaultDialogs.ContainsKey(id)) {
                return defaultDialogs[id];
            }

            Debug.LogWarning("Specified string id doesnt exist: " + id);

            return id;
        }
    }
}
