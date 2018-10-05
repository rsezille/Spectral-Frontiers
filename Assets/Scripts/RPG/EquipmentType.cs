using UnityEngine;

namespace SF {
    [CreateAssetMenu(menuName = "SF/EquipmentType")]
    public class EquipmentType : ScriptableObject {
        public string name;
        public EquipmentSlot slot;
        [Tooltip("Used for weapons only")]
        public bool twoHanded = false;
    }
}
