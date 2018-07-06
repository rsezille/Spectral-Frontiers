using TMPro;
using UnityEngine;

[RequireComponent(typeof(BoardEntity), typeof(BoardCharacter)), DisallowMultipleComponent]
public class CharacterNameHUD : MonoBehaviour {
    private TextMeshProUGUI characterName;
    private Transform instance;

    private BoardCharacter boardCharacter;

    public Transform characterNameHUD;

    private void Awake() {
        instance = Instantiate(characterNameHUD, GetComponent<BoardEntity>().transform);
        boardCharacter = GetComponent<BoardCharacter>();
    }

    private void Start() {
        SpriteRenderer sprite = GetComponentInChildren<SpriteRenderer>();
        float spriteSizeY = sprite ? sprite.bounds.size.y : 0;
        float offset = HealthBarHUD.BaseOffset;

        if (GetComponent<HealthBarHUD>()) {
            offset += 0.3f;
        }

        instance.transform.position += new Vector3(0f, spriteSizeY + offset);

        characterName = instance.Find("CharacterName").GetComponent<TextMeshProUGUI>();

        characterName.text = boardCharacter.character.name;
    }

    private void Update() {
        if (boardCharacter.IsDead()) {
            characterName.text = "(Dead) " + boardCharacter.character.name;
        } else {
            characterName.text = boardCharacter.character.name;
        }
    }
}
