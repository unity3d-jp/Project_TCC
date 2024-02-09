# HeadPositionCheck (HeadCollisionCheck.cs)

#### **Namespace**: Unity.TinyCharacterController.Check
---

## Summary:
`HeadCollisionCheck` is a component that performs upward object detection. It conducts upward detection considering the height set in `CharacterSettings`. It also considers slightly ambiguous detection, not just complete contact with other objects, and calls UnityEvents upon collision.

## Features and Operation:
- **Upward Detection**: Detects objects above the character's head.
- **Contact and Distance Calculation**: Calculates whether there is contact with objects and the distance, and sets properties based on these calculations.
- **Collision Event Invocation**: Invokes Unity Events when the head makes contact with objects.

## Properties
| Name | Description |
|------------------|------|
| `_headPositionOffset` | Offset from the head position, used to determine contact with objects above. |
| `MaxHeight` | The maximum distance at which upward objects can be detected. |
| `_onContact` | Unity Event called when the head makes contact during the frame. |
| `_onChangeInRange` | Executes a Unity Event when the value of `InRange` changes. |
| `IsHitCollisionInThis Frame` | Indicates if the head is in contact with other objects during the frame. |
| `DistanceFromRootPosition` | The distance from the `ContactPoint` to the root position. |
| `IsHeadContact` | Indicates if the head is in contact with other objects. |
| `IsObjectOverhead` | Indicates if there is a collider within range above the head. |
| `ContactPoint` | The contact point if `IsHeadContact` is true, otherwise, the head position. |
| `ContactedObject` | The collided GameObject, null if `IsHeadContact` is false. |

## Methods
- There are no public methods.
