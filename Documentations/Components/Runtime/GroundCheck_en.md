# GroundCheck

#### **Namespace**: Unity.TinyCharacterController.Check
---

## Summary:
`GroundCheck` is a component that performs collision detection with the ground. It determines whether the ground is present, the orientation of the contact surface, information about the contact object, and notifies any changes in the ground object. The calculations for this component are executed at the timing of OnUpdate.

## Features and Operation:
- **Ground Detection**: Detects collision with the ground and determines whether it is in contact.
- **Ground Contact Information Retrieval**: Provides information about the presence of the ground, the orientation of the contact surface, and the contact object.
- **Ground Object Change Notification**: Issues a Unity Event when there is a change in the ground object.

## Properties
| Name | Description |
|------------------|------|
| `_ambiguousDistance` | The maximum distance at which the character can be recognized as being on the ground, used for ambiguous ground detection. |
| `_preciseDistance` | The distance at which the character is recognized as being on the ground, used for strict ground detection. |
| `_maxSlope` | The maximum slope at which the ground is recognized as "ground." |
| `_onChangeGroundObject` | Unity Event invoked when the ground collider changes. |
| `IsOnGround` | Indicates if the character is in contact with the ground. |
| `IsFirmlyOnGround` | Indicates if the character is firmly in contact with the ground. |
| `MaxGroundCheckDistance` | The maximum distance for ground check. |
| `GroundCollider` | The current ground collider. |
| `GroundSurfaceNormal` | The orientation of the ground surface. |
| `GroundContactPoint` | The contact point with the ground. |
| `DistanceFromGround` | The distance from the ground. |
| `GroundObject` | The ground object. |
| `IsChangeGroundObject` | Indicates if the ground object has changed in the current frame. |

## Methods
| Name | Function |
|------------------|------|
| ``public bool`` Raycast( ``Vector3 position, float distance, out RaycastHit hit`` ) | Performs a Raycast that ignores the Collider attached to the character. |

