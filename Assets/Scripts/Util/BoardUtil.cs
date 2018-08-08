using UnityEngine;

public static class BoardUtil {
    public static Vector3 CoordToWorldPosition(int x, int y) {
        return new Vector3(x - y, (x + y + 1f) / 2f, 0f);
    }
}
