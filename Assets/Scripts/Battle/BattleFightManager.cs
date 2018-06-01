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
            SelectPreviousPlayerBoardCharacter();

            if (battleManager.currentTurnStep == BattleManager.TurnStep.Status) {
                battleManager.statusHUD.Show(
                    battleManager.playerCharacters.IndexOf(battleManager.statusHUD.boardCharacter) <= 0 ?
                    battleManager.playerCharacters[battleManager.playerCharacters.Count - 1] :
                    battleManager.playerCharacters[battleManager.playerCharacters.IndexOf(battleManager.statusHUD.boardCharacter) - 1]
                );
            }
        } else if (Input.GetButtonDown(InputBinds.Next)) {
            SelectNextPlayerBoardCharacter();

            if (battleManager.currentTurnStep == BattleManager.TurnStep.Status) {
                battleManager.statusHUD.Show(
                    battleManager.playerCharacters.IndexOf(battleManager.statusHUD.boardCharacter) >= battleManager.playerCharacters.Count - 1 ?
                    battleManager.playerCharacters[0] :
                    battleManager.playerCharacters[battleManager.playerCharacters.IndexOf(battleManager.statusHUD.boardCharacter) + 1]
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

        battleManager.battleCamera.SetPosition(battleManager.GetSelectedPlayerBoardCharacter(), true);
    }

    // Called by BattleManager
    public void EnterTurnStepStatus(BattleManager.TurnStep previousTurnStep) {
        battleManager.fightHUD.SetActiveWithAnimation(false);

        battleManager.statusHUD.Show(battleManager.GetSelectedPlayerBoardCharacter());
    }

    // Called by FightHUD
    public void Move() {
        if (battleManager.currentTurnStep == BattleManager.TurnStep.Move) {
            battleManager.EnterTurnStepNone();
        } else {
            if (battleManager.GetSelectedPlayerBoardCharacter().movable != null && battleManager.GetSelectedPlayerBoardCharacter().movable.CanMove()) {
                EnterTurnStepMove();
            }
        }
    }

    // Called by FightHUD
    public void Previous() {
        battleManager.EnterTurnStepNone();

        SelectPreviousPlayerBoardCharacter();
    }

    // Called by FightHUD
    public void Next() {
        battleManager.EnterTurnStepNone();

        SelectNextPlayerBoardCharacter();
    }

    // Called by FightHUD
    public void Status() {
        battleManager.EnterTurnStepStatus();
    }

    public void EnterBattleStepFight() {
        if (battleManager.playerCharacters.Count > 0) {
            // Disable outlines from the PlacingStep
            if (battleManager.playerPlacingChars[battleManager.placingCharIndex].boardCharacter != null) {
                battleManager.playerPlacingChars[battleManager.placingCharIndex].boardCharacter.outline.enabled = false;
            }

            battleManager.currentBattleStep = BattleManager.BattleStep.Fight;
            battleManager.placingHUD.SetActiveWithAnimation(false);
            battleManager.fightHUD.SetActiveWithAnimation(true);
            NewPlayerTurn();
        }
    }

    private void NewPlayerTurn() {
        foreach (BoardCharacter bc in battleManager.playerCharacters) {
            bc.NewTurn();
        }

        battleManager.EnterTurnStepNone();

        battleManager.SetSelectedPlayerBoardCharacter(battleManager.playerCharacters[0]);
    }

    // Mark all squares where the character can move
    private void EnterTurnStepMove() {
        battleManager.currentTurnStep = BattleManager.TurnStep.Move;

        List<Square> ts = new List<Square>();
        ts.Add(battleManager.GetSelectedPlayerBoardCharacter().GetSquare());

        List<Square> ts2 = new List<Square>();

        for (int i = 0; i < battleManager.GetSelectedPlayerBoardCharacter().movable.movementPoints; i++) {
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

    private void SelectPreviousPlayerBoardCharacter() {
        BoardCharacter boardCharacter = battleManager.GetSelectedPlayerBoardCharacter();

        do {
            if (battleManager.playerCharacters.IndexOf(boardCharacter) == 0) {
                boardCharacter = battleManager.playerCharacters[battleManager.playerCharacters.Count - 1];
            } else {
                boardCharacter = battleManager.playerCharacters[battleManager.playerCharacters.IndexOf(boardCharacter) - 1];
            }
        } while (boardCharacter.IsDead());

        battleManager.SetSelectedPlayerBoardCharacter(boardCharacter);
    }

    private void SelectNextPlayerBoardCharacter() {
        BoardCharacter boardCharacter = battleManager.GetSelectedPlayerBoardCharacter();

        do {
            if (battleManager.playerCharacters.IndexOf(boardCharacter) >= battleManager.playerCharacters.Count - 1) {
                boardCharacter = battleManager.playerCharacters[0];
            } else {
                boardCharacter = battleManager.playerCharacters[battleManager.playerCharacters.IndexOf(boardCharacter) + 1];
            }
        } while (boardCharacter.IsDead());

        battleManager.SetSelectedPlayerBoardCharacter(boardCharacter);
    }
}
