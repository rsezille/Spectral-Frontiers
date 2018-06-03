using System.Collections;
using UnityEngine;

/**
 * Can only be used by a BoardCharacter
 */
 [RequireComponent(typeof(BoardCharacter))]
public class AI : MonoBehaviour {
    private BattleManager battleManager;
    private BoardCharacter boardCharacter;

    private void Awake() {
        battleManager = BattleManager.instance;
        boardCharacter = GetComponent<BoardCharacter>();
    }

    public IEnumerator Process() {
        CustomAI customAI = GetComponent<CustomAI>();

        if (customAI != null) {
            yield return customAI.Process();
        } else {
            Move();

            while (boardCharacter.isMoving) {
                yield return null;
            }

            yield return new WaitForSeconds(1);
        }

        yield return null;
    }

    /**
     * Determine a path to get closer to a player's BoardCharacter
     */
    private void Move() {
        Path bestPath = null;
        int lowestCharHP = 0;

        foreach (BoardCharacter playerCharacter in battleManager.playerCharacters) {
            if (!playerCharacter.IsDead()) {
                Path p = battleManager.board.pathFinder.FindPath(
                    boardCharacter.GetSquare().x,
                    boardCharacter.GetSquare().y,
                    playerCharacter.GetSquare().x,
                    playerCharacter.GetSquare().y
                );
                if (p != null) {
                    if (bestPath == null) {
                        bestPath = p;
                        lowestCharHP = playerCharacter.character.GetCurrentHP();
                    } else {
                        if (p.GetLength() > boardCharacter.character.movementPoints + 1) {
                            if (p.GetLength() < bestPath.GetLength()) {
                                bestPath = p;
                                lowestCharHP = playerCharacter.character.GetCurrentHP();
                            }
                        } else {
                            if (bestPath.GetLength() > boardCharacter.character.movementPoints + 1) {
                                bestPath = p;
                                lowestCharHP = playerCharacter.character.GetCurrentHP();
                            } else {
                                if (playerCharacter.character.GetCurrentHP() < lowestCharHP) {
                                    bestPath = p;
                                    lowestCharHP = playerCharacter.character.GetCurrentHP();
                                }
                            }
                        }
                    }
                }
            }
        }

        if (bestPath != null) {
            boardCharacter.Move(bestPath, true);
        }
    }
}
