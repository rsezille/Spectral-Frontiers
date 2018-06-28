# Spectral Frontiers Map Editor (SFME)

The SFME takes place in a special scene which can be found in the Assets/SFMapEditor folder.

**WARNING**
**You should touch the Map GameObject only to move entities (position of transform component). Nothing else.**

### How to use

The SFME only works when the SFMapEditor GameObject in the scene view is focused.

#### Add a new tileset

1. Create a subfolder in Assets/Resources/SFMapEditor/Tiles
2. Subfolder must start with `Ent_` if the tilset is composed by entities
3. Put the tile prefabs in the newly created folder
4. Select the new tileset in the SF Sprite Picker interface

### Features

#### Drag and draw

If you hold the left mouse button while clicking an empty square, you can fill other squares by holding the mouse button and moving the mouse.

#### Fill empty

Fill the empty squares with the selected tile
- Doesn't work with water
- Must be a tile (and not an entity)

### Shortcuts
*Available when focusing the SFMapEditor GameObject in the scene view*

- [R] Switch current mode (Draw, Selection, Height, Delete, Block)
- [G] Toggle grid gizmos
- [W] Use the special water tile
- [T] Switch current selection mode (Grid, Tile)
- [Hold Left Ctrl] (during Block mode only) Hide the map squares

### Technical details

The editor logic is splitted between several files:
- `SFMapEditor.cs` - data, square/tile/entity creation
- `SFMapEditorCustom.cs` - main logic, shortcuts
- `SFEditorWindow.cs` - editor window
- `SFSpritePickerCustom.cs` - tileset, sprite picker, water
- `SFMapGizmos.cs` - draw grid gizmos as well as hovered square gizmos
