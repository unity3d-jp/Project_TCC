# AnimatorMoveControl

#### **Namespace**: Unity.TinyCharacterController.Control
---

## Summary:
`AnimatorMoveControl` is a component for controlling character movement and rotation. This component is only active on frames when `WorkThisFrame()` is executed. It is intended for use with `AnimatorMoveBehaviour` and achieves movement using animation root motion.

## Features and Operation:
- **Movement and Rotation Priority Settings**: You can set priorities for movement and rotation with `MovePriority` and `TurnPriority`.
- **Using Ground Normal for Movement**: Calculates the character's movement vector in accordance with the slope of the ground.
- **Receiving Animation Root Motion**: Manages behavior in cases where different hierarchy objects own the animator.
- **Updating Position with Warp**: When `_isFixedPosition` is enabled, the character's position is updated using Warp.

## Properties
| Name | Description |
|------------------|------|
| `MovePriority` | Sets the priority for movement. |
| `TurnPriority` | Sets the priority for rotation. |
| `UseGroundNormal` | Sets whether to use the ground slope for movement. |

## Methods
| Name | Function |
|------------------|------|
|  ``public void`` Rebuild( )  | Reconnects with the animator. |
