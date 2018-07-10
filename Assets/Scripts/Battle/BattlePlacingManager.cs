using SF;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BattlePlacingManager {
    private BattleManager battleManager; // Shortcut for BattleManager.instance

    public BattlePlacingManager() {
        battleManager = BattleManager.instance;
    }

    // Called by BattleManager
    public void Update() {
        if (InputManager.Previous.IsKeyDown) {
            PreviousPlacingChar();

            if (battleManager.currentTurnStep == BattleManager.TurnStep.Status) {
                battleManager.statusHUD.Show(battleManager.playerPlacingChars[battleManager.placingCharIndex]);
            }
        } else if (InputManager.Next.IsKeyDown) {
            NextPlacingChar();

            if (battleManager.currentTurnStep == BattleManager.TurnStep.Status) {
                battleManager.statusHUD.Show(battleManager.playerPlacingChars[battleManager.placingCharIndex]);
            }
        } else if (InputManager.Special1.IsKeyDown) {
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
        // Create a temporary list with all available characters from the player
        foreach (Character character in GameManager.instance.player.characters) {
            battleManager.playerPlacingChars.Add(character);
        }

        battleManager.currentBattleStep = BattleManager.BattleStep.Placing;
        battleManager.placingHUD.SetActiveWithAnimation(true);

        battleManager.EventOnEnterPlacing();
    }

    private void PreviousPlacingChar() {
        if (battleManager.playerPlacingChars.Count > 1) {
            if (battleManager.placingCharIndex == 0) {
                SetCurrentPlacingChar(battleManager.playerPlacingChars.Count - 1);
            } else {
                SetCurrentPlacingChar(battleManager.placingCharIndex - 1);
            }
        }
    }

    private void NextPlacingChar() {
        if (battleManager.playerPlacingChars.Count > 1) {
            if (battleManager.placingCharIndex >= battleManager.playerPlacingChars.Count - 1) {
                SetCurrentPlacingChar(0);
            } else {
                SetCurrentPlacingChar(battleManager.placingCharIndex + 1);
            }
        }
    }

    public void SetCurrentPlacingChar(Character character) {
        if (battleManager.playerPlacingChars.Contains(character)) {
            SetCurrentPlacingChar(battleManager.playerPlacingChars.IndexOf(character));
        } else {
            Debug.LogWarning("Trying to set an inexisting character");
        }
    }

    public void SetCurrentPlacingChar(int index) {
        if (index >= 0 && index <= battleManager.playerPlacingChars.Count - 1) {
            if (battleManager.playerPlacingChars[battleManager.placingCharIndex].boardCharacter != null
                    && battleManager.playerPlacingChars[battleManager.placingCharIndex].boardCharacter.outline != null) {
                battleManager.playerPlacingChars[battleManager.placingCharIndex].boardCharacter.outline.enabled = false;
            }

            battleManager.placingCharIndex = index;

            if (battleManager.playerPlacingChars[battleManager.placingCharIndex].boardCharacter != null) {
                if (battleManager.playerPlacingChars[battleManager.placingCharIndex].boardCharacter.outline != null) {
                    battleManager.playerPlacingChars[battleManager.placingCharIndex].boardCharacter.outline.enabled = true;
                }

                battleManager.battleCamera.SetPosition(battleManager.playerPlacingChars[battleManager.placingCharIndex].boardCharacter, true);
            }
        } else {
            Debug.LogWarning("Trying to set an out of bound index");
        }
    }

    public Character GetCurrentPlacingChar() {
        return battleManager.playerPlacingChars[battleManager.placingCharIndex];
    }

    // Used by the placing HUD to display the next character data
    public Character GetPreviousPlacingChar() {
        if (battleManager.placingCharIndex <= 0) {
            return battleManager.playerPlacingChars[battleManager.playerPlacingChars.Count - 1];
        } else {
            return battleManager.playerPlacingChars[battleManager.placingCharIndex - 1];
        }
    }

    // Used by the placing HUD to display the next character data
    public Character GetNextPlacingChar() {
        if (battleManager.placingCharIndex >= battleManager.playerPlacingChars.Count - 1) {
            return battleManager.playerPlacingChars[0];
        } else {
            return battleManager.playerPlacingChars[battleManager.placingCharIndex + 1];
        }
    }

    public void RemoveCurrentMapChar() {
        if (battleManager.currentBattleStep == BattleManager.BattleStep.Placing) {
            if (battleManager.playerPlacingChars[battleManager.placingCharIndex].boardCharacter != null) {
                Object.Destroy(battleManager.playerPlacingChars[battleManager.placingCharIndex].boardCharacter.gameObject);

                battleManager.playerCharacters.Remove(battleManager.playerPlacingChars[battleManager.placingCharIndex].boardCharacter);
                battleManager.playerPlacingChars[battleManager.placingCharIndex].boardCharacter.SetSquare(null);
                battleManager.playerPlacingChars[battleManager.placingCharIndex].boardCharacter.character.boardCharacter = null;

                if (battleManager.playerCharacters.Count <= 0) {
                    battleManager.placingHUD.startBattleText.gameObject.SetActive(false);
                }
            } else {
                Debug.LogWarning("Trying to remove a PlayerCharacter which does not exist");
            }
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
                if (battleManager.playerPlacingChars[battleManager.placingCharIndex].boardCharacter != null) {
                    battleManager.playerPlacingChars[battleManager.placingCharIndex].boardCharacter.SetSquare(square);
                    battleManager.playerPlacingChars[battleManager.placingCharIndex].boardCharacter.outline.enabled = true;
                    battleManager.playerPlacingChars[battleManager.placingCharIndex].boardCharacter.direction = square.startingDirection;
                } else {
                    if (battleManager.playerCharacters.Count >= battleManager.mission.max_player_characters) {
                        return;
                    }

                    PlayerCharacter pc = Object.Instantiate(battleManager.testPlayerCharacter, square.transform.position, Quaternion.identity) as PlayerCharacter;
                    pc.boardCharacter.character = battleManager.playerPlacingChars[battleManager.placingCharIndex];
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

                //battleManager.battleCamera.SetPosition(battleManager.playerPlacingChars[battleManager.placingCharIndex].boardCharacter, true);
            }
        }
    }
}
