using TMPro;
using UnityEngine;

[RequireComponent(typeof(BoardEntity), typeof(BoardCharacter)), DisallowMultipleComponent]
public class CharacterNameHUD : MonoBehaviour {
    private TextMeshProUGUI characterName;
    private Transform instance;

    private BoardCharacter boardCharacter;

    public Transform characterNameHUD;
    public float offset = 0.2f; // Used if there is no HealthBarHUD attached

    private void Awake() {
        instance = Instantiate(characterNameHUD, GetComponent<BoardEntity>().transform);
        boardCharacter = GetComponent<BoardCharacter>();
    }

    private void Start() {
        SpriteRenderer sprite = GetComponentInChildren<SpriteRenderer>();
        float spriteSizeY = sprite ? sprite.bounds.size.y : 0;
        float offset = this.offset;

        HealthBarHUD healthBarHUD = GetComponent<HealthBarHUD>();

        if (healthBarHUD) {
            offset = 0.3f + healthBarHUD.offset;
        }

        instance.transform.position += new Vector3(0f, spriteSizeY + offset);

        characterName = instance.Find("CharacterName").GetComponent<TextMeshProUGUI>();

        characterName.text = boardCharacter.character.name;
    }
}
