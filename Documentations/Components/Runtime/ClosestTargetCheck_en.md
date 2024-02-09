# ClosestTargetCheck

#### **Namespace**: Unity.TinyCharacterController.Check
---

## Summary:
`ClosestTargetCheck` is a component that retrieves the nearest target within a certain range.

## Features and Operations:
- **Target Tag Specification**: Specifies the tag to be targeted for the search.
- **Search Range Specification**: Specifies the search range (radius).
- **Target Layer Specification**: Specifies the layer of the search target.
- **Detection of the Closest Target**: Detects and sets the closest collider within the specified range as the target.
- **Event Triggering on Target Change**: Triggers the `OnChangeClosestTarget` event when the target changes.

## Properties
| Name | Description |
|------------------|------|
| `_tag` | The tag that will be targeted for the search. |
| `_radius` | The search range (radius). |
| `_layer` | The layer of the search target. |
| `Target` | The collider found as a result of the search. |
| `PreTarget` | The collider detected in the previous frame. |
| `OnChangeClosestTarget` | An event called when the target has changed. |

## Methods
- There are no public methods.