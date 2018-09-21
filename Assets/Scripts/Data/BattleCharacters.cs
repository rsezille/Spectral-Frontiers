using System.Collections.Generic;
using UnityEngine;

namespace SF {
    [CreateAssetMenu(menuName = "SF/Systems/BattleCharacters")]
    public class BattleCharacters : ScriptableObject {
        public List<BoardCharacter> player;
        public List<BoardCharacter> enemy;

        public void ResetData() {
            player.Clear();
            enemy.Clear();
        }

        /*
        Replace Character.boardCharacter with this for a cleaner solution but more expensive one

        public BoardCharacter GetBoardCharacter(Character character) {
            foreach (BoardCharacter boardCharacter in player) {
                if (boardCharacter.character == character) {
                    return boardCharacter;
                }
            }

            foreach (BoardCharacter boardCharacter in enemy) {
                if (boardCharacter.character = character) {
                    return boardCharacter;
                }
            }

            return null;
        }
        */
    }
}
