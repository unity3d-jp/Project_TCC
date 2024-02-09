# HighPriorityTargetSearch

#### **Namespace**: Unity.TinyCharacterController.Check
---

## Summary:
`HighPriorityTargetSearch` is a component that discovers the target with the lowest cost. This component calculates the cost by considering the angle to the target, the distance to the target, whether the target is visible, and whether it is captured by the MainCamera. The cost calculation can be customized. For example, it is used to determine which enemy to attack once discovered.

The calculation is performed only when `Find` is executed.

## Features and Operation:
- **Vision Range and Angle**: You can set the vision range (`Range`) and angle (`MaxAngle`) for detecting targets.
- **Visibility and Cost Calculation**: Calculates the cost based on whether the target is visible by the MainCamera and other conditions.
- **Customizable Cost Calculation**: You can customize the way the cost for discovered elements is calculated using the `OnCalculatePriority` action.
- **Discovering the Lowest Cost Target**: Use the `Find` method to discover the lowest cost target that meets the conditions within the range.

## Properties
| Name | Description |
|------------------|------|
| `HeadOffset` | The head height to determine visibility. |
| `Range` | The range of vision. |
| `MaxAngle` | The range of angles for objects to include in the detection. |
| `CostLimit` | The upper limit of the calculated cost. Targets with a cost greater than this value will be removed from the list. |
| `RadiusForIsInsightCheck` | The width of the ray for visibility checks. |
| `HitLayer` | The layer to include in the search target. |
| `TargetTags` | The tags to include in the search target. |
| `IsCalculateDistance` | Whether to calculate the distance determination to the character. |
| `IsCalculateAngle` | Whether to calculate the angle from direction (about `Find(out UnityEngine.Transform)`). |
| `IsCalculateIsInsight` | Whether to calculate the visibility from the character. |
| `IsCalculateVisible` | Whether to limit inclusion to objects seen by the MainCamera. |

## Methods
| Name | Function |
|------------------|------|
|  ``public void`` SetCost( ``int index, int cost`` )  | "Directly sets the cost for the specified element, primarily intended for use in VisualScripting." |
|  ``public bool`` Find( ``out Transform target`` )  | "Discovers the target with the lowest cost that matches the condition within the range using the Transform's Position and Forward for judgment." |
|  ``public bool`` Find( ``out Transform target, Vector3 direction`` )  | "Discovers the target with the lowest cost that matches the condition within the range using the Transform's Position for judgment." |

