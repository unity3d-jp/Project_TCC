# MoveControl

#### **Namespace**: Unity.TinyCharacterController.Control
---

## Summary:
`MoveControl` is a component designed to control the movement of a character. It uses `Move` to relocate the character's position. The direction of movement is adjusted based on the camera position and ground incline, and the character's orientation is also corrected according to the movement direction. The character moves if `MovePriority` is higher than other components, and turns in the direction of movement if `TurnPriority` is higher. The `TurnSpeed` determines the speed of the turn when the character changes direction.

## Features and Operation:
- **Movement Settings**: Sets the character's maximum movement speed, brake strength, acceleration, and the angle of movable slope.
- **Movement Restriction**: Restricts the character's movement to specific axis directions.
- **Movement Priority**: Controls movement if the value is higher than other components.
- **Turn Settings**: Sets the rotation speed of the character.
- **Turn Priority**: Controls orientation if the value is higher compared to other priorities.

## Properties
| Name | Description |
|------------------|------|
| `_moveSpeed` | Maximum movement speed of the character. |
| `_turnSpeed` | Rotation speed of the character. |
| `_brakePower` | Brake strength. |
| `_accelerator` | Acceleration. |
| `_angle` | Angle of the movable slope. |
| `_lockAxis` | Axis for movement restriction. |
| `_movePriority` | Movement priority. |
| `_moveStopThreshold` | Threshold for movement stop. |
| `TurnPriority` | Turn priority. |
| `_turnStopThreshold` | Threshold for turn stop. |
| `CurrentSpeed` | Current movement speed. |
| `MoveVelocity` | Movement vector in world coordinates. |
| `LocalVelocity` | Character-based movement vector. |
| `Direction` | Direction of movement in world coordinates. |
| `Velocity` | Current velocity vector. |

## Methods
| Name | Function |
|------------------|------|
| ``public void`` Move( ``Vector2 leftStick`` ) | Moves the character in the specified direction. |
| ``public void`` StartLockAxis( ``Vector3 axis`` ) | Restricts movement to a specific axis. |
| ``public void`` StopLockAxis() | Releases movement restriction. |

