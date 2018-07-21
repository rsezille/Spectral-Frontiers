using SF;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleFightManager {
    private BattleManager battleManager; // Shortcut for BattleManager.instance

    private BoardCharacter _selectedPlayerCharacter;
    public BoardCharacter selectedPlayerCharacter {
        get {
            return _selectedPlayerCharacter;
        }

        set {
            if (_selectedPlayerCharacter != null && _selectedPlayerCharacter.outline != null) {
                _selectedPlayerCharacter.outline.enabled = false;
            }

            _selectedPlayerCharacter = value;
            battleManager.fightHUD.UpdateSelectedSquare();

            if (_selectedPlayerCharacter != null) {
                if (_selectedPlayerCharacter.outline != null) {
                    _selectedPlayerCharacter.outline.enabled = true;
                }

                battleManager.battleCamera.SetPosition(_selectedPlayerCharacter, true);
                battleManager.fightHUD.Refresh();
            }
        }
    }

    public BattleFightManager() {
        battleManager = BattleManager.instance;
    }

    // Called by BattleManager
    public void Update() {
        if (InputManager.Previous.IsKeyDown) {
            switch (battleManager.currentTurnStep) {
                case BattleManager.TurnStep.Status:
                    SelectPreviousPlayerBoardCharacter(false);

                    battleManager.statusHUD.Show(
                        battleManager.playerCharacters.IndexOf(battleManager.statusHUD.boardCharacter) <= 0 ?
                        battleManager.playerCharacters[battleManager.playerCharacters.Count - 1] :
                        battleManager.playerCharacters[battleManager.playerCharacters.IndexOf(battleManager.statusHUD.boardCharacter) - 1]
                    );
                    break;
                case BattleManager.TurnStep.None:
                    Previous();
                    break;
            }
        } else if (InputManager.Next.IsKeyDown) {
            switch (battleManager.currentTurnStep) {
                case BattleManager.TurnStep.Status:
                    SelectNextPlayerBoardCharacter(false);

                    battleManager.statusHUD.Show(
                        battleManager.playerCharacters.IndexOf(battleManager.statusHUD.boardCharacter) >= battleManager.playerCharacters.Count - 1 ?
                        battleManager.playerCharacters[0] :
                        battleManager.playerCharacters[battleManager.playerCharacters.IndexOf(battleManager.statusHUD.boardCharacter) + 1]
                    );
                    break;
                case BattleManager.TurnStep.None:
                    Next();
                    break;
            }
        }

        if (battleManager.currentTurnStep == BattleManager.TurnStep.Direction) {
            if (InputManager.Up.IsKeyDown) {
                selectedPlayerCharacter.direction = BoardCharacter.Direction.North;
            } else if (InputManager.Down.IsKeyDown) {
                selectedPlayerCharacter.direction = BoardCharacter.Direction.South;
            } else if (InputManager.Left.IsKeyDown) {
                selectedPlayerCharacter.direction = BoardCharacter.Direction.West;
            } else if (InputManager.Right.IsKeyDown) {
                selectedPlayerCharacter.direction = BoardCharacter.Direction.East;
            } else if (InputManager.Confirm.IsKeyDown) {
                battleManager.fightHUD.SetActiveWithAnimation(true);
                battleManager.EnterTurnStepNone();
            }
        }
    }

    // Called by BattleManager
    public void EnterTurnStepNone(BattleManager.TurnStep previousTurnStep) {
        switch (previousTurnStep) {
            case BattleManager.TurnStep.Move:
            case BattleManager.TurnStep.Attack:
                battleManager.EventOnLeavingMarkStep();
                break;
            case BattleManager.TurnStep.Status:
                battleManager.fightHUD.SetActiveWithAnimation(true);
                break;
        }

        battleManager.fightHUD.Refresh();

        //battleManager.battleCamera.SetPosition(battleManager.GetSelectedPlayerBoardCharacter(), true);
    }

    // Called by BattleManager
    public void EnterTurnStepStatus(BattleManager.TurnStep previousTurnStep) {
        switch (previousTurnStep) {
            case BattleManager.TurnStep.Move:
            case BattleManager.TurnStep.Attack:
                battleManager.EventOnLeavingMarkStep();
                break;
        }

        battleManager.fightHUD.SetActiveWithAnimation(false);

        battleManager.statusHUD.Show(selectedPlayerCharacter);
    }

    // Called by FightHUD
    public void Move() {
        if (battleManager.currentTurnStep == BattleManager.TurnStep.Move) {
            battleManager.EnterTurnStepNone();
        } else {
            battleManager.fightHUD.actionMenu.SetActiveWithAnimation(false);

            if (selectedPlayerCharacter.movable != null && selectedPlayerCharacter.movable.CanMove()) {
                EnterTurnStepMove();
            }
        }
    }

    // Called by FightHUD
    public void Previous() {
        battleManager.EnterTurnStepNone();
        battleManager.fightHUD.actionMenu.SetActiveWithAnimation(false);

        SelectPreviousPlayerBoardCharacter();
    }

    // Called by FightHUD
    public void Next() {
        battleManager.EnterTurnStepNone();
        battleManager.fightHUD.actionMenu.SetActiveWithAnimation(false);

        SelectNextPlayerBoardCharacter();
    }

    // Called by FightHUD
    public void Status() {
        battleManager.EnterTurnStepStatus();
    }

    // Called by FightHUD
    public void Direction() {
        EnterTurnStepDirection();
    }

    // Called by FightHUD
    public void EndTurn() {
        battleManager.fightHUD.actionMenu.SetActiveWithAnimation(false);
        // TODO [ALPHA] FlashMessage
        // TODO [ALPHA] Disable inputs

        if (selectedPlayerCharacter.outline != null) {
            selectedPlayerCharacter.outline.enabled = false;
        }

        EnterTurnStepEnemy();
    }

    // Called by FightHUD
    public void Action() {
        battleManager.EnterTurnStepNone();

        if (selectedPlayerCharacter.actionable.CanDoAction()) {
            battleManager.fightHUD.actionMenu.Toggle();
        }
    }

    // Called by ActionMenu
    public void Attack() {
        if (battleManager.currentTurnStep == BattleManager.TurnStep.Attack) {
            battleManager.EnterTurnStepNone();
        } else {
            battleManager.fightHUD.actionMenu.SetActiveWithAnimation(false);

            if (selectedPlayerCharacter.actionable != null && selectedPlayerCharacter.actionable.CanDoAction()) {
                EnterTurnStepAttack();
            }
        }
    }

    public void EnterBattleStepFight() {
        if (battleManager.playerCharacters.Count > 0) {
            battleManager.EventOnLeavingMarkStep();

            // Disable outlines from the PlacingStep
            if (battleManager.placing.GetCurrentPlacingChar().boardCharacter != null) {
                battleManager.placing.GetCurrentPlacingChar().boardCharacter.outline.enabled = false;
            }

            battleManager.currentBattleStep = BattleManager.BattleStep.Fight;
            battleManager.placingHUD.SetActiveWithAnimation(false);
            battleManager.fightHUD.SetActiveWithAnimation(true);
            NewPlayerTurn();
        }
    }

    private void NewPlayerTurn() {
        if (battleManager.CheckEndBattle()) {
            return;
        }

        battleManager.turn++;

        foreach (BoardCharacter bc in battleManager.playerCharacters) {
            bc.NewTurn();
        }

        battleManager.EnterTurnStepNone();

        BoardCharacter aliveCharacter = battleManager.playerCharacters[0];

        foreach (BoardCharacter character in battleManager.playerCharacters) {
            if (!character.IsDead()) {
                aliveCharacter = character;
                break;
            }
        }

        selectedPlayerCharacter = aliveCharacter;
    }

    private void EnterTurnStepEnemy() {
        switch (battleManager.currentTurnStep) {
            case BattleManager.TurnStep.Move:
            case BattleManager.TurnStep.Attack:
                battleManager.EventOnLeavingMarkStep();
                break;
        }

        battleManager.currentTurnStep = BattleManager.TurnStep.Enemy;

        foreach (BoardCharacter enemy in battleManager.enemyCharacters) {
            enemy.NewTurn();
        }

        battleManager.StartCoroutine(StartAI());
    }

    /**
     * Process each enemy AI then start a new player turn
     */
    private IEnumerator StartAI() {
        battleManager.fightHUD.SetActiveWithAnimation(false);

        foreach (BoardCharacter enemy in battleManager.enemyCharacters) {
            if (enemy.IsDead()) continue;

            if (enemy.AI != null) {
                yield return enemy.AI.Process();
            }

            yield return new WaitForSeconds(0.5f);
        }

        battleManager.fightHUD.SetActiveWithAnimation(true);
        NewPlayerTurn();

        yield return null;
    }

    private void MarkSquares(int distance, Square.MarkType markType, bool ignoreBlocking = false) {
        battleManager.EventOnLeavingMarkStep();

        List<Square> squaresHit = battleManager.board.PropagateLinear(selectedPlayerCharacter.GetSquare(), distance, selectedPlayerCharacter.side.value, ignoreBlocking);

        foreach (Square squareHit in squaresHit) {
            if (squareHit.IsNotBlocking() || ignoreBlocking) {
                squareHit.markType = markType;
            }
        }
    }

    // Mark all squares where the character can move
    private void EnterTurnStepMove() {
        battleManager.currentTurnStep = BattleManager.TurnStep.Move;

        battleManager.fightHUD.actionMenu.SetActiveWithAnimation(false);

        MarkSquares(selectedPlayerCharacter.movable.movementPoints, Square.MarkType.Movement);
    }

    // Mark all squares the character can attack
    private void EnterTurnStepAttack() {
        battleManager.currentTurnStep = BattleManager.TurnStep.Attack;

        MarkSquares(1, Square.MarkType.Attack, true); // TODO [ALPHA] weapon range
    }

    public void EnterTurnStepDirection() {
        battleManager.currentTurnStep = BattleManager.TurnStep.Direction;
        battleManager.fightHUD.SetActiveWithAnimation(false);
    }

    private void SelectPreviousPlayerBoardCharacter(bool checkForDead = true) {
        BoardCharacter boardCharacter = selectedPlayerCharacter;

        do {
            if (battleManager.playerCharacters.IndexOf(boardCharacter) == 0) {
                boardCharacter = battleManager.playerCharacters[battleManager.playerCharacters.Count - 1];
            } else {
                boardCharacter = battleManager.playerCharacters[battleManager.playerCharacters.IndexOf(boardCharacter) - 1];
            }
        } while (boardCharacter.IsDead() && checkForDead);

        selectedPlayerCharacter = boardCharacter;
    }

    private void SelectNextPlayerBoardCharacter(bool checkForDead = true) {
        BoardCharacter boardCharacter = selectedPlayerCharacter;

        do {
            if (battleManager.playerCharacters.IndexOf(boardCharacter) >= battleManager.playerCharacters.Count - 1) {
                boardCharacter = battleManager.playerCharacters[0];
            } else {
                boardCharacter = battleManager.playerCharacters[battleManager.playerCharacters.IndexOf(boardCharacter) + 1];
            }
        } while (boardCharacter.IsDead() && checkForDead);

        selectedPlayerCharacter = boardCharacter;
    }
}
