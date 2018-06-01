using UnityEngine;
using UnityEngine.Events;

public class MouseReactive : MonoBehaviour {
    [HideInInspector]
    public UnityEvent MouseEnter;
    [HideInInspector]
    public UnityEvent MouseLeave;
    [HideInInspector]
    public UnityEvent Click;
}
