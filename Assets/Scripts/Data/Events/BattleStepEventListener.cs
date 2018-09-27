using System;
using UnityEngine;
using UnityEngine.Events;

namespace SF {
    public class BattleStepEventListener : MonoBehaviour {
        [Serializable]
        public class BattleStepUnityEvent : UnityEvent<BattleState.BattleStep> { }

        public BattleStepEvent battleStepEvent;
        public BattleStepUnityEvent response;

        private void OnEnable() {
            battleStepEvent.RegisterListener(this);
        }

        private void OnDisable() {
            battleStepEvent.UnregisterListener(this);
        }

        public void OnEventRaised(BattleState.BattleStep battleStep) {
            response.Invoke(battleStep);
        }
    }
}
