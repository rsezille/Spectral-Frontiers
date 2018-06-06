using UnityEngine;

public class World : MonoBehaviour {
    public Vector2Int size = new Vector2Int(3, 3);

    public bool showGrid = true;
    public bool drawMode = true;
    public Color gridColor = Color.grey;

    public GameObject tile;

    private void OnDrawGizmos() {
        size.Clamp(new Vector2Int(1, 1), new Vector2Int(1000, 1000));

        if (showGrid) {
            Gizmos.color = gridColor;

            for (int x = 0; x <= size.x; x++) {
                Gizmos.DrawLine(new Vector3(x, x / 2f), new Vector3(-size.y + x, (size.y + x) / 2f));
            }

            for (int y = 0; y <= size.y; y++) {
                Gizmos.DrawLine(new Vector3(-y, y / 2f), new Vector3(size.x - y, (size.x + y) / 2f));
            }
        }
    }
}
