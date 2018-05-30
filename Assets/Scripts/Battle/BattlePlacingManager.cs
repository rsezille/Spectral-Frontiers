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
        if (Input.GetButtonDown(InputBinds.Previous)) {
            PreviousPlacingChar();
        } else if (Input.GetButtonDown(InputBinds.Next)) {
            NextPlacingChar();
        } else if (Input.GetButtonDown(InputBinds.SpecialKey1)) {
            battleManager.fight.EnterBattleStepFight();
        }
    }

    // Called by BattleManager
    public void EnterTurnStepNone(BattleManager.TurnStep previousTurnStep) {}

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

            if (battleManager.currentTurnStep == BattleManager.TurnStep.Status) {
                battleManager.statusHUD.Show(battleManager.playerPlacingChars[battleManager.placingCharIndex]);
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

            if (battleManager.currentTurnStep == BattleManager.TurnStep.Status) {
                battleManager.statusHUD.Show(battleManager.playerPlacingChars[battleManager.placingCharIndex]);
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
            if (battleManager.playerPlacingChars[battleManager.placingCharIndex].boardChar != null) {
                battleManager.playerPlacingChars[battleManager.placingCharIndex].boardChar.outline.enabled = false;
            }

            battleManager.placingCharIndex = index;

            if (battleManager.playerPlacingChars[battleManager.placingCharIndex].boardChar != null) {
                battleManager.playerPlacingChars[battleManager.placingCharIndex].boardChar.outline.enabled = true;
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
            if (battleManager.playerPlacingChars[battleManager.placingCharIndex].boardChar != null) {
                Object.Destroy(battleManager.playerPlacingChars[battleManager.placingCharIndex].boardChar.gameObject);

                battleManager.playerBoardChars.Remove(battleManager.playerPlacingChars[battleManager.placingCharIndex].boardChar);
                battleManager.playerPlacingChars[battleManager.placingCharIndex].boardChar.SetSquare(null);
                battleManager.playerPlacingChars[battleManager.placingCharIndex].boardChar.character.boardChar = null;

                if (battleManager.playerBoardChars.Count <= 0) {
                    battleManager.placingHUD.startBattleText.gameObject.SetActive(false);
                }
            } else {
                Debug.LogWarning("Trying to remove a BoardChar which does not exist");
            }
        }
    }

    public void RefreshStartBattleText() {
        if (battleManager.playerBoardChars.Count <= 0) {
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
                if (battleManager.playerPlacingChars[battleManager.placingCharIndex].boardChar != null) {
                    battleManager.playerPlacingChars[battleManager.placingCharIndex].boardChar.SetSquare(square);
                    battleManager.playerPlacingChars[battleManager.placingCharIndex].boardChar.outline.enabled = true;
                } else {
                    BoardChar bc = BattleManager.Instantiate(battleManager.testBoardChar, square.transform.position, Quaternion.identity) as BoardChar;
                    bc.character = battleManager.playerPlacingChars[battleManager.placingCharIndex];
                    bc.GetComponent<Side>().value = Side.Type.Player;
                    bc.SetSquare(square);

                    bc.character.boardChar = bc;
                    battleManager.playerBoardChars.Add(bc);

                    bc.transform.SetParent(battleManager.transform);
                    bc.outline.enabled = true;

                    RefreshStartBattleText();
                }
            }
        }
    }
}
