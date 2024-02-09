# AdditionalVelocity

#### **Namespace**: Unity.TinyCharacterController.Effect
---

## Summary:
`AdditionalVelocity` is a component that sets a custom acceleration for a character. This component reflects the externally set acceleration in the character's behavior through the Brain, without altering the acceleration itself.

## Features and Operation:
- **Acceleration Setting**: Allows external setting of acceleration for the character.
- **Speed Retrieval**: Calculates and retrieves the speed based on the set acceleration.
- **Acceleration Reset**: Enables resetting of the acceleration, returning the character's speed to zero.
- **Gizmo Visualization**: Utilizes Unity Editor's Gizmos to visually confirm the set acceleration.

## Properties
| Name | Description |
|------------------|------|
| `Velocity` | A `Vector3` property representing the acceleration applied to the character. |

## Methods
| Name | Function |
|------------------|------|
|  ``public void`` ResetVelocity( )  | Method to reset the acceleration. |
