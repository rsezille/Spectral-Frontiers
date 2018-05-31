using System.Collections.Generic;

public class BattleFightManager {
    private BattleManager battleManager; // Shortcut for BattleManager.instance

    public BattleFightManager() {
        battleManager = BattleManager.instance;
    }

    // Called by BattleManager
    public void Update() {}

    // Called by BattleManager
    public void EnterTurnStepNone(BattleManager.TurnStep previousTurnStep) {
        if (previousTurnStep == BattleManager.TurnStep.Move) {
            battleManager.EventOnLeavingMove();
        }

        battleManager.fightHUD.Refresh();

        battleManager.battleCamera.SetPosition(battleManager.GetSelectedBoardChar().GetSquare(), true);
        //TODO: tocheck - battleManager.GetSelectedBoardChar().outline.enabled = true;
    }

    // Called by BattleManager
    public void EnterTurnStepStatus(BattleManager.TurnStep previousTurnStep) {}

    // Called by FightHUD
    public void Move() {
        if (battleManager.currentTurnStep == BattleManager.TurnStep.Move) {
            battleManager.EnterTurnStepNone();
        } else {
            if (battleManager.GetSelectedBoardChar().movable.CanMove()) {
                EnterTurnStepMove();
            }
        }
    }

    public void EnterBattleStepFight() {
        if (battleManager.playerBoardChars.Count > 0) {
            if (battleManager.playerPlacingChars[battleManager.placingCharIndex].boardChar != null) {
                battleManager.playerPlacingChars[battleManager.placingCharIndex].boardChar.outline.enabled = false;
            }

            battleManager.currentBattleStep = BattleManager.BattleStep.Fight;
            battleManager.placingHUD.SetActiveWithAnimation(false);
            battleManager.fightHUD.SetActiveWithAnimation(true);
            NewPlayerTurn();
        }
    }

    private void NewPlayerTurn() {
        battleManager.SetSelectedBoardChar(battleManager.playerBoardChars[0]);

        foreach (BoardChar bc in battleManager.playerBoardChars) {
            bc.NewTurn();
        }

        battleManager.EnterTurnStepNone();
    }

    private void EnterTurnStepMove() {
        battleManager.currentTurnStep = BattleManager.TurnStep.Move;

        List<Square> ts = new List<Square>();
        ts.Add(battleManager.GetSelectedBoardChar().GetSquare());

        List<Square> ts2 = new List<Square>();

        for (int i = 0; i < battleManager.GetSelectedBoardChar().movable.movementPoints; i++) {
            foreach (Square t in ts) {
                Square north = battleManager.board.GetSquare(t.x, t.y - 1);
                Square south = battleManager.board.GetSquare(t.x, t.y + 1);
                Square west = battleManager.board.GetSquare(t.x - 1, t.y);
                Square east = battleManager.board.GetSquare(t.x + 1, t.y);

                if (north != null && !north.isMovementMarked && north.IsNotBlocking()) {
                    north.MarkForMovement();
                    ts2.Add(north);
                }

                if (south != null && !south.isMovementMarked && south.IsNotBlocking()) {
                    south.MarkForMovement();
                    ts2.Add(south);
                }

                if (west != null && !west.isMovementMarked && west.IsNotBlocking()) {
                    west.MarkForMovement();
                    ts2.Add(west);
                }

                if (east != null && !east.isMovementMarked && east.IsNotBlocking()) {
                    east.MarkForMovement();
                    ts2.Add(east);
                }
            }

            ts.Clear();

            foreach (Square tt in ts2) {
                ts.Add(tt);
            }

            ts2.Clear();
        }
    }
}
