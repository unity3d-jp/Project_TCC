# CursorPositionControl

#### **Namespace**: Unity.TinyCharacterController.Control
---

## Summary:
`CursorPositionControl` is a component that updates the character's orientation based on the cursor position. If the component has high priority, the character will look in the direction of the cursor. The coordinates at which the character gazes are calculated based on `LookTargetPoint`. To use a side view instead of a top-down view, the `_planeAxis` can be changed.

## Features and Operation:
- **Cursor Behavior Settings**: The `_maxDistance` setting is used to limit the range of camera movement, for example, if you want the camera to follow the cursor position.
- **Character Orientation Control**: Settings for the priority and speed of facing the cursor direction.
- **Judgment Plane Direction Settings**: Setting the direction of the plane used for cursor position judgment. 

## Properties
| Name | Description |
|------------------|------|
| `_maxDistance` | The maximum distance of the cursor, used to limit the range of camera movement. |
| `_originOffset` | Offset to compensate for orientation to the cursor. |
| `_planeAxis` | Direction of the judgment plane, accommodating top or side viewpoints. |
| `TurnPriority` | Priority for facing the cursor direction. |
| `_turnSpeed` | Speed of orientation adjustment towards the cursor. |

## Methods
| Name | Function |
|------------------|------|
|  ``public void`` LookTargetPoint( ``Vector2 screenPosition`` )  | Sets the direction the character should face based on screen coordinates. |

