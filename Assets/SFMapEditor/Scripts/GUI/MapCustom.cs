using UnityEditor;
using UnityEngine;

namespace SF {
    [CustomEditor(typeof(Map))]
    public class MapCustom : Editor {
        Map sfMap;

        private void OnEnable() {
            sfMap = (Map)target;
        }

        public override void OnInspectorGUI() {
            GUILayout.Label("/!\\ Only refresh prefabs on instantiated map", EditorStyles.boldLabel);
            GUILayout.Label("/!\\ Do NOT do it on the prefab", EditorStyles.boldLabel);

            if (GUILayout.Button("Refresh nested prefabs (tiles)")) {
                SpriteRenderer[] children = sfMap.GetComponentsInChildren<SpriteRenderer>();

                foreach (SpriteRenderer spriteRenderer in children) {
                    GameObject oldTile = spriteRenderer.gameObject;

                    GameObject newTile = Instantiate(Resources.Load("SFMapEditor/Tiles/" + spriteRenderer.sprite.name) as GameObject, oldTile.transform.position, oldTile.transform.rotation);

                    if (oldTile.transform.parent != null) {
                        newTile.transform.SetParent(oldTile.transform.parent);
                    }

                    DestroyImmediate(oldTile);
                }
            }
        }
    }
}
