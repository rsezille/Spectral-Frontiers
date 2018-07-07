# Semi transparent entities

On the board, there are big entities, such as trees, that have no goal expect decorating the map. But they can hide squares, characters... and thus they can be very annoying for users.

One solution that has been implanted is to change the opacity of the big entities to show what is behind them.

## Technical

The entity prefab must have the *SFSemiTransparent.cs* script attached to them, as well as a collider2D to determine its bounds.

The default behavior is to change the opacity when the mouse enters/leaves the entity.

But we can also trigger the change of the opacity when mouseovering an object which has a part hidden behind an entity. For example if the head of a boardcharacter is hidden by a tree.  
That is why the above script will automatically add the `SemiTransparent` layer to instancied prefabs, enabling a `Collider2D.OverlapCollider` with a layer mask to retrieve overlapped entities.

Example:
```C#
Collider2D collider = GetComponentInChildren<Collider2D>();

Collider2D[] collidersHit = new Collider2D[20];
ContactFilter2D contactFilter = new ContactFilter2D();
contactFilter.SetLayerMask(LayerMask.GetMask("SemiTransparent"));

int collidersNb = collider.OverlapCollider(contactFilter, collidersHit);

if (collidersNb > 0) {
    for (int i = 0; i < collidersNb; i++) {
        collidersHit[i].GetComponent<SFSemiTransparent>().MouseEnter(); // or .MouseLeave()
    }
}
```
