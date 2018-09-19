using UnityEngine;

namespace SF {
    [CreateAssetMenu(menuName = "SF/StringVariable")]
    public class StringVariable : ScriptableObject {
#if UNITY_EDITOR
        [Multiline]
        public string internalNotes = "";
#endif

        public string value;
    }
}
