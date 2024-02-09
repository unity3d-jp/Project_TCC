# SwingHitDetector

#### **Namespace**: Unity.TinyCharacterController.Utility
---

## Summary:
`SwingHitDetector` is a component designed to detect and notify of collision events during actions such as swings, attacks, and movements, as well as collisions with other colliders. It performs collision detection based on a defined range and the movement path of the contact point, primarily intended for scenarios like weapon swing attacks and collision detection with traps.

## Features and Operation:
- **Collision Detection Configuration**: Sets up positions and sizes for collision detection.
- **Collision Event Notification**: Notifies of detected collision events.

## Properties
| Name | Description |
|------|-------------|
| `Timing` | The timing for collision detection. |
| `Owner` | The owner of this component. |
| `CacheTargetType` | The type of cached target. |
| `HitLayer` | The layer used for collision detection. |
| `HitTags` | An array of tags for which collision detection is performed. |
| `OnHitObjects` | Event triggered when colliding with objects. |
| `_detectorPositions` | Positions and sizes for collision detection. |


## Methods
| Name | Function |
|------|----------|
|  ``Vector3`` GetContactPosition( ``GameObject obj`` )  | Retrieves the position where the specified object made contact in the past. |
|  ``Vector3`` GetContactPosition( ``Collider col`` )  | Retrieves the position where the specified collider made contact in the past. |

---
## Additional Notes
- `SwingHitDetector` inherits from `CollisionDetectorBase`, providing fundamental functionalities for collision detection.
- Collision detection configurations are defined using the `LineHitData` structure, specifying detection positions and sizes.
- Detected collision events are notified through the `OnHitObjects` event, allowing for triggering specific actions or reactions in response.
