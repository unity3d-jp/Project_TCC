# ManualCharacterTurn (ManualTurn.cs)

#### **Namespace**: Unity.TinyCharacterController.Control
---

## Summary:
`ManualCharacterTurn (ManualTurn.cs)` is a component for manually setting the orientation of a character. The use of `ManualControl` is recommended over this.

## Features and Operation:
- **Character Orientation Setting**: Uses the `_direction` vector to specify the character's orientation.
- **Turn Priority**: The priority is 0 if `_direction` is `Vector3.zero`; otherwise, it follows `TurnPriority`.
- **Turn Speed**: Sets the speed of direction change with `TurnSpeed`, ranging from -1 to 50.

## Properties
| Name | Description |
|------------------|------|
| `_direction` | Vector representing the character's orientation. |
| `TurnPriority` | Priority of turning. |
| `TurnSpeed` | Speed of orientation change. |

## Methods
- There are no public methods.

