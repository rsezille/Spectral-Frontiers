using System;
using UnityEngine;

namespace SF {
    [CreateAssetMenu(menuName = "SF/Data/Mission")]
    public class Mission : ScriptableObject {
        [Serializable]
        public class Background {
            public string preset;
            public Color mainColor = new Vector4(0.43f, 0.68f, 0.83f, 1f);
            public Color off1Color = new Vector4(0.22f, 0.36f, 0.88f, 1f);
            public Color off2Color = new Vector4(1f, 1f, 1f, 1f);
            public SF.Background.Type type = SF.Background.Type.Gradient;
        }

        [Serializable]
        public struct StartingSquare {
            public int posX;
            public int posY;
            public BoardCharacter.Direction direction;
        }

        [Serializable]
        public struct Enemy {
            public CharacterTemplate character;
            public string overloadedName;
            public int level;
            public int posX;
            public int posY;
            public BoardCharacter.Direction direction;
        }

        [Serializable]
        public struct CutsceneAction {
            public BattleCutsceneManager.ActionType type;
            public string[] args;
        }

        public Map map;
        [Tooltip("Should be inferior to the number of starting squares")]
        public int maxPlayerCharacters = 1;
        public StartingSquare[] startingSquares;
        public Background background;
        public SunSettings.LightingType lighting = SunSettings.LightingType.Day;

        // Missions tree
        public Mission[] parents;
        public Mission[] children;

        public Enemy[] enemies;

        public CutsceneAction[] openingCutscene;
        public CutsceneAction[] endingCutscene;
    }
}
