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
        public string direction = Globals.DefaultDirection.ToString();
    }

    public string id;
    public string map;
    public int maxPlayerCharacters = 1; // Should be inferior to starting_squares length
    public bool isStoryline = false;

    // Missions tree
    public string[] parents;
    public string[] children;

    public RawEnemy[] enemies;

    public string[] openingCinematic;
    public string[] endingCinematic;

    public RawStartingSquare[] startingSquares;
}
