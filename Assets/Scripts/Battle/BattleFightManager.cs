﻿using System.Collections.Generic;

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
        battleManager.currentTurnStep = BattleManager.TurnStep.Move;

        List<Square> ts = new List<Square>();
        ts.Add(battleManager.currentBoardChar.boardEntity.square);

        List<Square> ts2 = new List<Square>();

        for (int i = 0; i < battleManager.currentBoardChar.movable.movementPoints; i++) {
            foreach (Square t in ts) {
                Square north = battleManager.board.GetSquare(t.x, t.y - 1);
                Square south = battleManager.board.GetSquare(t.x, t.y + 1);
                Square west = battleManager.board.GetSquare(t.x - 1, t.y);
                Square east = battleManager.board.GetSquare(t.x + 1, t.y);

                if (north != null && !north.isMovementMarked && north.boardEntity == null) {
                    north.MarkForMovement();
                    ts2.Add(north);
                }

                if (south != null && !south.isMovementMarked && south.boardEntity == null) {
                    south.MarkForMovement();
                    ts2.Add(south);
                }

                if (west != null && !west.isMovementMarked && west.boardEntity == null) {
                    west.MarkForMovement();
                    ts2.Add(west);
                }

                if (east != null && !east.isMovementMarked && east.boardEntity == null) {
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
