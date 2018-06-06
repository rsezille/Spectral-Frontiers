using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(World))]
public class WorldEditor : Editor {
    World world;

    private void OnEnable() {
        world = (World)target;
    }

    private void OnSceneGUI() {
        Event e = Event.current;

        if (world.drawMode) {
            if (e.isMouse && e.type == EventType.MouseDown && e.button == 0) {
                e.Use();

                Vector3 mousePosition = new Vector3(e.mousePosition.x, -e.mousePosition.y + Camera.current.pixelHeight);
                Vector3 localMousePos = Camera.current.ScreenToWorldPoint(mousePosition);

                Debug.Log(localMousePos);

                // Square coords
                int Sx = Mathf.FloorToInt((localMousePos.x / 2) + localMousePos.y);
                int Sy = Mathf.FloorToInt(localMousePos.y - (localMousePos.x / 2));

                // Center of the square
                float Cx = Sx - Sy;
                float Cy = (Sx + Sy + 1f) / 2f;

                Debug.Log("Square: " + Sx + "," + Sy + " / Center: " + Cx + "," + Cy);

                GameObject go = Instantiate(world.tile, new Vector3(Cx, Cy, Camera.main.transform.position.z), Quaternion.identity);
                go.GetComponent<SpriteRenderer>().sortingOrder = -(world.size.y * Sy + Sx);

            }

            Selection.activeGameObject = world.gameObject;
        }
    }
}
