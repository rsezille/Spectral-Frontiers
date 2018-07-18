using UnityEngine;

[RequireComponent(typeof(BoardEntity), typeof(BoardCharacter)), DisallowMultipleComponent]
public class HealthBarHUD : MonoBehaviour {
    public static float BaseOffset = 0.2f;

    private BoardCharacter boardCharacter;
    private Transform healthBar;
    private Transform instance;

    public Transform healthBarHUD;

    private void Awake() {
        boardCharacter = GetComponent<BoardCharacter>();

        instance = Instantiate(healthBarHUD, GetComponent<BoardEntity>().transform);
    }

    private void Start() {
        SpriteRenderer sprite = GetComponentInChildren<SpriteRenderer>();
        float spriteSizeY = sprite ? sprite.bounds.size.y : 0;

        instance.position += new Vector3(0f, spriteSizeY + BaseOffset);
        
        healthBar = instance.Find("HealthBar").transform;

        UpdateScaleByHP();
    }

    private void Update() {
        UpdateScaleByHP();
    }

    private void UpdateScaleByHP() {
        healthBar.localScale = new Vector3(
            (float)boardCharacter.character.GetCurrentHP() / (float)boardCharacter.character.GetMaxHP(),
            healthBar.localScale.y,
            healthBar.localScale.z
        );
    }
}
