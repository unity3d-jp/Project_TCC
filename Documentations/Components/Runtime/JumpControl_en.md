# JumpControl

#### **Namespace**: Unity.TinyCharacterController.Control
---

## Summary:
`JumpControl` is a component that controls jumping behavior. When the `Jump` method is executed, it controls gravity and moves upwards. The priority is only effective during the jump operation. `TurnPriority` is effective only when the movement is set to the horizontal direction.

## Features and Operation:
- **Jump Control**: Sets jump height, number of aerial jumps, and aerodynamic drag.
- **Turning Speed**: Sets the speed at which the character will change direction. If this value is -1, the direction changes immediately.
- **Preparation Time for Jumping**: Sets the time `_standbyTime` for the jump to become possible. If a jump becomes possible within this time, it will automatically jump.
- **Movement and Rotation Priority**: Sets the priority for the character's movement and turning.
- **Callbacks**: Called when a jump is requested and just before jumping, regardless of whether `IsAllowJump` is true.

## Properties
| Name | Description |
|------------------|------|
| `JumpHeight` | Height of the jump. |
| `MaxAerialJumpCount` | Maximum number of jumps in the air. |
| `Drag` | Aerodynamic drag. |
| `_turnSpeed` | Speed of turning the character. |
| `_standbyTime` | Time for the jump to be ready. |
| `MovePriority` | Priority for movement. |
| `TurnPriority` | Priority for turning. |
| `OnReadyToJump` | Callback called when ready to jump. |
| `OnJump` | Callback called just before jumping. |
| `AerialJumpCount` | Current number of aerial jumps. |

## Methods
| Name | Function |
|------------------|------|
|  ``public void`` Jump( ``bool incrementJumpCount`` )  | Requests a jump and jumps at the timing when it becomes possible. |
|  ``public void`` ForceJump( ``bool incrementJumpCount`` )  | Forces a jump, ignoring `IsAllowJump` and jump count. |
|  ``public void`` ResetJump( )  | Resets the jump-related settings. |

---
## Additional Notes
- This component requires `CharacterSettings`, `IGravity`, `IGroundContact`, and `IOverheadDetection` interfaces.
- The direction and intensity of the jump are set through the `JumpDirection` property, which does not consider the character's orientation.
- The `ForceJump` method immediately executes a jump regardless of jump conditions.