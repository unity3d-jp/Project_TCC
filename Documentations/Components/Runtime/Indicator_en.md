# Indicator

#### **Namespace**: Unity.TinyCharacterController.UI
---

## Summary:
The `Indicator` component serves as a utility for visually tracking a specific target within games and applications. It is designed primarily to be attached to UI elements and displays an icon representing the position of a target object. The icon's position and visibility adjust based on whether the target is on-screen or off-screen, and it can trigger custom events in response to changes in the target's visibility. This class is particularly useful for indicating the positions of players or objects in 2D or 3D environments.

## Features and Operation:
- **Target Tracking**: Tracks a specific target's position and displays it through a UI element.
- **On-Screen and Off-Screen Target Display**: Displays different icons depending on whether the target is on-screen or off-screen.
- **Visibility-Based Events**: Triggers events based on changes in the target's visibility.

## Properties
| Name | Description |
|------|-------------|
| `_updateTiming` | The timing of the component's update. Set according to whether the character moves during the `Update` or `FixedUpdate` timing. |
| `_target` | The `Transform` of the target to track. |
| `Offset` | The offset at which the indicator is displayed. |
| `_isLimitIconRange` | Whether the indicator adjusts its position to stay within the screen boundaries. |
| `_bounds` | Sets the position for UI adjustment when `_isLimitIconRange` is `true`. |
| `OnValueChanged` | Event called when the UI is either inside or outside the screen. |
| `_onScreenIcon` | The icon used when the target is visible on-screen. |
| `_offScreenIcon` | The icon used when the target is not visible on-screen. |
| `_isTurnOffScreenIcon` | Whether the icon rotates towards the target when the target is off-screen. |

## Methods
- There are no public methods.

---
## Additional Notes
- The `Indicator` functions as a UI element and should be placed as a child of a `Canvas` component.
- When the target is off-screen, the off-screen icon can rotate to point in the direction of the target, allowing users to intuitively know the direction of the target.
- Each time the target's visibility changes, configured events are triggered, enabling the execution of various custom actions.