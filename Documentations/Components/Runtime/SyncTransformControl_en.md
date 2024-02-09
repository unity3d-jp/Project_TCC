# SyncTransformControl

#### **Namespace**: Unity.TinyCharacterController.Control
---

## Summary:
`SyncTransformControl` is a component designed to synchronize a character's position with that of a specific Transform. When the `MovePriority` is high, the character transitions to a position that matches the `Target` coordinates. Additionally, when `TurnPriority` is high, it updates the character's orientation to align with the target.

## Features and Operation:
- **Position Synchronization**: Synchronizes the character's position with the specified Transform's position.
- **Movement and Rotation Prioritization**: Sets priorities for movement and rotation actions to manage interactions with other components.
- **Smooth Transitions**: Utilizes `MoveTransitionTime` and `TurnTransitionTime` to smoothly transition to the target point and orientation.

## Properties
| Name | Description |
|------------------|------|
| `_targetTransform` | The Transform with which to synchronize the character's position. |
| `MoveTransitionTime` | The time it takes for the component to reach the target point when its move priority is higher than other components. |
| `TurnTransitionTime` | The time it takes for the component to align its orientation to the target direction when its turn priority is higher than other components. |
| `MovePriority` | The move priority of the character. |
| `TurnPriority` | The priority for changing the character's orientation. |

## Methods
- There are no public methods.

---
## Additional Notes
- Parameters are provided to enable smooth transitions for managing movement and rotation transitions. If no target Transform is set, priorities are set to 0.
