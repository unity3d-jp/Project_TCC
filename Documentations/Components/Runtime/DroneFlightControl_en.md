# DroneFlightControl

#### **Namespace**: Unity.TinyCharacterController.Control
---

## Summary:
`DroneFlightControl` is a component that moves the character independently of the terrain. Movement of the character is controlled with `MoveHorizontal` or `MoveVertical`.

## Features and Operation:
- **Movement Settings**: Configures maximum speed, acceleration, braking power, and turning speed.
- **Directional Look Control**: Determines whether to face the input direction or the direction of movement using `LookForward`.
- **Horizontal-Only Movement**: Sets whether to ignore the pitch of the camera and move only horizontally with `IsOnlyHorizontal`.
- **Movement and Turn Priorities**: Sets priorities for movement and turning.

## Properties
| Name | Description |
|------------------|------|
| `Speed` | Maximum speed. |
| `Accel` | Amount of acceleration. |
| `Brake` | Braking power. |
| `TurnSpeed` | Speed of turning. |
| `LookForward` | Specifies whether to look in the direction of movement or input. |
| `IsOnlyHorizontal` | Determines if movement is only horizontal. |
| `MovePriority` | Priority for movement. |
| `TurnPriority` | Priority for turning. |

## Methods
| Name | Function |
|------------------|------|
|  ``public void`` MoveHorizontal( ``Vector2 input`` )  | Moves in the horizontal direction. The direction is adjusted based on the camera's orientation. |
|  ``public void`` MoveVertical( ``float input`` )  | Moves vertically, up for positive values and down for negative. |

