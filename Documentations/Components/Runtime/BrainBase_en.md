# BrainBase

#### **Namespace**: Unity.TinyCharacterController.Core
---

## Summary:
`BrainBase` is a fundamental component that drives the character. This component collects information from `IMove` and `ITurn` components within the same object and applies them to the character. Special processing for the highest priority is utilized through `IMoveUpdate` and `ITurnUpdate`. The specific method for reflecting position and rotation is determined by the inherited component. For instance, `TransformBrain` uses `UnityEngine.Transform.position`, while `RigidbodyBrain` uses `UnityEngine.Rigidbody.position`. Position updating is forcefully done using `Warp` or `Warp(UnityEngine.Vector3)`. The Position and Rotation of the Transform are cached and reused. Components inheriting from this should add `IEarlyUpdateComponent` to allow users to insert their own processing between the character's pre-computation (Check, Effect) and character reflection (Brain).

## Features and Operation:
- **Character Warping**: Instantly moves the character to a new position or orientation.
- **Dynamic Component Updates**: Updates components at runtime to control character movement and orientation.
- **Collision Avoidance**: Avoids contact between parent object colliders and child colliders, preventing the character from infinitely rising due to its own colliders.
- **Character Speed and Direction Control**: Adjusts the current movement speed and orientation of the character, considering additional velocities like gravity or impacts.
- **Camera Updates**: Updates the camera's position and orientation after updating the character's position and orientation.

## Properties
| Name | Description |
|------------------|------|
| `Settings` | Holds settings for the character. |
| `CachedTransform` | Caches the character's Transform. |
| `Position` | Represents the current position of the character. |
| `Rotation` | Represents the current rotation of the character. |
| `CurrentSpeed` | Returns the current movement speed of the character. |
| `TurnSpeed` | Returns the rotation speed of the character. |
| `YawAngle` | Returns the character's orientation in world space. |
| `LocalVelocity` | Returns the local vector for the direction the character is facing. |
| `ControlVelocity` | Returns the movement vector of the character in world space. |
| `EffectVelocity` | Returns additional movement vectors added to the character, like gravity or impacts. |
| `TotalVelocity` | Returns the sum of the character's movement vector and additional movement vectors. |
| `DeltaTurnAngle` | Returns the difference between the current and target direction of the character. |

## Methods
| Name | Function |
|------------------|------|
|  ``void`` Warp( ``Vector3 position, Vector3 direction`` )  | "Warps the character to a new position and orientation. If direction is Vector3.zero, the current orientation is maintained." |
|  ``void`` Warp( ``Vector3 position, Quaternion rotation`` )  | "Warps the character to a new position and new rotation." |
|  ``void`` Warp( ``Vector3 position`` )  | "Warps the character to a new position." |
|  ``void`` Warp( ``Quaternion rotation`` )  | "Warps the character to a new rotation." |
|  ``void`` Move( ``Vector3 position`` )  | "Moves the character to the specified position." |
|  ``protected abstract void`` ApplyPosition( ``in Vector3 totalVelocity, float deltaTime`` )  | "Applies the final position." |
|  ``protected abstract Vector3`` AdjustMovementVector( ``Vector3 controlVelocity, Vector3 externalVelocity`` )  | "Adjusts the movement vector." |
|  ``private void`` UpdateComponents( ``float deltaTime`` )  | "Updates components." |
|  ``protected void`` UpdateBrain( ``float deltaTime`` )  | "Updates information in the Brain." |
|  ``protected abstract void`` ApplyRotation( ``Quaternion rotation`` )  | "Applies rotation." |
|  ``protected abstract void`` SetPositionDirectly( ``in Vector3 newPosition`` )  | "Directly moves the character to the specified position." |
|  ``protected abstract void`` SetRotationDirectly( ``in Quaternion newRotation`` )  | "Directly sets the character's orientation to the specified rotation." |
|  ``protected abstract void`` MovePosition( ``in Vector3 newPosition`` )  | "Moves the character to the specified position." |
|  ``private void`` UpdateCamera( ``float deltaTime`` )  | "Updates the camera's position and orientation." |
|  ``protected abstract void`` OnUpdateCachedPosition( ``out Vector3 position, out Quaternion rotation`` )  | "Caches Position and Rotation for each inherited Brain." |

---
## Additional Notes
- `BrainBase` is an abstract class that needs to be inherited for implementing specific character control logic.
- Multiple interfaces are used to manage character movement and rotation.
- This code includes abstract methods (like `ApplyPosition`, `ApplyRotation`) that need to be implemented in the inherited classes for specific character movement and rotation implementations.