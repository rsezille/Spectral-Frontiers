using UnityEngine;
using UnityEngine.U2D;

public class SFMapEditor : MonoBehaviour {
    public Vector2Int size = new Vector2Int(3, 3);

    public bool showGrid = true;
    public Color gridColor = Color.grey;

    public SpriteAtlas currentAtlas;
    [Range(1, 64)]
    public int scrollStep = 1;

    public Sprite waterSprite;
    public Color waterColor = new Color(0.52f, 0.82f, 1f, 0.56f);
    public Color underwaterColor = new Color(0.7f, 0.78f, 1f, 1f);

    public GameObject map;

    private void OnDrawGizmos() {
        if (!map) map = GameObject.Find("Map") ?? new GameObject("Map");

        //map.transform.SetParent(transform);

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

    public void ResetWaterColor() {
        waterColor = new Color(0.52f, 0.82f, 1f, 0.56f);
        underwaterColor = new Color(0.7f, 0.78f, 1f, 1f);
    }
}
