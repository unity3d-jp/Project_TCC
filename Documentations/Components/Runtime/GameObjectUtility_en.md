# GameObjectUtility

#### **Namespace**: Unity.TinyCharacterController.Components.Utility
---

## Summary:
`GameObjectUtility` is a class that provides a static method for checking if a GameObject is included in a list of tags. This method checks whether a GameObject is included in the specified list of tags and returns true even if nothing is registered in the tag list.

## Features and Operation:
- **Tag Checking**: Checks if the tag of a GameObject is included in a specified list of tags.

## Methods
| Name | Function |
|------------------|------|
|  ``static bool`` ContainTag( ``GameObject obj, in string[] hitTags`` )  | Checks whether a GameObject is included in the specified list of tags. If the tag list is empty, it always returns true. |

---
## Additional Notes
- This class offers a basic utility for handling GameObject tags, which is useful for processing based on specific tags.
- If the `_hitTag` list is empty, the method always returns `true`, implying that processing can proceed without checking for specific tags.
