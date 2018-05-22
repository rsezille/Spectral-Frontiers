[System.Serializable]
public class RawMission {
    [System.Serializable]
    public class RawEnemy {
        public string key;
        public int level;
        public int posX;
        public int posY;
    }

    public string id;
    public string map;
    public int max_chars = 1;
    public bool is_storyline = false;

    // Missions tree
    public string[] parents;
    public string[] children;

    public RawEnemy[] enemies;
}
