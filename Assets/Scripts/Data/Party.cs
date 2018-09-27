using System.Collections.Generic;
using UnityEngine;

namespace SF {
    [CreateAssetMenu(menuName = "SF/Systems/Party")]
    public class Party : ScriptableObject {
        public List<Character> characters;

        public Character GetNextCharacter(Character currentCharacter) {
            int index = characters.IndexOf(currentCharacter);

            if (index == -1) {
                return null;
            }

            if (index >= characters.Count - 1) {
                return characters[0];
            } else {
                return characters[index + 1];
            }
        }

        public Character GetPreviousCharacter(Character currentCharacter) {
            int index = characters.IndexOf(currentCharacter);

            if (index == -1) {
                return null;
            }

            if (index <= 0) {
                return characters[characters.Count - 1];
            } else {
                return characters[index - 1];
            }
        }
    }
}
