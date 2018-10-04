using SF;

public class BattleVictoryManager {
    private BattleManager battleManager;

    public BattleVictoryManager(BattleManager battleManager) {
        this.battleManager = battleManager;
    }

    // Called by BattleManager
    public void Update() {
        if (InputManager.Special1.IsKeyDown) {
            battleManager.battleState.currentBattleStep = BattleState.BattleStep.Cutscene;
        }
    }
}
