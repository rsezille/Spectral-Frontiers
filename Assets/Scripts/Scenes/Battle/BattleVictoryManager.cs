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
        if (battleManager.fight.selectedPlayerCharacter.outline != null) {
            battleManager.fight.selectedPlayerCharacter.outline.enabled = false;
        }

        battleManager.currentBattleStep = BattleManager.BattleStep.Victory;
        
        battleManager.fightHUD.SetActiveWithAnimation(false);
        battleManager.victoryHUD.SetActiveWithAnimation(true);
    }
}
