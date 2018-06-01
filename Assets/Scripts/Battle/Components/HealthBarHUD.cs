using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BoardEntity), typeof(PlayerCharacter)), DisallowMultipleComponent]
public class HealthBarHUD : MonoBehaviour {
    public static float BaseOffset = 0.3f;

    private BoardCharacter boardCharacter;
    private Image healthBarImage;
    private Transform instance;

    public Transform healthBarHUD;

    private void Awake() {
        boardCharacter = GetComponent<BoardCharacter>();

        instance = Instantiate(healthBarHUD, GetComponent<BoardEntity>().transform);
    }

    private void Start() {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        float spriteSizeY = sprite ? sprite.bounds.size.y : 0;

        instance.position += new Vector3(0f, spriteSizeY + BaseOffset);

        healthBarImage = instance.Find("HealthBar").GetComponent<Image>();

        healthBarImage.transform.localScale = new Vector3(
            (float)boardCharacter.character.GetCurrentHP() / (float)boardCharacter.character.GetMaxHP(),
            healthBarImage.transform.localScale.y,
            healthBarImage.transform.localScale.z
        );
    }

    private void Update() {
        healthBarImage.transform.localScale = new Vector3(
            (float)boardCharacter.character.GetCurrentHP() / (float)boardCharacter.character.GetMaxHP(),
            healthBarImage.transform.localScale.y,
            healthBarImage.transform.localScale.z
        );
    }
}
