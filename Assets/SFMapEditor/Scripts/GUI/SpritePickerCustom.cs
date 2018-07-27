using System.IO;
using UnityEditor;
using UnityEngine;

namespace SF {
    [CustomEditor(typeof(SpritePicker))]
    public class SpritePickerCustom : Editor {
        private SpritePicker sfSpritePicker;

        private MapEditor sfMapEditor; // Shortcut

        private Vector2 scrollPos;

        private void OnEnable() {
            sfSpritePicker = (SpritePicker)target;
            sfMapEditor = sfSpritePicker.GetComponent<MapEditor>();
        }

        public override void OnInspectorGUI() {
            Event e = Event.current;
            serializedObject.Update();

            GUILayout.Label("Sprite picker", EditorStyles.boldLabel);

            string[] subdirectories = Directory.GetDirectories(Application.dataPath + "/Resources/SFMapEditor/Tiles");

            for (int i = 0; i < subdirectories.Length; i++) {
                subdirectories[i] = System.IO.Path.GetFileName(subdirectories[i]);
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Tileset to use");

            int newTileset = EditorGUILayout.Popup(sfSpritePicker.selectedTileset, subdirectories);

            if (sfSpritePicker.tileset == null || sfSpritePicker.tileset.Length == 0 || newTileset != sfSpritePicker.selectedTileset) {
                sfSpritePicker.selectedTileset = newTileset;
                sfSpritePicker.selectedIndex = -1;
                sfSpritePicker.isEntity = subdirectories[sfSpritePicker.selectedTileset].Contains("Ent_");
                sfSpritePicker.tileset = Resources.LoadAll<GameObject>("SFMapEditor/Tiles/" + subdirectories[sfSpritePicker.selectedTileset]);
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.Label("(Subfolders in Resources/SFMapEditor/Tiles)");
            GUILayout.Label("/!\\ Entities must be placed in \"Ent_XXX\" folder", EditorStyles.boldLabel);

            if (sfSpritePicker.tileset != null && sfSpritePicker.tileset.Length > 0) {
                float layoutWidth = Screen.width - 15; // 15 is for the scrollbar
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(layoutWidth), GUILayout.Height(200));

                float currentWidth = 0f;
                float currentHeight = 0f;
                int sideLength = 75;

                for (int i = 0; i < sfSpritePicker.tileset.Length; i++) {
                    Sprite currentTile = sfSpritePicker.tileset[i].GetComponent<SpriteRenderer>().sprite;

                    if (currentTile == null) continue;

                    Rect spriteRect = new Rect(currentWidth, currentHeight, sideLength, sideLength);

                    if (e.type == EventType.MouseDown && e.button == 0 && spriteRect.Contains(e.mousePosition)) {
                        sfSpritePicker.selectedIndex = i;
                        sfMapEditor.useWater = false;
                    }

                    if (sfSpritePicker.selectedIndex == i) {
                        Texture2D selectedBackground = new Texture2D(1, 1);
                        selectedBackground.SetPixel(0, 0, new Color(1f, 1f, 0.35f, 0.5f));
                        selectedBackground.wrapMode = TextureWrapMode.Repeat;
                        selectedBackground.Apply();
                        GUI.DrawTexture(spriteRect, selectedBackground);
                    }

                    GUI.DrawTexture(spriteRect, currentTile.texture, ScaleMode.ScaleToFit);

                    currentWidth += sideLength;

                    if (i < sfSpritePicker.tileset.Length - 1 && currentWidth + sideLength >= layoutWidth) {
                        currentWidth = 0f;
                        currentHeight += sideLength;
                    }
                }

                EditorGUILayout.EndScrollView();
            }

            GUILayout.Label("Water", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("waterPrefab"), new GUIContent("GameObject"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("waterColor"), new GUIContent("Color"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("underwaterColor"), new GUIContent("Underwater color"));

            if (GUILayout.Button("Reset colors", GUILayout.Width(150))) {
                sfSpritePicker.ResetWaterColor();
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("waterOffset"), new GUIContent("Offset (32)"));

            var origFontStyle = EditorStyles.label.fontStyle;
            EditorStyles.label.fontStyle = FontStyle.Bold;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("tileSelectorPrefab"), new GUIContent("Tile Selector"));
            EditorStyles.label.fontStyle = origFontStyle;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
