[System.Serializable]
public class RawMission {
    [System.Serializable]
    public class RawEnemy {
        public string key;
        public int level;
        public int posX;
        public int posY;
    }

    [System.Serializable]
    public class RawStartingSquare {
        public int posX;
        public int posY;
        public string direction = BoardCharacter.Direction.East.ToString();
    }

    public string id;
    public string map;
    public int max_chars = 1;
    public bool is_storyline = false;

    // Missions tree
    public string[] parents;
    public string[] children;

    public RawEnemy[] enemies;

    public RawStartingSquare[] starting_squares;
}
