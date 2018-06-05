using TMPro;
using UnityEngine;

[RequireComponent(typeof(BoardEntity), typeof(PlayerCharacter)), DisallowMultipleComponent]
public class CharacterNameHUD : MonoBehaviour {
    private TextMeshProUGUI characterName;
    private Transform instance;

    public Transform characterNameHUD;

    private void Awake() {
        instance = Instantiate(characterNameHUD, GetComponent<BoardEntity>().transform);
    }

    private void Start() {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        float spriteSizeY = sprite ? sprite.bounds.size.y : 0;
        float offset = HealthBarHUD.BaseOffset;

        if (GetComponent<HealthBarHUD>()) {
            offset += 0.3f;
        }

        instance.transform.position += new Vector3(0f, spriteSizeY + offset);

        characterName = instance.Find("CharacterName").GetComponent<TextMeshProUGUI>();

        characterName.text = GetComponent<BoardCharacter>().character.name;
    }
}
