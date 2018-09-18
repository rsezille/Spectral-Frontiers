using UnityEngine;

[CreateAssetMenu(menuName = "SF/FloatVariable")]
public class FloatVariable : ScriptableObject {
#if UNITY_EDITOR
    [Multiline]
    public string internalNotes = "";
#endif
    public float value;
}
