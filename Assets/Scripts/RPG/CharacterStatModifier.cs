namespace SF {
    public struct CharacterStatModifier {
        public float value;
        public StatModType type;
        public object source;

        public CharacterStatModifier(float value, StatModType type, object source) {
            this.value = value;
            this.type = type;
            this.source = source;
        }
    }
}
