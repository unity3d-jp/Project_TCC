# RangeTargetCheck

#### **Namespace**: Unity.TinyCharacterController.Check

---

## Summary:
`RangeTargetCheck` is a component that considers line of sight and obstacles to retrieve objects with specific tags within its range. This component runs every frame and invokes a callback if there are any changes in its content. It is primarily used to retrieve objects within a certain range.

## Features and Operation:
- **Sensor Center Point**: Specifies the position of the sensor.
- **Detectable Layers**: Specifies the layers that the sensor can detect.
- **Transparent Layer Ignore**: Used to specify objects that should be movable but not detectable, such as transparent windows.
- **Target Search Settings**: Allows setting properties such as the target's tag, search range, and whether to consider visibility.
- **Target Retrieval**: Retrieves a list of objects with certain tags.
- **Target Empty Check**: Checks whether there are no objects with the specified tag.
- **Nearest Target Retrieval**: Retrieves the nearest object with a specified tag.
- **Target Change Detection**: Retrieves newly added or removed targets within the specified tag range.

## Properties
| Name | Description |
|------------------|------|
| `_sensorOffset` | Specifies the offset of the sensor's center point. |
| `_hitLayer` | Specifies the layers that the sensor can detect. |
| `_transparentLayer` | Specifies the layer to ignore transparent objects. |
| `_searchData` | Contains settings for searching targets. |

## Methods
| Name | Function |
|------------------|------|
| ``public List<Transform>`` GetTargets(``string tagName``) | Returns a list of objects with the specified tag. |
| ``public List<Transform>`` GetTargets(``int index``) | Returns a list of objects with the tag at the specified index. |
| ``public bool`` IsEmpty(``int tagIndex``) | Checks if there are no objects with the specified tag. |
| ``public bool`` IsEmpty(``string tagName``) | Checks if there are no objects with the specified tag name. |
| ``public bool`` TryGetClosestTarget(``string tagName, out Transform target``) | Retrieves the closest object with the specified tag. |
| ``public int`` GetTagIndex(``string tagName``) | Gets the index of the specified tag name. |
| ``public bool`` TryGetClosestTarget(``string tagName, out Transform target, out Transform preTarget``) | Retrieves the closest object and the previous closest object with the specified tag. |
| ``public bool`` ChangedValues(``string tagName, out List<Transform> added, out List<Transform> removed``) | Retrieves objects that have been newly added or removed within the specified tag range. |

---
## Additional Notes
This component offers advanced capabilities for efficiently detecting and managing objects with specific tags in Unity game development. It is particularly useful for detecting objects near a character and executing processes based on changes in their state. Additionally, the control of layers and visibility allows for more precise target detection. These features can be applied in various scenarios, such as enemy detection, item discovery, and environmental awareness.

