using System.Collections.Generic;
using UnityEngine;

namespace SF {
    [CreateAssetMenu(menuName = "SF/Events/SquareEvent")]
    public class SquareEvent : ScriptableObject {
        private readonly List<SquareEventListener> listeners = new List<SquareEventListener>();

        public void Raise(Square square) {
            for (int i = listeners.Count - 1; i >= 0; i--) {
                listeners[i].OnEventRaised(square);
            }
        }

        public void RegisterListener(SquareEventListener listener) {
            if (!listeners.Contains(listener)) {
                listeners.Add(listener);
            }
        }

        public void UnregisterListener(SquareEventListener listener) {
            if (listeners.Contains(listener)) {
                listeners.Remove(listener);
            }
        }
    }
}
