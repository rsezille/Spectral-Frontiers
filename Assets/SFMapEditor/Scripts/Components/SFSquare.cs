using UnityEngine;

public class SFSquare : MonoBehaviour {
    public int x;
    public int y;
    public int height = 0;
    public bool solid = false;

    public BoardEntity boardEntity;

    private SFTileContainer sfTileContainer;
    private SFEntityContainer sfEntityContainer;

    public bool IsNotBlocking() {
        return boardEntity == null;
    }

    public SFTileContainer tileContainer {
        get {
            if (sfTileContainer == null) {
                sfTileContainer = GetComponentInChildren<SFTileContainer>();
            }

            return sfTileContainer;
        }
    }

    public SFEntityContainer entityContainer {
        get {
            if (sfEntityContainer == null) {
                sfEntityContainer = GetComponentInChildren<SFEntityContainer>();
            }

            return sfEntityContainer;
        }
    }
}
