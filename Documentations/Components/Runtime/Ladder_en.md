# Ladder

#### **Namespace**: Unity.TinyCharacterController.Modifier
---

## Summary:
The `Ladder` component represents the configuration of a ladder and is used to control movement with the `MoveLadderControl`. It defines the behavior for climbing up or down the ladder and provides parameters to make character movements appear natural.

## Features and Operation:
- **Defining Ladder Position and Size**: Sets the start point, end point, and number of steps for the ladder.
- **Character Offset**: Sets the position offset for the character when using the ladder.
- **Access Points to the Ladder**: Defines the start positions for accessing the top and bottom of the ladder.

## Properties
| Name | Description |
|------|-------------|
| `_startOffset` | Offset for the ladder's start point. |
| `_characterOffset` | Offset for the character. |
| `_length` | Height of the ladder. |
| `_stepCount` | Number of steps. |
| `_topStartPosition` | Start position for accessing the top of the ladder. |
| `_bottomStartPosition` | Start position for accessing the bottom of the ladder. |

## Methods
| Name | Function |
|------|----------|
|  `Vector3` CloseStepPosition( `Vector3 position` )  | Obtains the coordinates of the step closest to the specified position. |
|  `Vector3` CloseStepPosition( `float point` )  | Obtains the coordinates of the step closest to the specified travel distance. |
|  `Vector3` ClosePosition( `Vector3 position` )  | Obtains the coordinates on the ladder closest to the specified position. |
|  `float` CloseStepPoint( `float point` )  | Obtains the travel distance of the step closest to the specified travel distance. |
|  `float` ClosePoint( `Vector3 position` )  | Obtains the travel distance on the ladder closest to the specified position. |

---
## Additional Notes
- The `Ladder` is used to define character actions related to the ladder. It provides important parameters for achieving natural movements when the character climbs the ladder and actions that match each step of the ladder.
- Detailed settings, such as the position of each step on the ladder and the start positions for accessing the top and bottom of the ladder, can enhance the usability of ladders in the game, making them more intuitive and realistic for players.
