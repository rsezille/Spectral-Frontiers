using System;
using System.Collections.Generic;
using UnityEngine;

namespace SF {
    [CreateAssetMenu(menuName = "SF/Systems/Inventory")]
    public class Inventory : ScriptableObject {
        [Serializable]
        public struct ItemStack {
            public Item item;
            public int quantity;
        }

        public List<ItemStack> itemStacks;
    }
}
