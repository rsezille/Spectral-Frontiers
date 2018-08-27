using UnityEngine;

public static class BoardUtil {
    public static Vector3 CoordToWorldPosition(int x, int y, float height = 0f) {
        return new Vector3(x - y, ((x + y + 1f) / 2f) + height, 0f);
    }

    public static Vector3 CoordToWorldPosition(Square square) {
        return new Vector3(square.x - square.y, ((square.x + square.y + 1f) / 2f) + square.GetWorldHeight(), 0f);
    }
}
