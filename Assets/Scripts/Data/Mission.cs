using System;
using UnityEngine;

namespace SF {
    [CreateAssetMenu(menuName = "SF/Data/Mission")]
    public class Mission : ScriptableObject {
        [Serializable]
        public class Background {
            public string preset;
            public Color mainColor;
            public Color off1Color;
            public Color off2Color;
            public string type;
        }

        [Serializable]
        public class StartingSquare {
            public int posX;
            public int posY;
            public BoardCharacter.Direction direction;
        }

        [Serializable]
        public class Enemy {
            public Character character;
            public int level;
            public int posX;
            public int posY;
            public BoardCharacter.Direction direction;
        }

        [Serializable]
        public class CutsceneAction {
            public string type;
            public string[] args;
        }

        public Map map;
        [Tooltip("Should be inferior to the number of starting squares")]
        public int maxPlayerCharacters = 1;
        public StartingSquare[] startingSquares;
        public Background background;
        public string lighting;

        // Missions tree
        public Mission[] parents;
        public Mission[] children;

        public Enemy[] enemies;

        public CutsceneAction[] openingCutscene;
        public CutsceneAction[] endingCutscene;
    }
}
