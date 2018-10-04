using System;

namespace SF {
    public enum StatModType {
        Flat, PercentAdd, PercentMult
    }

    [Serializable]
    public struct StatModifier {
        public Stat stat;
        public float value;
        public StatModType type;
    }
}
