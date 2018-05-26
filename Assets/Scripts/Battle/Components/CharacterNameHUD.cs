using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BoardEntity), typeof(BoardChar)), DisallowMultipleComponent]
public class CharacterNameHUD : MonoBehaviour {
    private Text characterName;
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

        characterName = instance.Find("CharacterName").GetComponent<Text>();

        characterName.text = GetComponent<BoardChar>().character.name;
    }
}
