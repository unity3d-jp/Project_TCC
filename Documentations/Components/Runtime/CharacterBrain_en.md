# CharacterBrain

#### **Namespace**: Unity.TinyCharacterController.Brain
---

## Summary:
`CharacterBrain` is a component operating with Unity's `CharacterController`. It manages the height and width of an agent, which are determined by `CharacterSettings.Height` and `CharacterSettings.Radius`.

## Features and Operation:
- **Axis Locking**: Sets the axes along which the character can move.
- **Pushability**: Enables pushing when colliding with objects having a Rigidbody.
- **Collision Detection**: Detects collisions of the character.
- **Ground Detection**: Determines whether the character is in contact with the ground.
- **Position and Rotation Update**: Updates the position and rotation of the character.

## Properties
| Name | Description |
|------------------|------|
| `FreezeAxis` | Sets the axes to limit the movement of the character. |
| `LockAxis` | Vector3 format of `FreezeAxis`. |
| `Pushable` | Indicates whether the character can push other Rigidbody objects upon collision. |
| `DetectCollisions` | Enables or disables collision detection for the character. |

## Methods
| Name | Function |
|------------------|------|
| ``public void`` SetFreezeAxis( ``bool x, bool y, bool z`` )  | Sets the movable axes for the character. |

