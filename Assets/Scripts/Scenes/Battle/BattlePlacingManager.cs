using SF;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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
        
        battleManager.placingHUD.SetActiveWithAnimation(true, HUD.Speed.Slow);
        battleManager.battleCamera.SetPosition(battleManager.missionToLoad.value.startingSquares[0].posX, battleManager.missionToLoad.value.startingSquares[0].posY, true);

        foreach (Square s in battleManager.board.GetSquares()) {
            s.RefreshMark();
        }
    }

    // Called by BattleManager
    public void LeaveBattleStepPlacing() {
        battleManager.board.RemoveAllMarks();

        // Disable outlines from the placing step
        if (battleManager.currentPartyCharacter.value.boardCharacter != null) {
            battleManager.currentPartyCharacter.value.boardCharacter.glow.Disable();
        }

        battleManager.placingHUD.SetActiveWithAnimation(false);
    }

    // Called by BattleManager
    public void EnterTurnStepNone(BattleState.TurnStep previousTurnStep) {
        if (previousTurnStep == BattleState.TurnStep.Status) {
            battleManager.placingHUD.SetActiveWithAnimation(true);
        }
    }

    // Called by BattleManager
    public void EnterTurnStepStatus(BattleState.TurnStep previousTurnStep) {
        battleManager.placingHUD.SetActiveWithAnimation(false);

        battleManager.statusHUD.Show(battleManager.currentPartyCharacter.value);
    }
    
    public void SetCurrentPlacingChar(Character character) {
        if (!battleManager.party.characters.Contains(character)) {
            return;
        }

        if (battleManager.currentPartyCharacter.value.boardCharacter != null && battleManager.currentPartyCharacter.value.boardCharacter.glow != null) {
            battleManager.currentPartyCharacter.value.boardCharacter.glow.Disable();
        }

        battleManager.currentPartyCharacter.value = character;

        if (battleManager.currentPartyCharacter.value.boardCharacter != null) {
            if (battleManager.currentPartyCharacter.value.boardCharacter.glow != null) {
                battleManager.currentPartyCharacter.value.boardCharacter.glow.Enable();
            }

            battleManager.battleCamera.SetPosition(battleManager.currentPartyCharacter.value.boardCharacter, true);
        }
    }

    public void RemoveCurrentMapChar() {
        if (battleManager.battleState.currentBattleStep != BattleState.BattleStep.Placing) {
            return;
        }

        if (battleManager.currentPartyCharacter.value.boardCharacter == null) return;

        battleManager.currentPartyCharacter.value.boardCharacter.Remove();
        
        if (battleManager.battleCharacters.player.Count <= 0) {
            battleManager.placingHUD.startBattleText.gameObject.SetActive(false);
        }
    }

    public void RefreshStartBattleText() {
        if (battleManager.battleCharacters.player.Count <= 0) {
            battleManager.placingHUD.startBattleText.gameObject.SetActive(false);
        } else {
            battleManager.placingHUD.startBattleText.gameObject.SetActive(true);
            battleManager.StartCoroutine(ShowStartBattleTextFade());
        }
    }

    public IEnumerator ShowStartBattleTextFade() {
        Text startBattleText = battleManager.placingHUD.startBattleText;

        Color start = new Color(startBattleText.color.r, startBattleText.color.g, startBattleText.color.b, 0f);
        Color target = new Color(startBattleText.color.r, startBattleText.color.g, startBattleText.color.b, 1f);

        float minFade = 0.3f;
        float maxFade = 1f;

        float smoothness = 0.01f;
        float duration = 0.7f;
        float progress = 0f;
        float increment = smoothness / duration;

        while (startBattleText.isActiveAndEnabled) {
            startBattleText.color = Color.Lerp(start, target, progress);

            if (progress > maxFade || (progress < minFade && increment < 0f)) {
                increment = -increment;
            }

            progress += increment;

            yield return new WaitForSeconds(smoothness);
        }
    }

    /**
     * Place the current character on the specified tile
     */
    public void PlaceMapChar(Square square) {
        if (battleManager.battleState.currentBattleStep == BattleState.BattleStep.Placing) {
            if (square.IsNotBlocking()) {
                if (battleManager.currentPartyCharacter.value.boardCharacter != null) {
                    battleManager.currentPartyCharacter.value.boardCharacter.SetSquare(square);
                    battleManager.currentPartyCharacter.value.boardCharacter.glow.Enable();
                    battleManager.currentPartyCharacter.value.boardCharacter.direction = square.startingDirection;
                } else {
                    if (battleManager.battleCharacters.player.Count >= battleManager.missionToLoad.value.maxPlayerCharacters) {
                        return;
                    }

                    BoardCharacter playerTemplate = Resources.Load<BoardCharacter>("NewBoardCharacter");

                    BoardCharacter playerBoardCharacter = Object.Instantiate(playerTemplate, square.transform.position, Quaternion.identity);
                    playerBoardCharacter.Init(battleManager.currentPartyCharacter.value, Side.Type.Player, square.startingDirection);
                    playerBoardCharacter.SetSquare(square);
                    playerBoardCharacter.character.boardCharacter = playerBoardCharacter;
                    battleManager.battleCharacters.player.Add(playerBoardCharacter);

                    if (playerBoardCharacter.glow != null) {
                        playerBoardCharacter.glow.Enable();
                    }

                    RefreshStartBattleText();
                }

                battleManager.checkSemiTransparent.Raise();
            }
        }
    }
}
