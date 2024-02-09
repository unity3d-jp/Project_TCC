# LookAtRig

#### **Namespace**: Unity.TinyCharacterController.Ik
---

## Summary:
`LookAtRig` is a component designed for characters to track a specific target with their gaze, using Inverse Kinematics (IK) to orient the character's head and eyes towards the target direction. This gaze tracking is often used to create natural character behaviors, enhancing realism and engagement in the game.

## Features and Operation:
- **Gaze Tracking**: Tracks the target with the character's gaze, ensuring the character naturally looks towards the target.
- **Weight and Range Adjustment**: Adjusts the weight of the gaze and the clamping angle to control the gaze movement.

## Properties
| Name | Description |
|------|-------------|
| `IsWork` | Flag indicating whether to direct the gaze towards the target. |
| `Target` | The `Transform` of the target to be looked at. |
| `TransitionTime` | The time it takes to switch the gaze on/off. |
| `bodyWeight` | The weight of the body in gaze tracking, indicating how much the body contributes to the gaze movement. |
| `headWeight` | The weight of the head in gaze tracking, indicating how much the head contributes to the gaze movement. |
| `eyeWeight` | The weight of the eyes in gaze tracking, indicating how much the eyes contribute to the gaze movement. |
| `clampAngle` | The clamping angle for gaze movement, limiting the range of motion. |

## Methods
- There are no public methods.

---
## Additional Notes
- `LookAtRig` implements the `IIkRig` interface, providing a basic framework for IK processing.
- Gaze tracking is crucial for adding realism and immersion to character behavior. Proper settings for weight and clamping angle can achieve natural gaze movements.