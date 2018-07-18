# Semi transparent entities

On the board, there are big entities, such as trees, that have no goal expect decorating the map. But they can hide squares, characters... and thus they can be very annoying for users.

One solution that has been implanted is to change the opacity of the big entities to show what is behind them.

## Technical

The entity prefab must have the *SFSemiTransparent.cs* script attached to them, as well as a collider2D to determine its bounds.

The default behavior is to change the opacity when the mouse enters/leaves the entity, as well as being semi transparent when hiding a board character.

That is why the above script will automatically add the `SemiTransparent` layer to instancied prefabs, enabling a `Collider2D.OverlapCollider()` with a layer mask to retrieve overlapped entities.

**Board character detection**

In order to detect if semi transparent entities are hiding board characters, when characters are moved on the board (placing or moving one), we emit two events:
- The first one will reset the opacity of all semi transparent entities
- The second will check for each board characters if they touch semi transparent entites and if so, update the opacity of the touched entites.
