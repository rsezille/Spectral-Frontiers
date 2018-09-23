using System;
using UnityEngine;

namespace SF {
    [CreateAssetMenu(menuName = "SF/Character")]
    public class Character : ScriptableObject {
        [Serializable]
        public struct CharacterStat {
            public Stat statType;
            public int currentValue;
        }

        [HideInInspector]
        public BoardCharacter boardCharacter;

        [Header("Basic")]
        public StringReference characterName;

        [Header("Stats")]
        public CharacterStat maxHP;
        public int currentHp;
        public CharacterStat atk;
        public CharacterStat spd;

        [Header("Display")]
        public GameObject spritePrefab;
        public int shadowSize;

        [Header("Battle")]
        [Tooltip("Can be overloaded in missions ; used when the character is played by the computer")]
        public AI.Preset defaultAI = AI.Preset.Aggressive;
    }
}
