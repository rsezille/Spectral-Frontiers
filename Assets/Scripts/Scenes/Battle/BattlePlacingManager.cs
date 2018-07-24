﻿using SF;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattlePlacingManager {
    private BattleManager battleManager; // Shortcut for BattleManager.instance

    private int placingCharIndex;

    public BattlePlacingManager() {
        battleManager = BattleManager.instance;

        placingCharIndex = 0;
    }

    // Called by BattleManager
    public void Update() {
        if (InputManager.Previous.IsKeyDown) {
            PreviousPlacingChar();

            if (battleManager.currentTurnStep == BattleManager.TurnStep.Status) {
                battleManager.statusHUD.Show(GetCurrentPlacingChar());
            }
        } else if (InputManager.Next.IsKeyDown) {
            NextPlacingChar();

            if (battleManager.currentTurnStep == BattleManager.TurnStep.Status) {
                battleManager.statusHUD.Show(GetCurrentPlacingChar());
            }
        } else if (InputManager.Special1.IsKeyDown && battleManager.playerCharacters.Count > 0) {
            battleManager.fight.EnterBattleStepFight();
        }
    }

    // Called by BattleManager
    public void EnterTurnStepNone(BattleManager.TurnStep previousTurnStep) {
        if (previousTurnStep == BattleManager.TurnStep.Status) {
            battleManager.placingHUD.SetActiveWithAnimation(true);
        }
    }

    // Called by BattleManager
    public void EnterTurnStepStatus(BattleManager.TurnStep previousTurnStep) {
        battleManager.placingHUD.SetActiveWithAnimation(false);

        battleManager.statusHUD.Show(GetCurrentPlacingChar());
    }

    public void EnterBattleStepPlacing() {
        foreach (RawMission.RawStartingSquare startingSquare in battleManager.mission.startingSquares) {
            battleManager.board.GetSquare(startingSquare.posX, startingSquare.posY).markType = Square.MarkType.Placing;
        }

        battleManager.currentBattleStep = BattleManager.BattleStep.Placing;
        battleManager.placingHUD.SetActiveWithAnimation(true, HUD.Speed.Slow);

        battleManager.battleCamera.SetPosition(battleManager.mission.startingSquares[0].posX, battleManager.mission.startingSquares[0].posY, true);

        battleManager.EventOnEnterPlacing();
    }

    private void PreviousPlacingChar() {
        if (placingCharIndex == 0) {
            SetCurrentPlacingChar(GameManager.instance.player.characters.Count - 1);
        } else {
            SetCurrentPlacingChar(placingCharIndex - 1);
        }
    }

    private void NextPlacingChar() {
        if (placingCharIndex >= GameManager.instance.player.characters.Count - 1) {
            SetCurrentPlacingChar(0);
        } else {
            SetCurrentPlacingChar(placingCharIndex + 1);
        }
    }

    public void SetCurrentPlacingChar(Character character) {
        if (!GameManager.instance.player.characters.Contains(character)) {
            return;
        }

        SetCurrentPlacingChar(GameManager.instance.player.characters.IndexOf(character));
    }

    public void SetCurrentPlacingChar(int index) {
        if (index < 0 || index > GameManager.instance.player.characters.Count - 1) {
            return;
        }

        if (GetCurrentPlacingChar().boardCharacter != null && GetCurrentPlacingChar().boardCharacter.outline != null) {
            GetCurrentPlacingChar().boardCharacter.outline.enabled = false;
        }

        placingCharIndex = index;

        if (GetCurrentPlacingChar().boardCharacter != null) {
            if (GetCurrentPlacingChar().boardCharacter.outline != null) {
                GetCurrentPlacingChar().boardCharacter.outline.enabled = true;
            }

            battleManager.battleCamera.SetPosition(GetCurrentPlacingChar().boardCharacter, true);
        }
    }

    public Character GetCurrentPlacingChar() {
        return GameManager.instance.player.characters[placingCharIndex];
    }

    // Used by the placing HUD to display the next character data
    public Character GetPreviousPlacingChar() {
        if (placingCharIndex <= 0) {
            return GameManager.instance.player.characters[GameManager.instance.player.characters.Count - 1];
        } else {
            return GameManager.instance.player.characters[placingCharIndex - 1];
        }
    }

    // Used by the placing HUD to display the next character data
    public Character GetNextPlacingChar() {
        if (placingCharIndex >= GameManager.instance.player.characters.Count - 1) {
            return GameManager.instance.player.characters[0];
        } else {
            return GameManager.instance.player.characters[placingCharIndex + 1];
        }
    }

    public void RemoveCurrentMapChar() {
        if (battleManager.currentBattleStep != BattleManager.BattleStep.Placing) {
            return;
        }

        if (GetCurrentPlacingChar().boardCharacter == null) return;
            
        Object.Destroy(GetCurrentPlacingChar().boardCharacter.gameObject);
            
        if (battleManager.playerCharacters.Count <= 0) {
            battleManager.placingHUD.startBattleText.gameObject.SetActive(false);
        }
    }

    public void RefreshStartBattleText() {
        if (battleManager.playerCharacters.Count <= 0) {
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
        if (battleManager.currentBattleStep == BattleManager.BattleStep.Placing) {
            if (square.IsNotBlocking()) {
                if (GetCurrentPlacingChar().boardCharacter != null) {
                    GetCurrentPlacingChar().boardCharacter.SetSquare(square);
                    GetCurrentPlacingChar().boardCharacter.outline.enabled = true;
                    GetCurrentPlacingChar().boardCharacter.direction = square.startingDirection;
                } else {
                    if (battleManager.playerCharacters.Count >= battleManager.mission.maxPlayerCharacters) {
                        return;
                    }

                    PlayerCharacter pc = Object.Instantiate(battleManager.testPlayerCharacter, square.transform.position, Quaternion.identity) as PlayerCharacter;
                    pc.boardCharacter.character = GetCurrentPlacingChar();
                    pc.GetComponent<Side>().value = Side.Type.Player;
                    pc.boardCharacter.SetSquare(square);
                    pc.boardCharacter.direction = square.startingDirection;

                    pc.boardCharacter.character.boardCharacter = pc.boardCharacter;
                    battleManager.playerCharacters.Add(pc.boardCharacter);

                    if (pc.boardCharacter.outline != null) {
                        pc.boardCharacter.outline.enabled = true;
                    }

                    RefreshStartBattleText();
                }

                battleManager.EventOnSemiTransparentReset();
            }
        }
    }
}