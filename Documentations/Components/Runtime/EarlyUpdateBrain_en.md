# EarlyUpdateBrain

#### **Namespace**: Unity.TinyCharacterController.Core
---

## Summary:
`EarlyUpdateBrain` is a class that inherits from `EarlyUpdateBrainBase` and performs character update processing within the `Update` method, which corresponds to the timing of frame updates.

## Features and Operation:
- **Synchronization with Frame Updates**: Utilizes the `Update` method to execute character update processing at every frame update timing.

## Methods
| Name | Function |
|------------------|------|
|  ``private void`` Update()  | "Performs character update processing at the timing of frame updates." |

---
## Additional Notes
- `EarlyUpdateBrain` is used when there are specific update processes that need to be executed within the `Update` method, corresponding to each frame's update.
- This class uses the `DefaultExecutionOrder` attribute to set the execution order to `Order.EarlyUpdateBrain`, ensuring it runs before other `Update` methods.
- The `AddComponentMenu("")` attribute ensures it does not appear in the editor's component menu.

