using System.Collections.Generic;
using UnityEngine;

namespace SF {
    [CreateAssetMenu(menuName = "SF/Events/BattleStepEvent")]
    public class BattleStepEvent : ScriptableObject {
        private readonly List<BattleStepEventListener> listeners = new List<BattleStepEventListener>();

        public void Raise(BattleState.BattleStep battleStep) {
            for (int i = listeners.Count - 1; i >= 0; i--) {
                listeners[i].OnEventRaised(battleStep);
            }
        }

        public void RegisterListener(BattleStepEventListener listener) {
            if (!listeners.Contains(listener)) {
                listeners.Add(listener);
            }
        }

        public void UnregisterListener(BattleStepEventListener listener) {
            if (listeners.Contains(listener)) {
                listeners.Remove(listener);
            }
        }
    }
}
