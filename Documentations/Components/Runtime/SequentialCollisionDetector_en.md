# SequentialCollisionDetector

#### **Namespace**: Unity.TinyCharacterController.Utility
---

## Summary:
`SequentialCollisionDetector` is a custom component for Unity, designed to perform collision detection using multiple detection points in a step-by-step manner. This is especially useful for scenarios where characters or objects in a game need to detect collisions with other objects using multiple partial detection areas.

## Features and Operation:
- **Multiple Detection Points**: Utilizes multiple detection points for more precise collision detection.
- **Step-by-Step Collision Detection**: Performs collision detection in phases for more accurate results.
- **Visualization of Detection Range**: Visually displays the detection range in the editor for easier debugging.

## Properties
| Name | Description |
|------|-------------|
| `Timing` | The timing for collision detection. |
| `Owner` | The owner of this component. |
| `CacheTargetType` | The type of the cached target. |
| `HitLayer` | The layer used for collision detection. |
| `HitTags` | Array of tags for which collision detection is performed. |
| `OnHitObjects` | Event triggered when colliding with objects. |
| `_frame` | The detection frame range. |
| `_hitPositions` | Configuration of detection positions and their effective ranges. |


## Methods
- There are no public methods.

---
## Additional Notes
- `SequentialCollisionDetector` inherits from `CollisionDetectorBase`, providing fundamental functionalities for collision detection.
- By setting collision data (`CollisionData`) for each detection point, the detection range can be finely controlled.
- The settings for the detection range and detection frame can be intuitively adjusted within the editor, facilitating the tuning of collision detection behavior in actual gameplay.