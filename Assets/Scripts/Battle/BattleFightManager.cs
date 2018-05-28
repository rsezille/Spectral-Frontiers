public class BattleFightManager {
    private BattleManager battleManager; // Shortcut for BattleManager.instance

    public BattleFightManager() {
        battleManager = BattleManager.instance;
    }

    // Called by BattleManager
    public void Update() {}

    // Called by BattleManager
    public void EnterTurnStepNone() {
        battleManager.battleCamera.SetPosition(battleManager.currentBoardChar.boardEntity.square, true);
        battleManager.currentBoardChar.outline.enabled = true;
    }

    // Called by BattleManager
    public void EnterTurnStepStatus() {}

    // Called by FightHUD
    public void Move() {
        if (battleManager.currentTurnStep == BattleManager.TurnStep.Move) {
            battleManager.EnterTurnStepNone();
        } else {
            if (battleManager.currentBoardChar.movable.CanMove()) {
                EnterTurnStepMove();
            }
        }
    }

    private void EnterTurnStepMove() {

    }
}
