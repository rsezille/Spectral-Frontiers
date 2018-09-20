using UnityEngine;

namespace SF {
    [CreateAssetMenu(menuName = "SF/MissionVariable")]
    public class MissionVariable : ScriptableObject {
#if UNITY_EDITOR
        [Multiline]
        public string internalNotes = "";
#endif

        public RawMission value;
    }
}
