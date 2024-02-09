# LadderMoveControl

#### **Namespace**: Unity.TinyCharacterController.Control
---

## Summary:
`LadderMoveControl` is a component that implements the process of climbing a ladder. This component moves the character within the movement range specified by the `Ladder` component. The priority is only effective when the ladder to be climbed is registered with the character.

## Features and Operation:
- **Ladder Utilization**: Grabs and releases the ladder using `GrabLadder` and `ReleaseLadder`.
- **Movement and Rotation Priority**: The priority for movement and rotation is applied only when connected to a ladder.
- **Time to Reach Next Step**: Sets the time to reach the next step on the ladder.
- **Step Movement Curve**: Sets the animation curve for step movement.
- **Completion Event**: Configures the event that occurs when the character reaches the start or end of the ladder.

## Properties
| Name | Description |
|------------------|------|
| `Priority` | Priority for movement and rotation. |
| `StepTime` | Time to reach the next step. |
| `_curve` | Animation curve for step movement. |
| `_onComplete` | Event when the character arrives at the start or end of the ladder. |
| `CurrentLadder` | The ladder currently connected to. |

## Methods
| Name | Function |
|------------------|------|
|  ``public void`` GrabLadder( ``Ladder ladder`` )  | Method to grab the ladder. |
|  ``public void`` ReleaseLadder( )  | Method to release the ladder. |
|  ``public void`` Move( ``float direction`` )  | Method to move the character on the ladder's path. |
|  ``public void`` AdjustCharacterPosition( )  | Method to adjust the character's position. |

---
## Additional Notes
- This component requires the `CharacterSettings` component and collaborates with the `Ladder` object.
- Use `GrabLadder` and `ReleaseLadder` methods to connect or disconnect the character to or from the ladder.
- The character automatically releases from the ladder upon reaching the endpoint, but can also release voluntarily.
