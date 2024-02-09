# WallCheck

#### **Namespace**: Unity.TinyCharacterController.Check
---

## Summary:
`WallCheck` is a component that performs collision detection with walls. It detects walls in the character's movement direction and triggers multiple Unity Events upon collision with a wall.

## Features and Operation:
- **Wall Collision Detection**: Detects walls in the character's movement direction and determines if there is contact.
- **Collision Callbacks**: Triggers multiple Unity Events upon collision with a wall.

## Properties
| Name | Description |
|------------------|------|
| `_wallAngleRange` | The range of angles recognized as a wall. |
| `_wallDetectionDistance` | Distance for wall detection. |
| `OnWallContacted` | Event triggered when contacting a wall. |
| `OnWallLeft` | Event triggered when leaving a wall. |
| `OnWallStuck` | Event triggered while in contact with a wall. |
| `IsContact` | Indicates whether there is contact with a wall. |
| `Normal` | The normal vector of the contact surface. |
| `ContactObject` | The object that was contacted. |
| `HitPoint` | The point of contact. |

## Methods
| Name | Function |
|------------------|------|
| ``public bool`` HitCheck( ``Vector3 direction, out Vector3 normal, out Vector3 point, out Collider contactCollider`` ) | Performs wall detection in the specified direction, returning the normal, contact point, and contacted object if contact is made. |

