using SF;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleFightManager {
    private BattleManager battleManager;

    public GameObject arrows;

    /*private BoardCharacter _selectedPlayerCharacter;
    public BoardCharacter selectedPlayerCharacter {
        get {
            return _selectedPlayerCharacter;
        }

        set {
            if (_selectedPlayerCharacter != null && _selectedPlayerCharacter.glow != null) {
                _selectedPlayerCharacter.glow.Disable(); // Into new character turn
            }

            _selectedPlayerCharacter = value;
            battleManager.fightHUD.UpdateSelectedSquare(); // Into new character turn

            if (_selectedPlayerCharacter != null) {
                if (_selectedPlayerCharacter.glow != null) {
                    _selectedPlayerCharacter.glow.Enable(); // Into new character turn
                }

                battleManager.battleCamera.SetPosition(_selectedPlayerCharacter, true); // Into new character turn
                battleManager.fightHUD.Refresh(); // Into new character turn
            }
        }
    }*/

    public BattleFightManager(BattleManager battleManager) {
        this.battleManager = battleManager;
    }

    // Called by BattleManager
    public void Update() {
        if (InputManager.Previous.IsKeyDown) {
            switch (battleManager.battleState.currentTurnStep) {
                case BattleState.TurnStep.Status:
                    battleManager.currentPartyCharacter.value = battleManager.party.GetPreviousCharacter(battleManager.currentPartyCharacter);

                    if (battleManager.currentPartyCharacter.value.boardCharacter != null) {
                        battleManager.currentFightBoardCharacter.value = battleManager.currentPartyCharacter.value.boardCharacter;
                    }
                    break;
            }
        } else if (InputManager.Next.IsKeyDown) {
            switch (battleManager.battleState.currentTurnStep) {
                case BattleState.TurnStep.Status:
                    battleManager.currentPartyCharacter.value = battleManager.party.GetNextCharacter(battleManager.currentPartyCharacter);

                    if (battleManager.currentPartyCharacter.value.boardCharacter != null) {
                        battleManager.currentFightBoardCharacter.value = battleManager.currentPartyCharacter.value.boardCharacter;
                    }
                    break;
            }
        }

        if (battleManager.battleState.currentTurnStep == BattleState.TurnStep.Direction) {
            if (InputManager.Up.IsKeyDown) {
                battleManager.currentFightBoardCharacter.value.direction = BoardCharacter.Direction.North;
            } else if (InputManager.Down.IsKeyDown) {
                battleManager.currentFightBoardCharacter.value.direction = BoardCharacter.Direction.South;
            } else if (InputManager.Left.IsKeyDown) {
                battleManager.currentFightBoardCharacter.value.direction = BoardCharacter.Direction.West;
            } else if (InputManager.Right.IsKeyDown) {
                battleManager.currentFightBoardCharacter.value.direction = BoardCharacter.Direction.East;
            } else if (InputManager.Confirm.IsKeyDown) {
                battleManager.battleState.currentTurnStep = BattleState.TurnStep.None;
            }
        }
    }

    // Called by BattleManager
    public void EnterBattleStepFight() {
        battleManager.fightHUD.SetActiveWithAnimation(true);
        //NewPlayerTurn();

        Action<BoardCharacter> InitializeWaitingTime = (BoardCharacter c) => {
            c.tick = 0;
        };

        battleManager.battleCharacters.player.ForEach(InitializeWaitingTime);
        battleManager.battleCharacters.enemy.ForEach(InitializeWaitingTime);

        NewTurn();
    }

    public void NewTurn() {
        battleManager.newCharacterTurn.Raise();

        while (!CharacterReady()) {
            battleManager.battleCharacters.player.ForEach(c => c.tick += 1);
            battleManager.battleCharacters.enemy.ForEach(c => c.tick += 1);
        }

        BoardCharacter characterToPlay = GetCharacterToPlay();
        characterToPlay.tick = 0;
        
        characterToPlay.NewTurn();
    }

    private bool CharacterReady() {
        foreach (BoardCharacter boardCharacter in battleManager.battleCharacters.player) {
            if (boardCharacter.tick >= battleManager.turnSpeed - boardCharacter.character.spd) {
                return true;
            }
        }

        foreach (BoardCharacter boardCharacter in battleManager.battleCharacters.enemy) {
            if (boardCharacter.tick >= battleManager.turnSpeed - boardCharacter.character.spd) {
                return true;
            }
        }

        return false;
    }

    private BoardCharacter GetCharacterToPlay() {
        BoardCharacter characterToPlay = null;

        foreach (BoardCharacter boardCharacter in battleManager.battleCharacters.player) {
            if (boardCharacter.tick >= battleManager.turnSpeed - boardCharacter.character.spd) {
                if (characterToPlay == null) {
                    characterToPlay = boardCharacter;
                } else if (battleManager.board.SquarePositionToIndex(boardCharacter.GetSquare()) < battleManager.board.SquarePositionToIndex(characterToPlay.GetSquare())) {
                    characterToPlay = boardCharacter;
                }
            }
        }

        foreach (BoardCharacter boardCharacter in battleManager.battleCharacters.enemy) {
            if (boardCharacter.tick >= battleManager.turnSpeed - boardCharacter.character.spd) {
                if (characterToPlay == null) {
                    characterToPlay = boardCharacter;
                } else if (battleManager.board.SquarePositionToIndex(boardCharacter.GetSquare()) < battleManager.board.SquarePositionToIndex(characterToPlay.GetSquare())) {
                    characterToPlay = boardCharacter;
                }
            }
        }

        return characterToPlay;
    }

    // Called by BattleManager
    public void LeaveBattleStepFight() {
        if (battleManager.currentFightBoardCharacter.value.glow != null) {
            battleManager.currentFightBoardCharacter.value.glow.Disable();
        }

        battleManager.board.RemoveAllMarks();
        battleManager.statusHUD.Hide();
        battleManager.fightHUD.SetActiveWithAnimation(false);
    }

    private void NewPlayerTurn() {
        /*if (battleManager.CheckEndBattle()) {
            return;
        }*/

        //battleManager.sunLight.NewTurn();

        foreach (BoardCharacter bc in battleManager.battleCharacters.player) {
            bc.NewTurn();
        }

        battleManager.battleState.currentTurnStep = BattleState.TurnStep.None;

        battleManager.currentFightBoardCharacter.value = battleManager.battleCharacters.player[0];
    }

    private void EnterTurnStepEnemy() {
        switch (battleManager.battleState.currentTurnStep) {
            case BattleState.TurnStep.Move:
            case BattleState.TurnStep.Attack:
                battleManager.board.RemoveAllMarks();
                break;
        }

        battleManager.battleState.currentTurnStep = BattleState.TurnStep.Enemy;

        foreach (BoardCharacter enemy in battleManager.battleCharacters.enemy) {
            enemy.NewTurn();
        }

        battleManager.StartCoroutine(StartAI());
    }

    /**
     * Process each enemy AI then start a new player turn
     */
    private IEnumerator StartAI() {
        battleManager.fightHUD.SetActiveWithAnimation(false);

        foreach (BoardCharacter enemy in battleManager.battleCharacters.enemy) {
            if (enemy.AI != null) {
                yield return enemy.AI.Process();
            }

            yield return new WaitForSeconds(0.5f);
        }

        battleManager.fightHUD.SetActiveWithAnimation(true);
        NewPlayerTurn();

        yield return null;
    }

    // Mark all squares where the character can move
    public void EnterTurnStepMove() {
        battleManager.board.MarkSquares(
            battleManager.currentFightBoardCharacter.value.GetSquare(),
            battleManager.currentFightBoardCharacter.value.movementPoints,
            Square.MarkType.Movement,
            battleManager.currentFightBoardCharacter.value.side.value
        );
    }
}
