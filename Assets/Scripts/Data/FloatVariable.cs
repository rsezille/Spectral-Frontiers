using UnityEngine;

[CreateAssetMenu(menuName = "SF/FloatVariable")]
public class FloatVariable : ScriptableObject {
#if UNITY_EDITOR
    [Multiline]
    public string developerDescription = "";
#endif
    public float value;
}
