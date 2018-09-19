using SF;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/**
 * Compute the mouse position and dispatch events to the first object hit
 */
public class BoardDispatcher : MonoBehaviour {
    private List<SemiTransparent> previousSemiTransparents = new List<SemiTransparent>(); // Used to detect a mouse leave
    private MouseReactive previousMouseEntity = null; // Used to detect a mouse leave

    [Header("Dependencies")]
    public BattleState battleState;

    /**
     * Compute the current mouse position and dispatch events to the first object hit
     */
    private void Update() {
        if (battleState.currentTurnStep != BattleState.TurnStep.Status &&
                (battleState.currentBattleStep == BattleState.BattleStep.Placing || battleState.currentBattleStep == BattleState.BattleStep.Fight)) {
            Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            MouseReactive entityHit = null;
            List<SemiTransparent> semiTransparentsHit = new List<SemiTransparent>();

            // Process mouse position
            foreach (RaycastHit2D hit in Physics2D.RaycastAll(position, Vector2.zero)) {
                // If the mouse is hovering a HUD, don't trigger any board entity
                if (hit.collider.tag == "HUD") {
                    entityHit = null;
                    break;
                }

                // If the mouse is hovering a semi transparent sprite, triggers it before checking for MouseReactive
                SemiTransparent sFSemiTransparent = hit.collider.gameObject.GetComponent<SemiTransparent>();

                if (sFSemiTransparent != null) {
                    semiTransparentsHit.Add(sFSemiTransparent);
                }

                MouseReactive mr = hit.collider.gameObject.GetComponent<MouseReactive>();

                // Only trigger game objects that react to the mouse
                if (mr != null) {
                    EntityContainer entityContainer = mr.GetComponentInParent<EntityContainer>();
                    EntityContainer previousEntityContainer = entityHit != null ? entityHit.GetComponentInParent<EntityContainer>() : null;

                    // This first if is for checking map related entities
                    if (entityContainer != null && previousEntityContainer != null) {
                        if (entityContainer.GetComponentInParent<Square>().GetComponent<SortingGroup>().sortingOrder > previousEntityContainer.GetComponentInParent<Square>().GetComponent<SortingGroup>().sortingOrder
                                || (mr.GetComponentInParent<BoardCharacter>() != null && entityContainer == previousEntityContainer)
                                || (entityContainer == previousEntityContainer && mr.GetComponentInParent<SortingGroup>().sortingOrder > entityHit.GetComponentInParent<SortingGroup>().sortingOrder)) {
                            entityHit = mr;
                            continue;
                        }
                    } else {
                        if (entityHit == null) {
                            entityHit = mr;
                            continue;
                        } else {
                            // This part is to check objects that are higher than maps
                            SortingGroup groupEntity = mr.GetComponentInParent<SortingGroup>();
                            SortingGroup groupPreviousEntity = entityHit.GetComponentInParent<SortingGroup>();

                            if (groupEntity != null && groupEntity.sortingLayerName == "Battle" && groupEntity.sortingOrder > 0) {
                                if (groupPreviousEntity == null || groupPreviousEntity.sortingLayerName != "Battle") {
                                    entityHit = mr;
                                } else {
                                    if (groupPreviousEntity.sortingLayerName == "Battle" && groupPreviousEntity.sortingOrder <= groupEntity.sortingOrder) {
                                        entityHit = mr;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Dispatch events to entity
            if (previousMouseEntity != null && previousMouseEntity != entityHit) {
                previousMouseEntity.MouseLeave?.Invoke();
            }

            if (entityHit != null) {
                if (InputManager.Click.IsKeyDown) {
                    entityHit.Click?.Invoke();
                }

                if (entityHit == previousMouseEntity) {
                    //entity.MouseOver?.Invoke();
                } else {
                    entityHit.MouseEnter?.Invoke();
                }
            }

            // Dispatch events to semi transparent sprites
            foreach (SemiTransparent previousSemiTransparent in previousSemiTransparents) {
                if (!semiTransparentsHit.Contains(previousSemiTransparent)) {
                    previousSemiTransparent.MouseLeave();
                }
            }

            foreach (SemiTransparent semiTransparentHit in semiTransparentsHit) {
                if (!previousSemiTransparents.Contains(semiTransparentHit)) {
                    semiTransparentHit.MouseEnter();
                }
            }

            previousSemiTransparents.Clear();
            previousSemiTransparents.AddRange(semiTransparentsHit);

            previousMouseEntity = entityHit;
        }
    }
}
