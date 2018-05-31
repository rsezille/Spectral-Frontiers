using System.Collections.Generic;
using UnityEngine;

public class BattleFightManager {
    private BattleManager battleManager; // Shortcut for BattleManager.instance

    public BattleFightManager() {
        battleManager = BattleManager.instance;
    }

    // Called by BattleManager
    public void Update() {
        if (Input.GetButtonDown(InputBinds.Previous)) {
            SelectPreviousPlayerBoardChar();

            if (battleManager.currentTurnStep == BattleManager.TurnStep.Status) {
                battleManager.statusHUD.Show(
                    battleManager.playerBoardChars.IndexOf(battleManager.statusHUD.boardChar) <= 0 ?
                    battleManager.playerBoardChars[battleManager.playerBoardChars.Count - 1] :
                    battleManager.playerBoardChars[battleManager.playerBoardChars.IndexOf(battleManager.statusHUD.boardChar) - 1]
                );
            }
        } else if (Input.GetButtonDown(InputBinds.Next)) {
            SelectNextPlayerBoardChar();

            if (battleManager.currentTurnStep == BattleManager.TurnStep.Status) {
                battleManager.statusHUD.Show(
                    battleManager.playerBoardChars.IndexOf(battleManager.statusHUD.boardChar) >= battleManager.playerBoardChars.Count - 1 ?
                    battleManager.playerBoardChars[0] :
                    battleManager.playerBoardChars[battleManager.playerBoardChars.IndexOf(battleManager.statusHUD.boardChar) + 1]
                );
            }
        }
    }

    // Called by BattleManager
    public void EnterTurnStepNone(BattleManager.TurnStep previousTurnStep) {
        switch (previousTurnStep) {
            case BattleManager.TurnStep.Move:
                battleManager.EventOnLeavingMove();
                break;
            case BattleManager.TurnStep.Status:
                battleManager.fightHUD.SetActiveWithAnimation(true);
                break;
        }

        battleManager.fightHUD.Refresh();

        battleManager.battleCamera.SetPosition(battleManager.GetSelectedBoardChar().GetSquare(), true);
        //TODO: tocheck - battleManager.GetSelectedBoardChar().outline.enabled = true;
    }

    // Called by BattleManager
    public void EnterTurnStepStatus(BattleManager.TurnStep previousTurnStep) {
        battleManager.fightHUD.SetActiveWithAnimation(false);

        battleManager.statusHUD.Show(battleManager.GetSelectedBoardChar());
    }

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

    // Called by FightHUD
    public void Previous() {
        battleManager.EnterTurnStepNone();

        SelectPreviousPlayerBoardChar();
    }

    // Called by FightHUD
    public void Next() {
        battleManager.EnterTurnStepNone();

        SelectNextPlayerBoardChar();
    }

    // Called by FightHUD
    public void Status() {
        battleManager.EnterTurnStepStatus();
    }

    public void EnterBattleStepFight() {
        if (battleManager.playerBoardChars.Count > 0) {
            // Disable outlines from the PlacingStep
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
        foreach (BoardChar bc in battleManager.playerBoardChars) {
            bc.NewTurn();
        }

        battleManager.EnterTurnStepNone();

        battleManager.SetSelectedBoardChar(battleManager.playerBoardChars[0]);
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

    private void SelectPreviousPlayerBoardChar() {
        BoardChar boardChar = battleManager.GetSelectedBoardChar();

        do {
            if (battleManager.playerBoardChars.IndexOf(boardChar) == 0) {
                boardChar = battleManager.playerBoardChars[battleManager.playerBoardChars.Count - 1];
            } else {
                boardChar = battleManager.playerBoardChars[battleManager.playerBoardChars.IndexOf(boardChar) - 1];
            }
        } while (boardChar.IsDead());

        battleManager.SetSelectedBoardChar(boardChar);
    }

    private void SelectNextPlayerBoardChar() {
        BoardChar boardChar = battleManager.GetSelectedBoardChar();

        do {
            if (battleManager.playerBoardChars.IndexOf(boardChar) >= battleManager.playerBoardChars.Count - 1) {
                boardChar = battleManager.playerBoardChars[0];
            } else {
                boardChar = battleManager.playerBoardChars[battleManager.playerBoardChars.IndexOf(boardChar) + 1];
            }
        } while (boardChar.IsDead());

        battleManager.SetSelectedBoardChar(boardChar);
    }
}
