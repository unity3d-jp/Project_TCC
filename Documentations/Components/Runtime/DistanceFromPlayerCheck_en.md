# DistanceFromPlayerCheck

#### **Namespace**: Unity.TinyCharacterController
---

## Summary:
`DistanceFromPlayerCheck` is a component that retrieves the distance from objects with the "Player" tag and emits a callback when the player enters a specified range. It collects a list of objects with this component, calculates the distance to objects with the "Player" tag, and stores it in `Distance`. The distance index, stored in `DistanceIndex`, is determined from `_ranges`, with the first element as 0 and incrementing based on the distance of elements. `OnChangeDistanceIndex` is called when the distance index changes. When the component is disabled, no calculations are performed, `DistanceIndex` is set to -1, and `Distance` is set to `float.MaxValue`.

## Features and Operations:
- **Distance Ranges from Player**: Specifies the range of distances from the player. For example, if specified as 1, 5, and 8, it means that if the distance from the player is 2.3m, the index is 1, and if it's 7m, the index is 2.
- **Callback on Distance Range Change**: A callback that is invoked when the range of distances from the player changes.

## Properties
| Name | Description |
|------------------|------|
| `_ranges` | An array specifying the ranges of distances from the player. |
| `DistanceIndex` | The current distance (range) index from the player. Returns -1 if the component is not registered or the player does not exist. |
| `Distance` | The distance from the player. Returns `float.MaxValue` if the component is not registered or the player does not exist. |
| `OnChangeDistanceIndex` | An event that is triggered when the distance index changes. |

## Methods
| Name | Function |
|------------------|------|
| ``public static List<GameObject>`` GetIndexObjects( ``int distanceIndex`` ) | "Retrieves a list of objects corresponding to the specified distance index." |
