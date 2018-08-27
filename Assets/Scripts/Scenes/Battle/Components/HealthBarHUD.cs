using UnityEngine;

[RequireComponent(typeof(BoardEntity), typeof(BoardCharacter)), DisallowMultipleComponent]
public class HealthBarHUD : MonoBehaviour {
    private BoardCharacter boardCharacter;
    private Transform healthBar;
    private Transform instance;

    public Transform healthBarHUD;
    public float offset = 0.2f;

    private void Awake() {
        boardCharacter = GetComponent<BoardCharacter>();

        instance = Instantiate(healthBarHUD, GetComponent<BoardEntity>().transform);
    }

    private void Start() {
        instance.position += new Vector3(0f, boardCharacter.sprite.bounds.size.y + offset);
        
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
