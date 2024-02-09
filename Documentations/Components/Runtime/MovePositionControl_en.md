# MovePositionControl

#### **Namespace**: Unity.TinyCharacterController.Control
---

## Summary:
`MovePositionControl` is a component designed for moving a character to a specified position. It operates within the context of `IMove`.

## Features and Operation:
- **Movement**: Moves the character to the coordinates specified by `SetTargetPosition(Vector3)`.
- **Turning**: If `TurnPriority` is high, the character will face the direction of movement.
- **Terrain Following**: If `_ignoreYAxis` is set to false, the character will move along the terrain.
- **Obstacle Avoidance**: This component does not have obstacle avoidance capabilities. Another component is required for avoiding obstacles.

## Properties
| Name | Description |
|------|-------------|
| `Speed` | Maximum movement speed of the character. |
| `TurnSpeed` | Rotation speed of the character. |
| `MovePriority` | Movement priority of the character. |
| `TurnPriority` | Rotation priority of the character. |
| `OnArrivedAtDestination` | Callback when the destination is reached. |
| `_maxSlope` | Angle of slopes that the character can move on. |
| `_ignoreYAxis` | Whether to ignore the Y-axis for movement. |
| `_currentSpeed` | Current movement speed. |
| `IsArrived` | Whether the character has arrived at the target destination. |

## Methods
| Name | Function |
|------|----------|
| ``public void`` SetTargetPosition( ``Vector3 position`` ) | Moves the character to a specified position. |
| ``public void`` SetTargetPosition( ``Vector3 position, bool ignoreYAxis`` ) | Moves the character to a specified position with an option to ignore the Y-axis. |
| ``public void`` SetTargetPosition( ``Vector3 position, float distance`` ) | Moves the character around a specified position while maintaining a certain distance. |
| ``public void`` SetTargetPosition( ``Vector3 position, float distance, bool ignoreYAxis`` ) | Moves the character around a specified position with distance and Y-axis ignore options. |
| ``public void`` SetTargetPosition( ``Vector3 position, float distance, bool ignoreYAxis, int cycleAround`` ) | Moves the character around a specified position with distance, Y-axis ignore, and cycling options. |

---
## Additional Notes
- This component lacks the ability to avoid obstacles. A different component is necessary for obstacle avoidance.
- Target positions are dynamically set using the `SetTargetPosition` method.
- Movement and turning priorities may affect interactions with other movement components.

