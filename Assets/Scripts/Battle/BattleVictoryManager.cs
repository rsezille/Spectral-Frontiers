public class BattleVictoryManager {
    private BattleManager battleManager; // Shortcut for BattleManager.instance

    public BattleVictoryManager() {
        battleManager = BattleManager.instance;
    }

    // Called by BattleManager
    public void Update() {}

    // Called by BattleManager
    public void EnterTurnStepNone(BattleManager.TurnStep previousTurnStep) {}

    public void EnterBattleStepVictory() {
        if (battleManager.GetSelectedPlayerBoardCharacter().outline != null) {
            battleManager.GetSelectedPlayerBoardCharacter().outline.enabled = false;
        }

        battleManager.currentBattleStep = BattleManager.BattleStep.Victory;
        
        battleManager.fightHUD.SetActiveWithAnimation(false);
        battleManager.victoryHUD.SetActiveWithAnimation(true);
    }
}
