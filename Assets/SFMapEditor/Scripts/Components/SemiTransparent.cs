using UnityEngine;

namespace SF {
    /**
     * Change the attached game object layer with "SemiTransparent"
     */
    [RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
    public class SemiTransparent : MonoBehaviour {
        private SpriteRenderer spriteRenderer;

        [Header("Dependencies")]
        public BattleState battleState;

        [Header("Data")]
        // Because several objects can trigger the opacity change, we need to store the count of those objects
        public int transparentObjectsCount = 0;
        private bool hideCharacter = false;

        private void Awake() {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start() {
            gameObject.layer = LayerMask.NameToLayer("SemiTransparent");
        }

        public void CharacterHiding() {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.5f);
            hideCharacter = true;
        }

        /**
         * Triggered by Board and also by other objects (board character...)
         */
        public void MouseEnter() {
            if (battleState.currentBattleStep == BattleState.BattleStep.Cutscene || Time.timeScale == 0) return;

            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.5f);
            
            transparentObjectsCount++;
        }

        /**
         * Triggered by Board and also by other objects (board character...)
         */
        public void MouseLeave() {
            if (battleState.currentBattleStep == BattleState.BattleStep.Cutscene) return;

            transparentObjectsCount = Mathf.Min(0, transparentObjectsCount - 1);

            if (transparentObjectsCount <= 0 && !hideCharacter) {
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
            }
        }

        public void CheckSemiTransparent() {
            hideCharacter = false;

            if (transparentObjectsCount == 0) {
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
            }
        }
    }
}
