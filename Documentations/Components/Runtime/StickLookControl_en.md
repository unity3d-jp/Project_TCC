# StickLookControl

#### **Namespace**: Unity.TinyCharacterController.Control
---

## Summary:
`StickLookControl` is a component that updates the character's orientation to the direction specified by the `Look` method. The character faces the direction indicated by the stick.

## Features and Operation:
- **Orientation Adjustment**: If `TurnPriority` is high, the character turns in the direction indicated by the stick movement.
- **Camera Orientation Compensation**: The direction indicated by the stick is compensated based on the camera's orientation.

## Properties
| Name | Description |
|------|-------------|
| `TurnPriority` | The rotation priority. When higher than other components' priorities, it rotates in the direction specified by the stick. |
| `_turnSpeed` | Speed of orientation change. |

## Methods
| Name | Function |
|------|----------|
| ``public void`` Look( ``Vector2 rightStick`` ) | Turns the character in the direction specified by the stick. X is left and right in screen space, and Y is up and down in screen space. |

---
## Additional Notes
- This component is designed to adjust the character's orientation based on stick input.
- It includes a function to adjust the character's orientation considering the camera's orientation.
- The `Look` method ensures the character faces the direction indicated by the stick input.

