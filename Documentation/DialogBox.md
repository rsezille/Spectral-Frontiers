# Dialog box

Dialog box is available in every scene of the game, as it is instanciated by GameManager.

## How it works

Dialog boxes use presets to be displayed. A preset is just a configuration of the dialog box with a custom background, custom font and so on.

You can not show two dialog boxes at the same time (could evolve in the future to simulate discussions).

Dialog boxes can be interacted with the keyboard or with the mouse.

Dialog boxes can be displayed in two different ways:

**1. Globally**

When using the standard show function, the dialog box will be displayed over the screen and will stick with it.

You can set the position (Top/Bottom) of the dialog box.

**2. Per board character**

A dialog box can also be shown for a targeted character. Its position will no longer be displayed over the screen but attached to a board character.

Can only be used in the Battle scene.

## Add a new preset

All transforms must be set to (0, 0, 0).

1. Select the dialog box prefab
2. Add a new child game object *PresetX* with a *DialogPreset.cs* component.
3. Add a canvas child with *ScreenSpaceCamera* and OrderInLayer of 10
4. Add an image (which is the background) as a canvas child
5. Add a TextMeshPro child to the image
6. Add a cursor child to the image with an animator component. The controller must have 3 states: *NoCursor* (Default state), *NextCursor* and *EndCursor*.
7. Link the cursor animator, TextMeshPro component, Canvas and Image to the DialogPreset script
8. Add an offset in DialogPreset which is roughly half of the background height. The offset is used to position the canvas at the top/bottom of the screen when using the dialog box globally.
