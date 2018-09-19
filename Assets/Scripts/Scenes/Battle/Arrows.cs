using SF;
using UnityEngine;
using UnityEngine.Events;

/**
 * TODO [FINAL] May have a leak, remove listeners OnDestroy
 */
public class Arrows : MonoBehaviour {
    private BattleManager battleManager; // Shortcut

    [Header("Dependencies")]
    public BattleState battleState;

    [Header("References")]
    public GameObject north;
    public GameObject west;
    public GameObject east;
    public GameObject south;

    private void Awake() {
        battleManager = BattleManager.instance;

        AddMouseReactive(north, BoardCharacter.Direction.North);
        AddMouseReactive(west, BoardCharacter.Direction.West);
        AddMouseReactive(east, BoardCharacter.Direction.East);
        AddMouseReactive(south, BoardCharacter.Direction.South);
    }

    private void AddMouseReactive(GameObject arrow, BoardCharacter.Direction direction) {
        MouseReactive reactive = arrow.AddComponent<MouseReactive>();
        reactive.MouseEnter = new UnityEvent();
        reactive.Click = new UnityEvent();

        reactive.MouseEnter.AddListener(() => ChangeDirection(direction));
        reactive.Click.AddListener(battleManager.fight.EndTurnStepDirection);
    }

    private void ChangeDirection(BoardCharacter.Direction direction) {
        if (battleManager.currentBattleStep == BattleManager.BattleStep.Fight && battleState.currentTurnStep == BattleState.TurnStep.Direction) {
            battleManager.fight.selectedPlayerCharacter.direction = direction;
        }
    }
}
