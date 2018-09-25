using System;
using UnityEngine;
using UnityEngine.Events;

namespace SF {
    public class SquareEventListener : MonoBehaviour {
        [Serializable]
        public class SquareUnityEvent : UnityEvent<Square> { }

        public SquareEvent squareEvent;
        public SquareUnityEvent response;

        private void OnEnable() {
            squareEvent.RegisterListener(this);
        }

        private void OnDisable() {
            squareEvent.UnregisterListener(this);
        }

        public void OnEventRaised(Square square) {
            response.Invoke(square);
        }
    }
}
