using UnityEngine;

namespace SF {
    [CreateAssetMenu(menuName = "SF/Systems/BattleState")]
    public class BattleState : ScriptableObject {
        public enum BattleStep {
            Cutscene, Placing, Fight, Victory
        };
        public enum TurnStep { // Placing: None or Status - Fight: None, Move, Attack, Skill, Item, Enemy, Status, Direction - Victory: None
            None, Move, Attack, Skill, Item, Enemy, Status, Direction
        };

        [Header("Events")]
        public BattleStepEvent LeaveBattleStepEvent;
        public BattleStepEvent EnterBattleStepEvent;

        [Header("Data")]
        [SerializeField]
        private BattleStep _currentBattleStep;
        public BattleStep currentBattleStep {
            get { return _currentBattleStep; }
            set {
                BattleStep previousBattleStep = _currentBattleStep;

                LeaveBattleStepEvent.Raise(_currentBattleStep);

                _currentBattleStep = value;

                EnterBattleStepEvent.Raise(_currentBattleStep);
            }
        }

        [SerializeField]
        private TurnStep _currentTurnStep;
        public TurnStep currentTurnStep {
            get { return _currentTurnStep; }
            set {
                if (_currentTurnStep != value) {
                    _currentTurnStep = value;
                    //turnHUD.Check();
                }
            }
        }
    }
}
