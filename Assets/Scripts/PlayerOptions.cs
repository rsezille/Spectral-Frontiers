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

        public static Option Language = new Option("language", Globals.DefaultLanguage);
        public static Option BattleSpeed = new Option("battlespeed", "1.0");

        private static List<Option> initOptions = new List<Option>() {
            Language, BattleSpeed
        };

        private static Dictionary<string, string> options = new Dictionary<string, string>();

        public static void Load() {
            options = new Dictionary<string, string>();

            foreach (Option option in initOptions) {
                options.Add(option.key, PlayerPrefs.GetString(option.key, option.defaultValue));
            }

            foreach (KeyBind keyBind in InputManager.allKeyBinds) {
                keyBind.Bind(EnumUtil.ParseEnum(PlayerPrefs.GetString(keyBind.name, keyBind.defaultKey.ToString()), KeyCode.None), false);
            }
        }

        public static void Save() {
            PlayerPrefs.Save();
        }

        public static string GetString(string key) {
            if (options.ContainsKey(key)) {
                return options[key];
            }

            Debug.LogError("Key doesn't exist in PlayerPrefs: " + key);

            return key;
        }

        public static float GetFloat(string key) {
            if (options.ContainsKey(key)) {
                return float.Parse(options[key], CultureInfo.InvariantCulture);
            }

            Debug.LogError("Key doesn't exist in PlayerPrefs: " + key + " ; returning 0f");

            return 0f;
        }

        public static int GetInt(string key) {
            if (options.ContainsKey(key)) {
                return Int32.Parse(options[key]);
            }

            Debug.LogError("Key doesn't exist in PlayerPrefs: " + key + " ; returning 0");

            return 0;
        }

        public static void SetValue(string key, string value) {
            if (options.ContainsKey(key)) {
                options[key] = value;
            } else {
                options.Add(key, value);
            }

            PlayerPrefs.SetString(key, value);
        }

        public static void SetValue(string key, float value) {
            if (options.ContainsKey(key)) {
                options[key] = value.ToString("R");
            } else {
                options.Add(key, value.ToString("R"));
            }

            PlayerPrefs.SetString(key, value.ToString("R"));
        }

        public static void SetValue(string key, int value) {
            if (options.ContainsKey(key)) {
                options[key] = value.ToString();
            } else {
                options.Add(key, value.ToString());
            }

            PlayerPrefs.SetString(key, value.ToString());
        }
    }
}
