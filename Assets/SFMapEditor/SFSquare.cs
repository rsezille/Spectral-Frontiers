using UnityEngine;

public class SFSquare : MonoBehaviour {
    public int x;
    public int y;
    public int altitude = 0;

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;

        Gizmos.DrawLine(new Vector3(0, 0), new Vector3(-1, 0.5f));
        Gizmos.DrawLine(new Vector3(0, 0), new Vector3(1, 0.5f));
        Gizmos.DrawLine(new Vector3(-1, 0.5f), new Vector3(0, 1f));
        Gizmos.DrawLine(new Vector3(1, 0.5f), new Vector3(0, 1f));
    }
}
