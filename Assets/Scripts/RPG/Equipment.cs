using System;

namespace SF {
    [Serializable]
    public struct Equipment {
        public EquippableItem mainHand;
        public EquippableItem offHand;
        public EquippableItem head;
        public EquippableItem body;
        public EquippableItem feet;
        public EquippableItem hands;
        public EquippableItem accessory;
    }
}
