using SF;

public class BattleVictoryManager {
    private BattleManager battleManager; // Shortcut for BattleManager.instance

    public BattleVictoryManager() {
        battleManager = BattleManager.instance;
    }

    // Called by BattleManager
    public void Update() {
        if (InputManager.Special1.IsKeyDown) {
            Next();
        }
    }

    public void Next() {
        battleManager.currentBattleStep = BattleManager.BattleStep.Cutscene;
    }

    // Called by BattleManager
    public void EnterTurnStepNone(BattleManager.TurnStep previousTurnStep) {}

    // Called by BattleManager
    public void EnterBattleStepVictory() {
        battleManager.victoryHUD.SetActiveWithAnimation(true);
    }

    // Called by BattleManager
    public void LeaveBattleStepVictory() {
        battleManager.victoryHUD.SetActiveWithAnimation(false);
    }
}
