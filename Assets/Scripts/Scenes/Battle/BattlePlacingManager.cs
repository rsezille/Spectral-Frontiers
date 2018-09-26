using SF;
using UnityEngine;

public class BattlePlacingManager {
    private BattleManager battleManager;

    public BattlePlacingManager(BattleManager battleManager) {
        this.battleManager = battleManager;
    }

    // Called by BattleManager
    public void Update() {
        if (InputManager.Previous.IsKeyDown) {
            battleManager.currentPartyCharacter.value = battleManager.party.GetPreviousCharacter(battleManager.currentPartyCharacter);
        } else if (InputManager.Next.IsKeyDown) {
            battleManager.currentPartyCharacter.value = battleManager.party.GetNextCharacter(battleManager.currentPartyCharacter);
        } else if (InputManager.Special1.IsKeyDown && battleManager.battleCharacters.player.Count > 0) {
            battleManager.battleState.currentBattleStep = BattleState.BattleStep.Fight;
        }
    }

    // Called by BattleManager
    public void EnterBattleStepPlacing() {
        battleManager.currentPartyCharacter.value = battleManager.party.characters[0];

        foreach (Mission.Enemy enemy in battleManager.missionToLoad.value.enemies) {
            BoardCharacter enemyTemplate = Resources.Load<BoardCharacter>("NewBoardCharacter");

            BoardCharacter enemyBoardCharacter = Object.Instantiate(enemyTemplate, battleManager.board.GetSquare(enemy.posX, enemy.posY).transform.position, Quaternion.identity);
            enemyBoardCharacter.Init(new Character(enemy), Side.Type.Enemy, enemy.direction);
            enemyBoardCharacter.SetSquare(battleManager.board.GetSquare(enemy.posX, enemy.posY));
            battleManager.battleCharacters.enemy.Add(enemyBoardCharacter);
        }

        foreach (Mission.StartingSquare startingSquare in battleManager.missionToLoad.value.startingSquares) {
            battleManager.board.GetSquare(startingSquare.posX, startingSquare.posY).markType = Square.MarkType.Placing;
        }
        
        battleManager.mainCameraPosition.SetPosition(
            battleManager.board.GetSquare(battleManager.missionToLoad.value.startingSquares[0].posX, battleManager.missionToLoad.value.startingSquares[0].posY),
            true
        );

        foreach (Square s in battleManager.board.GetSquares()) {
            s.RefreshMark();
        }
    }
}
