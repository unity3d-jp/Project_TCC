# RigidbodyBrain

#### **Namespace**: Unity.TinyCharacterController.Brain
---

## Summary:
`RigidbodyBrain` is a component that operates using a `UnityEngine.Rigidbody`. It functions with a `UnityEngine.CapsuleCollider` and `UnityEngine.Rigidbody`, positioning the character at a raised position defined by the `_stepHeight` value and stopping at a margin defined by the `_skinWidth` value during movement. The height and width of the `UnityEngine.CapsuleCollider` are determined by `CharacterSettings.Height` and `CharacterSettings.Radius`. It requires `IGravity` and `IGroundContact` for proper operation.

## Features and Operation:
- **Axis Locking**: Sets the axes along which the character can move.
- **Skin Width**: Sets the width between the character and walls.
- **Step Height**: Defines the height of steps the character can climb over.
- **Position and Rotation Update**: Updates the character's position and rotation.

## Properties
| Name | Description |
|------------------|------|
| `FreezeAxis` | Sets the axes to limit the movement of the character. |
| `LockAxis` | Vector3 format of `FreezeAxis`. |
| `_skinWidth` | Sets the width between the character and walls. |
| `_stepHeight` | Sets the height of steps the character can climb over. |

## Methods
| Name | Function |
|------------------|------|
| ``public void`` SetFreezeAxis( ``bool x, bool y, bool z`` )  | Sets the movable axes for the character. |

