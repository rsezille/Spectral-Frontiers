# Water reflection

During battles, some tiles can reflect their surroundings to simulate a water reflection effect.

## How to use

1. Add the *SF_WaterReflection* material to any tile that will reflect surroundings
2. Add the *Reflectable.cs* script to entities which will be reverted

During run time, all entities with the Reflectable script attached will duplicate their sprite renderer and flipping it by Y on the layer *Reflectable*. This way, only the *WaterReflectionCamera* will catch the inverted sprite and will draw it on water tiles.

## Tech

The SF_WaterReflection shader will grab everything that is displayed by the *WaterReflectionCamera* (child of *MainCamera*) and blend it to the current tile, creating the reflection effect.

The *WaterReflectionCamera* is based on the culling mask *Reflectable* where is drawn only the reverted sprites of the entities which are reflected.

## Distortion

You can also put a distortion effect to the reverted sprite to simulate waves during run time.

Just put the *SF_WaterDistortion* material in the *Distortion Shader* field of the *Reflectable* script of the desired sprite.