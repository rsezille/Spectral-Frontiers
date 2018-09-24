using System.Collections.Generic;
using UnityEngine;

namespace SF {
    [CreateAssetMenu(menuName = "SF/Events/TurnStepEvent")]
    public class TurnStepEvent : ScriptableObject {
        private readonly List<TurnStepEventListener> listeners = new List<TurnStepEventListener>();

        public void Raise(BattleState.TurnStep turnStep) {
            for (int i = listeners.Count - 1; i >= 0; i--) {
                listeners[i].OnEventRaised(turnStep);
            }
        }

        public void RegisterListener(TurnStepEventListener listener) {
            if (!listeners.Contains(listener)) {
                listeners.Add(listener);
            }
        }

        public void UnregisterListener(TurnStepEventListener listener) {
            if (listeners.Contains(listener)) {
                listeners.Remove(listener);
            }
        }
    }
}
