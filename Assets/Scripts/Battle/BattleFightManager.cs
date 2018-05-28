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

    public void EnterBattleStepFight() {
        if (battleManager.playerBoardChars.Count > 0) {
            if (battleManager.placingAlliedChars[battleManager.placingCharIndex].boardChar != null) {
                battleManager.placingAlliedChars[battleManager.placingCharIndex].boardChar.outline.enabled = false;
            }

            battleManager.currentBattleStep = BattleManager.BattleStep.Fight;
            battleManager.placingHUD.gameObject.SetActive(false);
            battleManager.fightHUD.gameObject.SetActive(true);
            battleManager.NewPlayerTurn();
        }
    }

    private void EnterTurnStepMove() {

    }
}
