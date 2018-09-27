using UnityEngine;
using UnityEngine.Events;

namespace SF {
    public class GameEventListener : MonoBehaviour {
        public GameEvent gameEvent;
        public UnityEvent response;

        private void OnEnable() {
            gameEvent.RegisterListener(this);
        }

        private void OnDisable() {
            gameEvent.UnregisterListener(this);
        }

        public void OnEventRaised() {
            response.Invoke();
        }
    }
}
