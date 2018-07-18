# Inputs

**This should be temporary. Use a professional and cross platform input manager for the final release.**

Everything happens in `InputManager.cs`  
It overrides the default unity input system to use custom names instead of keys. (*KeyCode.LeftArrow* becomes *InputManager.Left*)  
This way, we can modify a key very easily and it will be even more important when allowing the player to change bindings, and we don't need to know what key it is before doing an action.

## How to use

Before:  
```C#
if (Input.GetKeyDown(KeyCode.E)) { ... }
```
After:  
```C#
if (InputManager.Next.IsKeyDown) { ... }
```

### Axis
There are also shortcuts for axis:

In InputManager, just create a shortcut with your axis keys  
```C#
// InputManager.cs
public static int CameraHorizontalAxis() { return GetAxis(CameraRight, CameraLeft); }

// In another file:
if (InputManager.CameraHorizontalAxis() != 0) { ... }
```

**Exception**

There is one exception for the scrollwheel.  
As it is difficult to detect it via code, we still need to use the unity builtin input manager.

It is already mapped in InputManager, to use it:  
```C#
if (Input.GetAxis(InputManager.Axis.Zoom) != 0) { ... }
```
