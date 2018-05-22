[System.Serializable]
public class RawMap {
    [System.Serializable]
    public struct RawSquare {
        public int x_map;
        public int y_map;
        public string tile;
        public int v_offset;
        public bool solid;
        public bool start;
    }

    public string name;
    public int width;
    public int height;

    public RawSquare[] squares;
}
