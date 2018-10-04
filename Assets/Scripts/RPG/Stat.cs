using UnityEngine;

[CreateAssetMenu(menuName = "SF/Stat")]
public class Stat : ScriptableObject {
    public string name;

    public int maxValue;
    public int minValue;
}
