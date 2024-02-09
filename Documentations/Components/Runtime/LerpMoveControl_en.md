# LerpMoveControl

#### **Namespace**: Unity.TinyCharacterController.Control
---

## Summary:
`LerpMoveControl` is a component designed to move the character from its current position to a target position, using linear interpolation. It is utilized for actions that require the character to move to specific locations, such as jumping or moving to a designated point. The target position is set using the `SetTarget` method, and it operates in conjunction with `MatchTargetBehaviour` within the Animator.

## Features and Operation:
- **Movement to Target Position**: Moves the character to the target position using linear interpolation.
- **Rotation Update**: Updates the character's orientation to match the target rotation.
- **Priority Control**: Assigns priorities to movement and rotation, managing interactions with other components.

## Properties
| Name | Description |
|------------------|------|
| `Priority` | The priority for movement and rotation. |
| `IsPlaying` | Indicates whether `LerpMoveControl` is currently active. |

## Methods
| Name | Function |
|------------------|------|
|  ``public void`` Play( ``PropertyName id`` )  | "Starts the target process with the specified ID." |
|  ``public void`` Stop( ``PropertyName id`` )  | "Stops the target process with the specified ID." |
|  ``public void`` SetNormalizedTime( ``float moveAmount, float turnAmount`` )  | "Sets the normalized time for the target position, where 0 is the start and 1 is the end." |
|  ``public void`` SetTarget( ``PropertyName id, Vector3 position`` )  | "Sets the target position." |
|  ``public void`` SetTarget( ``PropertyName id, Quaternion rotation`` )  | "Sets the target rotation." |
|  ``public void`` SetTarget( ``PropertyName id, Vector3 position, Quaternion rotation`` )  | "Sets both the target position and rotation." |
|  ``public void`` SetTarget( ``PropertyName id, Vector3 position, Vector3 offset`` )  | "Sets the target position with a specified offset." |
|  ``public void`` SetTarget( ``PropertyName id, Vector3 position, Quaternion rotation, Vector3 offset`` )  | "Sets the target position and rotation with a specified offset." |
|  ``public void`` RemoveTarget( ``PropertyName id`` )  | "Removes the target position information." |
|  ``public void`` Cancel( ``PropertyName id`` )  | "Cancels the currently running target process." |

---
## Additional Notes
- This component aims to smoothly transition the character to the target position, offering linear interpolation for both movement and rotation to achieve precise and controlled movements towards the set target.
