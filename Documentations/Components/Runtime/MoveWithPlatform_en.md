# MoveWithPlatform

#### **Namespace**: Unity.TinyCharacterController.Effect
---

## Summary:
`MoveWithPlatform` is a component that moves a character riding on a moving object. If an object with a ground collider has the `MovePlatform` component, the character will follow its movement. When the character leaves the ground, the amount of movement is converted to acceleration and set to the character.

## Features and Operation:
- **Movement Following Moving Platform**: The character moves along with a moving platform it's riding on.
- **Friction and Aerodynamic Drag Settings**: Sets ground friction and aerodynamic drag, affecting the character's movement.
- **Acceleration Reset**: Provides a function to reset additional vectors by `MoveWithPlatform`. For instance, it can be used to reset the vector when jumping in the air.
- **Platform Presence Detection**: Contains a property to determine whether the character is on any platform.

## Properties
| Name | Description |
|------------------|------|
| `Friction` | A `float` property representing friction with the ground. |
| `Drag` | A `float` property indicating aerodynamic drag. |
| `OnPlatform` | A property indicating whether the character is on any platform. |

## Methods
| Name | Function |
|------------------|------|
|  ``public void`` ResetVelocity( )  | Method to reset the acceleration caused by `MoveWithPlatform`. |

