﻿using System.Collections.Generic;
using UnityEngine;

namespace SF {
    [CreateAssetMenu(menuName = "SF/EquippableItem")]
    public class EquippableItem : Item {
        public EquipmentType equipmentType;
        public List<StatModifier> statModifier;
    }
}
