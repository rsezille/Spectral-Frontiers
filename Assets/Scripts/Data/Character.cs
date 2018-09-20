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

        public StringReference playerName;

        public CharacterStat hp;
        public int currentHp;
        public CharacterStat atk;
        public CharacterStat spd;
    }
}
