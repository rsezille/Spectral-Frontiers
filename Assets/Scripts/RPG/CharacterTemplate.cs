using System;
using UnityEngine;

namespace SF {
    [Serializable]
    public struct TemplateStat {
        public Stat stat;
        public int baseValue;
        public int perLevelValue;
    }

    [CreateAssetMenu(menuName = "SF/Character")]
    public class CharacterTemplate : ScriptableObject {
        [Header("Basic")]
        public StringReference characterName;

        [Header("Stats")]
        public TemplateStat maxHP;
        public TemplateStat maxMP;
        public TemplateStat atk;
        public TemplateStat def;
        public TemplateStat mag;
        public TemplateStat eva;
        public TemplateStat acc;
        public TemplateStat spd;

        [Header("Display")]
        public GameObject spritePrefab;
        public int shadowSize;

        [Header("Battle")]
        [Tooltip("Number of times the character can do an action (basic attack/skill/spell/item) per turn")]
        public int actionTokens = 1;
        [Tooltip("Number of times the character can move per turn")]
        public int movementTokens = 1;
        [Tooltip("Number of squares the character can travel with one movement token")]
        public int movementPoints = 3;
        [Tooltip("Can be overloaded in missions ; used when the character is played by the computer")]
        public AI.Preset defaultAI = AI.Preset.Aggressive;
    }
}
