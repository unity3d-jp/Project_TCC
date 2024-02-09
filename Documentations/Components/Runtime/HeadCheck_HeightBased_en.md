# HeadCheck (HeightBased) (HeightBasedHeadCheck.cs)

#### **Namespace**: Unity.TinyCharacterController.Check
---

## Summary:
`HeightBasedOverheadDetection` is a component that performs contact checks with overhead objects. However, the overhead check is done by height, not by Raycast, reducing processing time. This is intended for use in games with uncomplicated terrain or games without ceilings.

## Features and Operation:
- **HeadCheck (HeightBased) (HeightBasedHeadCheck.cs)**: Detects contact with objects overhead based on height.
- **Simplified Contact Determination**: Reduces processing load by using height instead of Raycast to determine overhead contact.

## Properties
| Name | Description |
|------------------|------|
| `RoofHeight` | The height determined as the ceiling. |
| `MaxHeight` | The maximum detectable height. |
| `IsObjectOverhead` | Indicates if there is an object overhead. |
| `RoofObject` | The object considered in contact when an overhead determination is made. |

## Methods
- There are no public methods.

