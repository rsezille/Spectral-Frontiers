using System;
using System.Collections.Generic;
using UnityEngine;

namespace SF {
    [CreateAssetMenu(menuName = "SF/Item")]
    public class Item : ScriptableObject {
        public string name;

        [Serializable]
        public struct StatModifier {
            public Stat stat;
            public int increase;
        }

        public List<StatModifier> statModifier;
    }
}
