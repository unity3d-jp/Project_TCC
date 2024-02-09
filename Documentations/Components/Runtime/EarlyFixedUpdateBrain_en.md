# EarlyFixedUpdateBrain

#### **Namespace**: Unity.TinyCharacterController.Core
---

## Summary:
`EarlyFixedUpdateBrain` is a class that inherits from `EarlyUpdateBrainBase` and performs character update processing within the `FixedUpdate` method, which is the timing for physics updates.

## Features and Operation:
- **Synchronization with Physics Updates**: Utilizes the `FixedUpdate` method to execute character update processing in sync with the physics calculations.

## Methods
| Name | Function |
|------------------|------|
|  ``private void`` FixedUpdate()  | "Performs character update processing at the time of physics updates." |

---
## Additional Notes
- `EarlyFixedUpdateBrain` is used when specific update processes need to be executed within `FixedUpdate` to synchronize with Unity's physics system.
- This class uses the `DefaultExecutionOrder` attribute to set the execution order to `Order.EarlyUpdateBrain`, ensuring it runs before other `FixedUpdate` methods.
- The `AddComponentMenu("")` attribute ensures it does not appear in the editor's component menu.
