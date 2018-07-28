using System.Collections;
using UnityEngine;

/**
 * Can only be used by a BoardCharacter
 */
 [RequireComponent(typeof(BoardCharacter))]
public class AI : MonoBehaviour {
    public enum Preset {
        Nothing, // Do nothing
        Aggressive, // Always try to be in range and attack
        Passive // Only attack when a player character is in range + movements
    }

    private BattleManager battleManager;
    private BoardCharacter boardCharacter;

    public Preset preset = Preset.Aggressive;

    private void Awake() {
        battleManager = BattleManager.instance;
        boardCharacter = GetComponent<BoardCharacter>();
    }

    /**
     * Use a custom AI if it exists, otherwise use the standard one
     * TODO [ALPHA] Create presets such as normal, aggressive, and so on
     */
    public IEnumerator Process() {
        CustomAI customAI = GetComponent<CustomAI>();

        if (customAI != null) {
            yield return customAI.Process();
        } else {
            switch (preset) {
                case Preset.Aggressive:
                    Move();

                    while (boardCharacter.isMoving) {
                        yield return null;
                    }

                    yield return new WaitForSeconds(0.3f);

                    bool attacked = Action();

                    if (attacked) {
                        yield return new WaitForSeconds(0.5f);
                    }
                    break;
                case Preset.Passive:
                    break;
                case Preset.Nothing:
                    break;
            }
            
        }

        yield return null;
    }

    /**
     * TODO [ALPHA] Add skills, etc.
     */
    private bool Action() {
        bool attacked = false;

        while (boardCharacter.actionable.CanDoAction()) {
            BoardCharacter target = null;

            foreach (BoardCharacter playerCharacter in battleManager.playerCharacters) {
                if (playerCharacter.GetSquare().GetManhattanDistance(boardCharacter.GetSquare()) == 1) {
                    if (target == null || target.character.GetCurrentHP() > playerCharacter.character.GetCurrentHP()) {
                        target = playerCharacter;
                    }
                }
            }

            if (target != null) {
                boardCharacter.BasicAttack(target);
                attacked = true;
            } else {
                // If no target found, stop attacking
                break;
            }
        }

        return attacked;
    }

    /**
     * Determine a path to get closer to a player's BoardCharacter
     */
    private void Move() {
        Path bestPath = null;
        int lowestCharHP = 0;

        foreach (BoardCharacter playerCharacter in battleManager.playerCharacters) {
            Path p = battleManager.board.pathFinder.FindPath(
                boardCharacter.GetSquare().x,
                boardCharacter.GetSquare().y,
                playerCharacter.GetSquare().x,
                playerCharacter.GetSquare().y,
                boardCharacter.side.value,
                boardCharacter.character.movementPoints
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

        if (bestPath != null && bestPath.GetLength() > 0) {
            boardCharacter.MoveThroughPath(bestPath, true);
        }
    }
}
