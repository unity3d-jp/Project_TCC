# MoveNavmeshControl

#### **Namespace**: Unity.TinyCharacterController.Control
---

## Summary:
`MoveNavmeshControl` is a component for Navmesh-based movement of a character. It utilizes the component specified by `_agent` to conduct pathfinding to the coordinates set by `SetTargetPosition(Vector3)` and moves the character within the context of `IMove`. When `MovePriority` is high, the character moves along the shortest path set by the NavmeshAgent. If `TurnPriority` is high, the character will turn towards the destination.

## Features and Operation:
- **Movement Agent Setup**: An agent used for controlling character movement, performing asynchronous path calculations.
- **Movement and Orientation Priorities**: Sets the priorities for movement and turning, governing the character's actions accordingly.
- **Destination Movement**: Uses `SetTargetPosition(Vector3)` to set the destination coordinates and move towards it.

## Properties
| Name | Description |
|------------------|------|
| `_agent` | The NavMeshAgent used for character movement control. |
| `_speed` | Maximum character movement speed. |
| `TurnSpeed` | Character's turn speed. |
| `MovePriority` | Priority for movement. |
| `TurnPriority` | Priority for turning. |
| `OnArrivedAtDestination` | Callback event when the destination is reached. |

## Methods
| Name | Function |
|------------------|------|
| ``public void`` SetTargetPosition( ``Vector3 position`` ) | Sets the target position for movement. |
| ``public void`` SetTargetPosition( ``Vector3 position``, ``float distance`` ) | Sets the target position to maintain a specified distance from the target. |

