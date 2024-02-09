# RootMotionReceiver

#### **Namespace**: Unity.TinyCharacterController.Modifier
---

## Summary:
The `RootMotionReceiver` component is utilized to capture and apply root motion data from animations to a game object. It extracts velocity and rotational changes from the `Animator` component and makes these values accessible for further use, such as moving or rotating the game object based on the animation's root motion.

## Features and Operation:
- **Root Motion Capture**: Retrieves velocity and rotational changes caused by root motion from the `Animator`.
- **Exposing Velocity and Rotation**: Provides access to the velocity and rotation, which can be used to move and rotate the game object accordingly.

## Properties
| Name | Description |
|------|-------------|
| `Velocity` | The velocity obtained from the `Animator`. |
| `Rotation` | The per-frame rotational difference obtained from the `Animator`. |

## Methods
| Name | Function |
|------|----------|
|  ``void`` Awake( )  | Acquires the `Animator` component during component initialization. |
|  ``void`` OnAnimatorMove( )  | Invoked when the animator moves, updating the velocity and rotation. |

---
## Additional Notes
- The `RootMotionReceiver` is used in conjunction with character controllers or physics systems to move characters based on the root motion defined in animations, offering a more realistic and intuitive control over character movements driven by the animator.
