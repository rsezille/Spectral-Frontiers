﻿using SF;
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
    
    private BoardCharacter boardCharacter;

    [Header("Dependencies")]
    public BattleCharacters battleCharacters;
    public Board board;

    [Header("Data")]
    public Preset preset = Preset.Aggressive;

    private void Awake() {
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

        while (boardCharacter.CanDoAction()) {
            BoardCharacter target = null;

            foreach (BoardCharacter playerCharacter in battleCharacters.player) {
                if (playerCharacter.GetSquare().GetManhattanDistance(boardCharacter.GetSquare()) == 1) {
                    if (target == null || target.character.currentHP > playerCharacter.character.currentHP) {
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

        foreach (BoardCharacter playerCharacter in battleCharacters.player) {
            Path p = board.pathFinder.FindPath(
                boardCharacter.GetSquare().x,
                boardCharacter.GetSquare().y,
                playerCharacter.GetSquare().x,
                playerCharacter.GetSquare().y,
                boardCharacter.side.value,
                boardCharacter.movementPoints
            );

            if (p != null) {
                if (bestPath == null) {
                    bestPath = p;
                    lowestCharHP = playerCharacter.character.currentHP;
                } else {
                    if (p.GetLength() > boardCharacter.movementPoints + 1) {
                        if (p.GetLength() < bestPath.GetLength()) {
                            bestPath = p;
                            lowestCharHP = playerCharacter.character.currentHP;
                        }
                    } else {
                        if (bestPath.GetLength() > boardCharacter.movementPoints + 1) {
                            bestPath = p;
                            lowestCharHP = playerCharacter.character.currentHP;
                        } else {
                            if (playerCharacter.character.currentHP < lowestCharHP) {
                                bestPath = p;
                                lowestCharHP = playerCharacter.character.currentHP;
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
