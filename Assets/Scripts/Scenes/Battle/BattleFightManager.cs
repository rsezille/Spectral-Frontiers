using SF;
using System;
using UnityEngine;

public class BattleFightManager {
    private BattleManager battleManager;

    public GameObject arrows;

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
        Action<BoardCharacter> InitializeWaitingTime = (BoardCharacter c) => {
            c.tick = 0;
        };

        battleManager.battleCharacters.player.ForEach(InitializeWaitingTime);
        battleManager.battleCharacters.enemy.ForEach(InitializeWaitingTime);

        NewTurn();
    }

    public void NewTurn() {
        battleManager.board.RemoveAllMarks();

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
