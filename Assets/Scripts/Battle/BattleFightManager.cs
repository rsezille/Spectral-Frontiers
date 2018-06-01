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
            SelectPreviousPlayerCharacter();

            if (battleManager.currentTurnStep == BattleManager.TurnStep.Status) {
                battleManager.statusHUD.Show(
                    battleManager.playerCharacters.IndexOf(battleManager.statusHUD.playerCharacter) <= 0 ?
                    battleManager.playerCharacters[battleManager.playerCharacters.Count - 1] :
                    battleManager.playerCharacters[battleManager.playerCharacters.IndexOf(battleManager.statusHUD.playerCharacter) - 1]
                );
            }
        } else if (Input.GetButtonDown(InputBinds.Next)) {
            SelectNextPlayerCharacter();

            if (battleManager.currentTurnStep == BattleManager.TurnStep.Status) {
                battleManager.statusHUD.Show(
                    battleManager.playerCharacters.IndexOf(battleManager.statusHUD.playerCharacter) >= battleManager.playerCharacters.Count - 1 ?
                    battleManager.playerCharacters[0] :
                    battleManager.playerCharacters[battleManager.playerCharacters.IndexOf(battleManager.statusHUD.playerCharacter) + 1]
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

        battleManager.battleCamera.SetPosition(battleManager.GetSelectedPlayerCharacter(), true);
    }

    // Called by BattleManager
    public void EnterTurnStepStatus(BattleManager.TurnStep previousTurnStep) {
        battleManager.fightHUD.SetActiveWithAnimation(false);

        battleManager.statusHUD.Show(battleManager.GetSelectedPlayerCharacter());
    }

    // Called by FightHUD
    public void Move() {
        if (battleManager.currentTurnStep == BattleManager.TurnStep.Move) {
            battleManager.EnterTurnStepNone();
        } else {
            if (battleManager.GetSelectedPlayerCharacter().movable.CanMove()) {
                EnterTurnStepMove();
            }
        }
    }

    // Called by FightHUD
    public void Previous() {
        battleManager.EnterTurnStepNone();

        SelectPreviousPlayerCharacter();
    }

    // Called by FightHUD
    public void Next() {
        battleManager.EnterTurnStepNone();

        SelectNextPlayerCharacter();
    }

    // Called by FightHUD
    public void Status() {
        battleManager.EnterTurnStepStatus();
    }

    public void EnterBattleStepFight() {
        if (battleManager.playerCharacters.Count > 0) {
            // Disable outlines from the PlacingStep
            if (battleManager.playerPlacingChars[battleManager.placingCharIndex].playerCharacter != null) {
                battleManager.playerPlacingChars[battleManager.placingCharIndex].playerCharacter.outline.enabled = false;
            }

            battleManager.currentBattleStep = BattleManager.BattleStep.Fight;
            battleManager.placingHUD.SetActiveWithAnimation(false);
            battleManager.fightHUD.SetActiveWithAnimation(true);
            NewPlayerTurn();
        }
    }

    private void NewPlayerTurn() {
        foreach (PlayerCharacter pc in battleManager.playerCharacters) {
            pc.NewTurn();
        }

        battleManager.EnterTurnStepNone();

        battleManager.SetSelectedPlayerCharacter(battleManager.playerCharacters[0]);
    }

    // Mark all squares where the character can move
    private void EnterTurnStepMove() {
        battleManager.currentTurnStep = BattleManager.TurnStep.Move;

        List<Square> ts = new List<Square>();
        ts.Add(battleManager.GetSelectedPlayerCharacter().GetSquare());

        List<Square> ts2 = new List<Square>();

        for (int i = 0; i < battleManager.GetSelectedPlayerCharacter().movable.movementPoints; i++) {
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

    private void SelectPreviousPlayerCharacter() {
        PlayerCharacter playerCharacter = battleManager.GetSelectedPlayerCharacter();

        do {
            if (battleManager.playerCharacters.IndexOf(playerCharacter) == 0) {
                playerCharacter = battleManager.playerCharacters[battleManager.playerCharacters.Count - 1];
            } else {
                playerCharacter = battleManager.playerCharacters[battleManager.playerCharacters.IndexOf(playerCharacter) - 1];
            }
        } while (playerCharacter.IsDead());

        battleManager.SetSelectedPlayerCharacter(playerCharacter);
    }

    private void SelectNextPlayerCharacter() {
        PlayerCharacter playerCharacter = battleManager.GetSelectedPlayerCharacter();

        do {
            if (battleManager.playerCharacters.IndexOf(playerCharacter) >= battleManager.playerCharacters.Count - 1) {
                playerCharacter = battleManager.playerCharacters[0];
            } else {
                playerCharacter = battleManager.playerCharacters[battleManager.playerCharacters.IndexOf(playerCharacter) + 1];
            }
        } while (playerCharacter.IsDead());

        battleManager.SetSelectedPlayerCharacter(playerCharacter);
    }
}
