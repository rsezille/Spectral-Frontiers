using UnityEngine;

namespace SF {
    [CreateAssetMenu(menuName = "SF/Systems/BattleState")]
    public class BattleState : ScriptableObject {
        public enum BattleStep {
            None, Cutscene, Placing, Fight, Victory
        };
        public enum TurnStep { // Placing: None or Status - Fight: None, Move, Attack, Skill, Item, Enemy, Status, Direction - Victory: None
            None, Move, Attack, Skill, Item, Enemy, Status, Direction
        };

        [Header("Events")]
        public BattleStepEvent LeaveBattleStepEvent;
        public BattleStepEvent EnterBattleStepEvent;
        public TurnStepEvent LeaveTurnStepEvent;
        public TurnStepEvent EnterTurnStepEvent;

        [Header("Data")]
        [SerializeField]
        private BattleStep _currentBattleStep;
        public BattleStep currentBattleStep {
            get { return _currentBattleStep; }
            set {
                LeaveBattleStepEvent.Raise(_currentBattleStep);

                previousBattleStep = _currentBattleStep;
                _currentBattleStep = value;

                EnterBattleStepEvent.Raise(_currentBattleStep);
            }
        }

        [SerializeField]
        private BattleStep _previousBattleStep;
        public BattleStep previousBattleStep {
            private set {
                _previousBattleStep = value;
            }
            get {
                return _previousBattleStep;
            }
        }

        [SerializeField]
        private TurnStep _currentTurnStep;
        public TurnStep currentTurnStep {
            get { return _currentTurnStep; }
            set {
                if (_currentTurnStep != value) {
                    LeaveTurnStepEvent.Raise(_currentTurnStep);

                    previousTurnStep = _currentTurnStep;
                    _currentTurnStep = value;

                    EnterTurnStepEvent.Raise(_currentTurnStep);
                }
            }
        }

        [SerializeField]
        private TurnStep _previousTurnStep;
        public TurnStep previousTurnStep {
            private set {
                _previousTurnStep = value;
            }
            get {
                return _previousTurnStep;
            }
        }

        public void ResetData() {
            _currentBattleStep = BattleStep.None;
            _currentTurnStep = TurnStep.None;
        }
    }
}
