using UnityEngine;

public class SFSquare : MonoBehaviour {
    public int x;
    public int y;
    public int height = 0;
    public bool solid = false;

    public BoardEntity boardEntity;

    public bool IsNotBlocking() {
        return boardEntity == null;
    }
}
