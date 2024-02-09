# AnimatorModifierCheck

#### **Namespace**: Unity.TinyCharacterController.Check
---

## Summary:
The `AnimatorModifierCheck` component monitors changes in stored information and extracts added or removed elements. It issues a callback when there's a change in keys, primarily expected to be used in conjunction with AnimationModifierBehaviour for adding or removing keys.

## Features and Operation:
- **Key Addition and Removal**: Designed with the expectation that keys will be added or removed through AnimationModifierBehaviour.
- **Key Change Monitoring**: Monitors whether any key was added or removed during the frame.
- **Update Timing Specification**: Specifies whether to use FixedUpdate (if the Animator operates in Physics) or Update for the update timing.

## Properties
| Name | Description |
|------------------|------|
| `_updateMode` | Specifies the timing for updates. |
| `OnChangeKey` | A callback for when a key is added or removed. |
| `ChangeKey` | Indicates whether any key was added or removed during the frame. |
| `CurrentKeys` | A list of currently active keys. |

## Methods
| Name | Function |
|------------------|------|
|  ``public bool`` HasKey( ``PropertyName key`` )  | "Checks whether the specified key is held." |
|  ``public bool`` HasKey( ``string key`` )  | "Checks whether the specified key is held." |
|  ``public bool`` IsRemoved( ``PropertyName key`` )  | "Checks whether the specified key was removed during this frame." |
|  ``public bool`` IsRemoved( ``string key`` )  | "Checks whether the specified key was removed during this frame." |
|  ``public bool`` IsAdded( ``PropertyName key`` )  | "Checks whether the specified key was added during this frame." |
|  ``public bool`` IsAdded( ``string key`` )  | "Checks whether the specified key was added during this frame." |

---
## Additional Notes
- Supports dynamic animation modification by monitoring the addition, removal, and changes of keys and executing callbacks accordingly.