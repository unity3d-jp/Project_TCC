Certainly! I will now provide the English translation of the analysis for the `ManualControl` code.

# ManualControl

#### **Namespace**: Unity.TinyCharacterController.Control
---

## Summary:
`ManualControl` is a component designed to directly set the movement and orientation of a character. It sets the direction of movement and orientation, using these values to move the character.

## Features and Operation:
- **Movement Settings**: Moves the character according to the `MoveVelocity` value.
- **Movement Priority**: If `MovePriority` is higher than other components, the character moves according to this value.
- **Turn Settings**: Rotates the character towards the `TurnAngle`.
- **Turn Priority**: If `TurnPriority` is higher than other values, the character turns in the specified direction.
- **Turn Speed**: Sets the speed of direction change with `TurnSpeed`. Negative values change direction instantly without interpolation.

## Properties
| Name | Description |
|------------------|------|
| `MovePriority` | Priority of movement. |
| `MoveVelocity` | Movement vector of the character. |
| `TurnPriority` | Priority of turning. |
| `TurnSpeed` | Speed of orientation change, adjustable from -1 to 50. |
| `TurnAngle` | The direction in which the character faces (world coordinates). |
| `TurnDirection` | The direction of the character's facing, ignoring the Y-axis. |
| `TurnRotation` | Character's rotation, ignoring the Y-axis. |

## Methods
- There are no public methods.

---
## Additional Notes
- Requires the `CharacterSettings` class.

