using UnityEngine;

namespace SF {
    [CreateAssetMenu(menuName = "SF/Item")]
    public class Item : ScriptableObject {
        public Sprite icon;
        public string name;
        public string description;
    }
}
