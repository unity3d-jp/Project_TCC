# Gravity

#### **Namespace**: Unity.TinyCharacterController.Effect
---

## Summary:
`Gravity` is a component that applies gravitational acceleration to characters. It adds downward acceleration at the speed set in Gravity, and the acceleration multiplier can be adjusted for each character. There is no acceleration while in contact with the ground. Events are executed at the timing of landing and takeoff. Components that move up and down, such as jumping, may manipulate this value.

## Features and Operation:
- **Application of Gravitational Acceleration**: Applies downward gravitational acceleration to the character.
- **Setting Gravity Multiplier**: Allows for adjustment of the gravitational acceleration multiplier, affecting the falling speed.
- **Landing and Takeoff Events**: Executes events at the moments of landing and leaving the ground.
- **Retrieval of Fall Speed**: Provides the current fall speed, indicating negative values for falling and positive values for rising.

## Properties
| Name | Description |
|------------------|------|
| `OnLanding` | An event triggered upon landing on the ground. |
| `OnLeave` | An event triggered when the character leaves the ground. |
| `FallSpeed` | Represents the current fall speed. |
| `GravityScale` | A property that represents the gravity multiplier. |
| `IsLeaved` | Indicates whether the character has left the ground in the current frame. |
| `IsLanded` | Indicates whether the character has landed on the ground in the current frame. |
| `CurrentState` | Represents the current state of the character, whether in air or on the ground. |

## Methods
| Name | Function |
|------------------|------|
|  ``public void`` SetVelocity( ``Vector3 velocity`` )  | A method to set the fall speed. |

