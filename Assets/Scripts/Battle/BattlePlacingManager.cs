using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BattlePlacingManager {
    private BattleManager battleManager; // Shortcut for BattleManager.instance

    public BattlePlacingManager() {
        battleManager = BattleManager.instance;

        BattleManager.OnEnterBattleStepPlacing += OnEnterBattleStepPlacing;
    }

    // Called by BattleManager
    public void Update() {
        if (Input.GetButtonDown(InputBinds.Previous)) {
            PreviousPlacingChar();
        } else if (Input.GetButtonDown(InputBinds.Next)) {
            NextPlacingChar();
        }
    }

    // Event
    public void OnEnterBattleStepPlacing() {
        // Create a temporary list with all available characters from the player
        foreach (Character character in GameManager.instance.player.ownedChars) {
            battleManager.placingAlliedChars.Add(character);
        }
    }

    private void PreviousPlacingChar() {
        if (battleManager.placingAlliedChars.Count > 1) {
            if (battleManager.placingCharIndex == 0) {
                SetCurrentPlacingChar(battleManager.placingAlliedChars.Count - 1);
            } else {
                SetCurrentPlacingChar(battleManager.placingCharIndex - 1);
            }
        }
    }

    private void NextPlacingChar() {
        if (battleManager.placingAlliedChars.Count > 1) {
            if (battleManager.placingCharIndex >= battleManager.placingAlliedChars.Count - 1) {
                SetCurrentPlacingChar(0);
            } else {
                SetCurrentPlacingChar(battleManager.placingCharIndex + 1);
            }
        }
    }

    public void SetCurrentPlacingChar(int index) {
        if (index >= 0 && index <= battleManager.placingAlliedChars.Count - 1) {
            if (battleManager.placingAlliedChars[battleManager.placingCharIndex].boardChar != null) {
                battleManager.placingAlliedChars[battleManager.placingCharIndex].boardChar.outline.gameObject.SetActive(false);
            }

            battleManager.placingCharIndex = index;

            if (battleManager.placingAlliedChars[battleManager.placingCharIndex].boardChar != null) {
                battleManager.placingAlliedChars[battleManager.placingCharIndex].boardChar.outline.gameObject.SetActive(true);
            }
        } else {
            Debug.LogWarning("Trying to set an out of bound index");//TODO: logger class
        }
    }

    public Character GetCurrentPlacingChar() {
        return battleManager.placingAlliedChars[battleManager.placingCharIndex];
    }

    // Used by the placing HUD to display the next character data
    public Character GetPreviousPlacingChar() {
        if (battleManager.placingCharIndex <= 0) {
            return battleManager.placingAlliedChars[battleManager.placingAlliedChars.Count - 1];
        } else {
            return battleManager.placingAlliedChars[battleManager.placingCharIndex - 1];
        }
    }

    // Used by the placing HUD to display the next character data
    public Character GetNextPlacingChar() {
        if (battleManager.placingCharIndex >= battleManager.placingAlliedChars.Count - 1) {
            return battleManager.placingAlliedChars[0];
        } else {
            return battleManager.placingAlliedChars[battleManager.placingCharIndex + 1];
        }
    }

    public void RemoveCurrentMapChar() {
        if (battleManager.currentBattleStep == BattleManager.BattleStep.Placing) {
            if (battleManager.placingAlliedChars[battleManager.placingCharIndex].boardChar != null) {
                Object.Destroy(battleManager.placingAlliedChars[battleManager.placingCharIndex].boardChar.gameObject);

                battleManager.alliedBoardChars.Remove(battleManager.placingAlliedChars[battleManager.placingCharIndex].boardChar);
                battleManager.placingAlliedChars[battleManager.placingCharIndex].boardChar.square.boardChar = null;
                battleManager.placingAlliedChars[battleManager.placingCharIndex].boardChar.character.boardChar = null;

                if (battleManager.alliedBoardChars.Count <= 0) {
                    battleManager.placingHUD.startBattleText.gameObject.SetActive(false);
                }
            } else {
                Debug.LogWarning("Trying to remove a BoardChar which does not exist");
            }
        }
    }

    public void RefreshStartBattleText() {
        if (battleManager.alliedBoardChars.Count <= 0) {
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
            if (square.boardChar == null) {
                if (battleManager.placingAlliedChars[battleManager.placingCharIndex].boardChar != null) {
                    battleManager.placingAlliedChars[battleManager.placingCharIndex].boardChar.SetSquare(square);
                    battleManager.placingAlliedChars[battleManager.placingCharIndex].boardChar.outline.gameObject.SetActive(true);
                } else {
                    BoardChar bc = BattleManager.Instantiate(battleManager.testBoardChar, square.transform.position, Quaternion.identity) as BoardChar;
                    bc.character = battleManager.placingAlliedChars[battleManager.placingCharIndex];
                    bc.side = BoardChar.Side.Ally;
                    bc.SetSquare(square);

                    bc.character.boardChar = bc;
                    battleManager.alliedBoardChars.Add(bc);

                    bc.transform.SetParent(battleManager.transform);
                    bc.outline.gameObject.SetActive(true);

                    RefreshStartBattleText();
                }
            }
        }
    }
}
