using UnityEngine;

public class SFSquare : MonoBehaviour {
    public int x;
    public int y;
    private int height = 0;
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

    public int Height {
        get {
            return height;
        }

        set {
            height = value;
            entityContainer.transform.position = new Vector3(0f, Height / Globals.TileHeight);
        }
    }
}
