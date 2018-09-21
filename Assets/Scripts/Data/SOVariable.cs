using UnityEngine;

namespace SF {
    public abstract class SOVariable<T> : ScriptableObject {
#if UNITY_EDITOR
        [Multiline]
        public string internalNotes = "";
#endif

        public T value;
    }
}
