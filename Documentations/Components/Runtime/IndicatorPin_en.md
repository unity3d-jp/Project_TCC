# IndicatorPin

#### **Namespace**: Unity.TinyCharacterController.UI
---

## Summary:
The `IndicatorPin` component synchronizes UI elements with 3D space coordinates, adjusting the UI's position to align with the coordinates specified in `WorldPosition`. It utilizes a `CanvasGroup` to conceal the UI when it moves off-screen, ensuring that UI elements only appear when within the player's view.

## Features and Operation:
- **3D Space Coordination**: Aligns the UI's position with specified coordinates in 3D space, ensuring that UI elements correspond to relevant game objects or locations.
- **Off-Screen Concealment**: Automatically hides the UI elements when they are not within the camera's view, using the `CanvasGroup`'s alpha property to manage visibility.
- **Position Offset**: Allows for the adjustment of the UI's position relative to the `WorldPosition`, providing the ability to fine-tune the exact location of UI elements in relation to their intended 3D position.

## Properties
| Name | Description |
|------------------|------|
| `_cameraUpdateTiming` | Determines when the component updates, based on camera movement within either the Update or FixedUpdate frames. |
| `_worldPosition` | The world coordinates associated with the UI element. |
| `_positionOffset` | An offset applied to the world coordinates, allowing for precise positioning of the UI element. |
| `WorldPosition` | The position in the world where the UI element is intended to appear. This can be set externally to update the UI's position. |
| `CorrectedPosition` | The world position of the UI, adjusted by the offset to determine its final display location. |
| `UiSize` | The size of the UI element, as defined by its RectTransform. |

## Methods
- The component automatically adjusts the UI's position based on changes to `WorldPosition`, with visibility controlled through the CanvasGroup based on whether the UI is within the camera's view. There are no direct methods exposed for developers to manipulate these behaviors further.

---
## Additional Notes
- `IndicatorPin` requires both `CanvasGroup` and `RectTransform` components to function effectively. These components are utilized for controlling the visibility of the UI and adjusting its position within the game's UI layer.
- The UI's visibility when off-screen is managed by setting the `CanvasGroup`'s alpha to 0, effectively making the UI element transparent and invisible.
- The UI's position updates are calculated based on the set `WorldPosition` and `PositionOffset`, ensuring that UI elements are displayed at precise locations within the 3D world, enhancing the immersion and interactivity of the game's UI elements with the game world.
