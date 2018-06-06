using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(World))]
public class WorldEditor : Editor {
    World world;

    private void OnEnable() {
        world = (World)target;
    }

    private void OnSceneGUI() {
        Debug.Log("Current event detected: " + Event.current.type);

        Event e = Event.current;

        if (world.drawMode) {
            if (e.isMouse && e.type == EventType.MouseDown && e.button == 0) {
                e.Use();

                Vector3 tilePosition = Camera.current.ScreenToWorldPoint(new Vector3(e.mousePosition.x, -e.mousePosition.y + Camera.current.pixelHeight));

                Instantiate(world.tile, new Vector3(tilePosition.x, tilePosition.y, Camera.main.transform.position.z), Quaternion.identity);

                
            }

            Selection.activeGameObject = world.gameObject;
        }
    }
}
