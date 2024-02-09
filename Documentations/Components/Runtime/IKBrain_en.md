# IKBrain

#### **Namespace**: Unity.TinyCharacterController.Ik
---

## Summary:
`IKBrain` is a component designed to control Inverse Kinematics (IK) for more natural character animations within Unity. It works in conjunction with the `Animator` component to apply specific IK rigs to character animations, enhancing realism and flexibility in movements.

## Features and Operation:
- **IK Rig Management**: Collects and manages IK rigs within the game object.
- **Additional IK Rigs**: Allows for the specification of additional IK rig targets, facilitating the management of multiple IK setups.
- **Animation Offset**: Applies an offset to the body position of the `Animator`, introducing variations to character movements.

## Properties
| Name | Description |
|------|-------------|
| `AdditionalRig` | Specifies additional IK rig targets. |
| `animatorOffset` | The offset applied to the body position of the `Animator`. |

## Methods
- There are no public methods.

---
## Additional Notes
- `IKBrain` relies on the `Animator` component and works in tandem with the Animator's IK functionalities to operate.
- By setting up additional IK rigs, it becomes possible to simultaneously manage multiple IK systems, adding depth and complexity to character animations.
- Utilizing the animation offset feature allows for fine-tuning character poses, providing an additional layer of customization to animations.
