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
    }
}
