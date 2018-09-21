using TMPro;
using UnityEngine;

namespace SF {
    public class CharacterNameHUDController : MonoBehaviour {
        private TextMeshProUGUI characterName;

        [Header("Dependencies")]
        public Character character;

        private void Awake() {
            if (!characterName) {
                characterName = GetComponentInChildren<TextMeshProUGUI>();
            }
        }

        private void Start() {
            characterName.SetText(character.characterName);
        }
    }
}
