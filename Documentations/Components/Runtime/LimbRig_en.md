# LimbRig

#### **Namespace**: Unity.TinyCharacterController.Ik
---

## Summary:
`LimbRig` is a component designed to control Inverse Kinematics (IK) for specific limbs (such as arms or legs) in Unity. It works in conjunction with the `Animator` component to ensure the designated limb of the character follows the positions of specified targets and hints.

## Features and Operation:
- **IK Application**: Applies IK to the specified limb, making it follow the positions of the target and hint.
- **Weight Adjustment**: Dynamically adjusts the weight (the extent of IK application), facilitating smooth transitions.

## Properties
| Name | Description |
|------|-------------|
| `isWorking` | Flag indicating whether the IK is active. |
| `_ikGoal` | The limb to which IK is applied. |
| `_ikHint` | The position of the limb that serves as a hint for IK. |
| `_target` | The `Transform` that serves as the IK target. |
| `_hint` | The `Transform` for the IK hint. |
| `transitionToEnable` | The transition time to enable IK. |
| `transitionToDisable` | The transition time to disable IK. |

## Methods
- There are no public methods.

---
## Additional Notes
- `LimbRig` implements the `IIkRig` interface, providing a basic framework for IK processing.
- The application of IK requires an `Animator` component and operates in coordination with the Animator's IK functionalities.
- Weight adjustment ensures that the application of IK is smoothly enabled or disabled, achieving natural movements.
