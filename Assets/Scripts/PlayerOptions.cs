using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace SF {
    /**
     * PlayerPrefs wrapper
     */
    public static class PlayerOptions {
        public class Option {
            public string key;
            public string defaultValue { get; private set; }

            public Option(string key, string defaultValue) {
                this.key = key;
                this.defaultValue = defaultValue;
            }
        }

        public static Option Language = new Option("language", Globals.FallbackLanguage);
        public static Option BattleSpeed = new Option("battlespeed", "1.0");
        public static Option TextSpeed = new Option("textspeed", DialogBox.TextSpeed.Fast.ToString());

        private static List<Option> initOptions = new List<Option>() {
            Language, BattleSpeed, TextSpeed
        };

        private static Dictionary<string, string> options = new Dictionary<string, string>();

        public static void Load() {
            options = new Dictionary<string, string>();

            foreach (Option option in initOptions) {
                options.Add(option.key, PlayerPrefs.GetString(option.key, option.defaultValue));
            }

            foreach (KeyBind keyBind in InputManager.allKeyBinds) {
                keyBind.Bind(EnumUtil.ParseEnum(PlayerPrefs.GetString(keyBind.name, keyBind.GetDefaultKey().ToString()), KeyCode.None), false);
            }
        }

        public static void ResetKeyBinds() {
            foreach (KeyBind key in InputManager.allKeyBinds) {
                key.ResetKey();
            }
        }

        public static void Save() {
            PlayerPrefs.Save();
        }

        public static string GetString(Option option) {
            if (options.ContainsKey(option.key)) {
                return options[option.key];
            }

            Debug.LogError("Key doesn't exist in PlayerPrefs: " + option.key);

            return option.key;
        }

        public static float GetFloat(Option option) {
            if (options.ContainsKey(option.key)) {
                return float.Parse(options[option.key], CultureInfo.InvariantCulture);
            }

            Debug.LogError("Key doesn't exist in PlayerPrefs: " + option.key + " ; returning 0f");

            return 0f;
        }

        public static int GetInt(Option option) {
            if (options.ContainsKey(option.key)) {
                return Int32.Parse(options[option.key]);
            }

            Debug.LogError("Key doesn't exist in PlayerPrefs: " + option.key + " ; returning 0");

            return 0;
        }

        public static void SetValue(Option option, string value) {
            if (options.ContainsKey(option.key)) {
                options[option.key] = value;
            } else {
                options.Add(option.key, value);
            }

            PlayerPrefs.SetString(option.key, value);
        }

        public static void SetValue(Option option, float value) {
            if (options.ContainsKey(option.key)) {
                options[option.key] = value.ToString("R");
            } else {
                options.Add(option.key, value.ToString("R"));
            }

            PlayerPrefs.SetString(option.key, value.ToString("R"));
        }

        public static void SetValue(Option option, int value) {
            if (options.ContainsKey(option.key)) {
                options[option.key] = value.ToString();
            } else {
                options.Add(option.key, value.ToString());
            }

            PlayerPrefs.SetString(option.key, value.ToString());
        }

        public static bool HasKey(Option option) {
            return PlayerPrefs.HasKey(option.key);
        }
    }
}
