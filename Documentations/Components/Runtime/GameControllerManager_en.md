# GameControllerManager

#### **Namespace**: Unity.TinyCharacterController.Core
---

## Summary:
`GameControllerManager` is a class inheriting from `MultiPhaseSingleton<GameControllerManager>` that adds callbacks to execute processing at arbitrary timings within the `MultiPhaseSingleton`. For instance, if a system implementing `IEarlyUpdate` is added to `UpdateTiming.Update`, its processing will be invoked before the `Update` phase.

## Features and Operation:
- **System Registration**: Registers systems for execution, allowing their callbacks to be executed at specified timings.
- **Management of Early and Late Updates**: Classes implementing `IEarlyUpdate` are added before the specified timing, whereas those implementing `IPostUpdate` are added after.

## Methods
| Name | Function |
|------------------|------|
|  ``public static void`` Register( ``ScriptableObject system, UpdateTiming timing`` )  | "Registers a system for processing at a specified timing." |
|  ``protected override void`` OnCreate( ``UpdateTiming timing`` )  | "Callback for when the class is created, setting up the update system based on specified timing." |
|  ``private void`` OnDestroy()  | "Callback for when the class is destroyed, cleaning up the registered systems." |
|  ``private void`` OnUpdate()  | "Callback invoked before the specified update timing." |
|  ``private void`` OnLateUpdate()  | "Callback invoked after the specified update timing." |

---
## Additional Notes
- `GameControllerManager` is utilized to customize Unity's `PlayerLoop` system for managing the update processes of specific game systems.
- This class provides the functionality to insert custom processing before or after specific update timings (e.g., `Update`, `FixedUpdate`, `LateUpdate`) within the Unity framework.
- By registering systems through the `Register` method, it offers the flexibility to control the order of update processes according to the game's logic.
