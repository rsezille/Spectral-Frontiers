using SF;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BoardEntity), typeof(BoardCharacter)), DisallowMultipleComponent]
public class HealthBarHUD : MonoBehaviour {
    public static float BaseOffset = 0.2f;

    private BoardCharacter boardCharacter;
    private Transform healthBar;
    private ChangeColorByScale changeColorByScale;
    private Collider2D healthCollider;
    private Outline outline;
    private Transform instance;

    private CharacterNameHUD characterNameHUD; // Can be null

    public Transform healthBarHUD;

    private void Awake() {
        boardCharacter = GetComponent<BoardCharacter>();

        instance = Instantiate(healthBarHUD, GetComponent<BoardEntity>().transform);
    }

    private void Start() {
        SpriteRenderer sprite = GetComponentInChildren<SpriteRenderer>();
        float spriteSizeY = sprite ? sprite.bounds.size.y : 0;

        instance.position += new Vector3(0f, spriteSizeY + BaseOffset);

        characterNameHUD = GetComponent<CharacterNameHUD>();

        outline = instance.GetComponentInChildren<Outline>();
        healthCollider = instance.GetComponentInChildren<Collider2D>();
        healthBar = instance.Find("HealthBar").transform;
        changeColorByScale = healthBar.GetComponent<ChangeColorByScale>();

        healthBar.localScale = new Vector3(
            (float)boardCharacter.character.GetCurrentHP() / (float)boardCharacter.character.GetMaxHP(),
            healthBar.localScale.y,
            healthBar.localScale.z
        );
    }

    private void Update() {
        healthBar.localScale = new Vector3(
            (float)boardCharacter.character.GetCurrentHP() / (float)boardCharacter.character.GetMaxHP(),
            healthBar.localScale.y,
            healthBar.localScale.z
        );

        // PERF: can be consuming to do it in Update(), OverlapCollider perfs?
        Collider2D[] collidersHit = new Collider2D[1];
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(LayerMask.GetMask("SemiTransparent"));

        int collidersNb = healthCollider.OverlapCollider(contactFilter, collidersHit);

        if (collidersNb > 0) {
            if (boardCharacter.GetSquare().sortingGroup.sortingOrder > collidersHit[0].GetComponentInParent<Square>().sortingGroup.sortingOrder) {
                ChangeOpacity(0.16f, 1f);

                return;
            }

            if (collidersHit[0].GetComponent<SemiTransparent>().transparentObjectsCount == 0) {
                ChangeOpacity(0.05f, 0.5f);
            } else {
                ChangeOpacity(0.16f, 1f);
            }
        }
    }

    private void ChangeOpacity(float outlineOpacity, float otherOpacity) {
        outline.effectColor = new Color(outline.effectColor.r, outline.effectColor.g, outline.effectColor.b, outlineOpacity);
        changeColorByScale.minColor = new Color(changeColorByScale.minColor.r, changeColorByScale.minColor.g, changeColorByScale.minColor.b, otherOpacity);
        changeColorByScale.maxColor = new Color(changeColorByScale.maxColor.r, changeColorByScale.maxColor.g, changeColorByScale.maxColor.b, otherOpacity);

        if (characterNameHUD != null) {
            characterNameHUD.characterName.color = new Color(characterNameHUD.characterName.color.r, characterNameHUD.characterName.color.g, characterNameHUD.characterName.color.b, otherOpacity);
        }
    }
}
