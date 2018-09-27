using System;
using UnityEngine;
using UnityEngine.Events;

namespace SF {
    public class TurnStepEventListener : MonoBehaviour {
        [Serializable]
        public class TurnStepUnityEvent : UnityEvent<BattleState.TurnStep> { }

        public TurnStepEvent turnStepEvent;
        public TurnStepUnityEvent response;

        private void OnEnable() {
            turnStepEvent.RegisterListener(this);
        }

        private void OnDisable() {
            turnStepEvent.UnregisterListener(this);
        }

        public void OnEventRaised(BattleState.TurnStep turnStep) {
            response.Invoke(turnStep);
        }
    }
}
