using SF;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/**
 * Load the map
 * Compute the mouse position and dispatch events to the first object hit
 * Propagate marks for moving and attacking
 */
public class Board : MonoBehaviour {
    private Square[] squares;

    private List<SemiTransparent> previousSemiTransparents = new List<SemiTransparent>(); // Used to detect a mouse leave
    private MouseReactive previousMouseEntity = null; // Used to detect a mouse leave

    public PathFinder pathFinder;
    
    public int width { get; private set; }
    public int height { get; private set; }

    public string mapName { get; private set; } = "";

    private void Awake() {
        width = 0;
        height = 0;
    }

    /**
     * Compute the current mouse position and dispatch events to the first object hit
     */
    private void Update() {
        if (BattleManager.instance.currentTurnStep != BattleManager.TurnStep.Status &&
                (BattleManager.instance.currentBattleStep == BattleManager.BattleStep.Placing || BattleManager.instance.currentBattleStep == BattleManager.BattleStep.Fight)) {
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
                    EntityContainer entityContainer = mr.transform.parent.GetComponent<EntityContainer>();
                    EntityContainer previousEntityContainer = entityHit != null ? entityHit.transform.parent.GetComponent<EntityContainer>() : null;

                    // This first if is for checking map related entities
                    if (entityContainer != null && previousEntityContainer != null) {
                        if (entityContainer.transform.parent.GetComponent<SortingGroup>().sortingOrder > previousEntityContainer.transform.parent.GetComponent<SortingGroup>().sortingOrder
                                || (mr.GetComponentInParent<BoardCharacter>() != null && mr.transform.parent == entityHit.transform.parent)) {
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

    public void LoadMap(RawMission mission) {
        GameObject mapGameObject = Resources.Load("Maps/" + mission.map) as GameObject;

        if (mapGameObject != null) {
            GameObject map = Instantiate(mapGameObject, Vector3.zero, Quaternion.identity) as GameObject;

            Square[] mapSquares = map.GetComponentsInChildren<Square>();

            width = 0;
            height = 0;

            foreach (Square mapSquare in mapSquares) {
                if (mapSquare.x > width) width = mapSquare.x;
                if (mapSquare.y > height) height = mapSquare.y;
            }

            width++; // Squares start from index 0
            height++; // Squares start from index 0

            squares = new Square[width * height];

            foreach (Square mapSquare in mapSquares) {
                int squareIndex = mapSquare.x + (mapSquare.y * width);

                squares[squareIndex] = mapSquare;
            }

            foreach (RawMission.RawStartingSquare startingSquare in mission.startingSquares) {
                squares[PositionToIndexSquare(startingSquare.posX, startingSquare.posY)].startingDirection = EnumUtil.ParseEnum<BoardCharacter.Direction>(startingSquare.direction, BoardCharacter.Direction.North);
            }

            pathFinder = new PathFinder(this, this.width + this.height);

            mapName = LanguageManager.instance.GetString("map." + mission.map + ".name");
        } else {
            Debug.LogError("Map not found! " + mission.map);
        }
    }

    /**
     * Return the square according to x and y coordinates ; null if coordinates are outside the board
     */
    public Square GetSquare(int x, int y) {
        if (x > width - 1 || x < 0 || y > height - 1 || y < 0) {
            return null;
        }

        return squares[PositionToIndexSquare(x, y)];
    }

    public int PositionToIndexSquare(int x, int y) {
        return x + (y * width);
    }

    /**
     * Return all square hit by a linear propagation, epicenter excluded
     * ignoreBlocking is usefull for attacks
     */
    public List<Square> PropagateLinear(Square epicenter, int distance, Side.Type side, bool ignoreBlocking) {
        List<Square> squaresHit = new List<Square>();

        List<Square> tmp1 = new List<Square>();
        List<Square> tmp2 = new List<Square>();

        squaresHit.Add(epicenter);
        tmp1.Add(epicenter);

        for (int i = 0; i < distance; i++) {
            foreach (Square square in tmp1) {
                Square north = GetSquare(square.x, square.y - 1);
                Square south = GetSquare(square.x, square.y + 1);
                Square west = GetSquare(square.x - 1, square.y);
                Square east = GetSquare(square.x + 1, square.y);

                if (north != null && !squaresHit.Contains(north) && ((north.IsNotBlocking(side) && !ignoreBlocking) || ignoreBlocking)) {
                    tmp2.Add(north);
                }

                if (south != null && !squaresHit.Contains(south) && ((south.IsNotBlocking(side) && !ignoreBlocking) || ignoreBlocking)) {
                    tmp2.Add(south);
                }

                if (west != null && !squaresHit.Contains(west) && ((west.IsNotBlocking(side) && !ignoreBlocking) || ignoreBlocking)) {
                    tmp2.Add(west);
                }

                if (east != null && !squaresHit.Contains(east) && ((east.IsNotBlocking(side) && !ignoreBlocking) || ignoreBlocking)) {
                    tmp2.Add(east);
                }
            }

            tmp1.Clear();
            tmp1.AddRange(tmp2);
            squaresHit.AddRange(tmp2);
            tmp2.Clear();
        }

        squaresHit.Remove(epicenter);

        return squaresHit;
    }
}
