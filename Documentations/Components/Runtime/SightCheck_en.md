# SightCheck

#### **Namespace**: Unity.TinyCharacterController.Check

---

## Summary:
`SightCheck` is a component that performs sight detection, detecting targets within a specified range from the viewpoint and considering obstacles. It calls `InsightTargets` when some targets are within the sight or when all objects have exited the sight.

## Features and Operation:
- **Sight Settings**: Specifies the position of the head used for detection.
- **Range of Sight**: Sets the range of sight.
- **Angle of Sight**: Sets the angle of sight.
- **Detection Layer**: Specifies the layer to use for detection. Objects in this layer will be visible.
- **Detection Tags**: Specifies the tags used for detection. Objects with these tags will be visible.
- **List of Objects in Sight**: Retrieves a list of objects within sight.
- **Obstacle Check**: Checks for the presence of obstacles. Obstacle detection uses `CharacterSettings._environmentLayer`.
- **First Object in Sight**: Retrieves the first object found within sight.
- **Presence of Targets in Sight**: Checks if there are objects within sight.
- **Event on Entering/Exiting Sight**: Calls an event when an object enters or exits the sight.

## Properties
| Name | Description |
|------------------|------|
| `_headTransform` | Specifies the head position used for detection. |
| `Range` | Sets the range of sight. |
| `Angle` | Sets the angle of sight. |
| `VisibleLayerMask` | Specifies the layer used for detection. |
| `_targetTagList` | Specifies the tags used for detection. |
| `InsightTargets` | Retrieves a list of objects within sight. |
| `RaycastCheck` | Specifies whether to check for the presence of obstacles. |
| `InsightTarget` | Retrieves the first object found within sight. |
| `IsInsightAnyTarget` | Checks if there are objects within sight. |
| `OnChangeInsightAnyTargetState` | Calls an event when an object enters or exits the sight. |

## Methods
- There are no public methods.

---
## Additional Notes
The `SightCheck` component provides advanced functionality in Unity game development for detecting targets within a specific range in the character's field of vision, taking into account the presence of obstacles. It is particularly effective for detections based on the player's or enemy's field of vision, and for determining the visibility of targets based on the presence or absence of obstacles. The settings for the angle and range of vision enable precise detection, which can be utilized in various scenarios, such as detecting enemies, searching for items, and recognizing the environment.
