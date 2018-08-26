[System.Serializable]
public class RawMission {
    [System.Serializable]
    public class RawEnemy {
        public string key;
        public int level;
        public int posX;
        public int posY;
        public string direction = Globals.DefaultDirection.ToString();
    }

    [System.Serializable]
    public class RawStartingSquare {
        public int posX;
        public int posY;
        public string direction = Globals.DefaultDirection.ToString();
    }

    [System.Serializable]
    public class Action {
        public string type;
        public string[] args;
    }

    [System.Serializable]
    public class Background {
        public string preset;
        public string mainColor;
        public string off1Color;
        public string off2Color;
        public string type;
    }

    public string id;
    public string map;
    public int maxPlayerCharacters = 1; // Should be inferior to starting_squares length
    public bool isStoryline = false;
    public Background background; // Managed in Background class
    public string lighting; // Managed in SunLight class

    // Missions tree
    public string[] parents;
    public string[] children;

    public RawEnemy[] enemies;

    public Action[] openingCutscene;
    public Action[] endingCutscene;

    public RawStartingSquare[] startingSquares;
}
