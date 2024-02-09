# SceneLoaderExtensions

#### **Namespace**: Unity.SceneManagement
---

## Summary:
`SceneLoaderExtensions` is a static class that provides extension methods for `GameObject`. With this extension, you can easily retrieve the owner `GameObject` of a scene added through `SceneLoader`.

## Features and Operation:
- **Owner Retrieval**: Retrieves the owner of the scene to which a `GameObject` belongs.

## Methods
| Name | Function |
|------|----------|
|  ``public static GameObject`` GetOwner( ``this GameObject obj`` )  | Returns the owner `GameObject` of the scene to which the specified `GameObject` belongs. |

---
## Additional Notes
- This extension method is particularly useful for scenes dynamically loaded using `SceneLoader`, allowing any `GameObject` within such a scene to easily reference its owner.
- The owner refers to the `GameObject` responsible for managing the scene, typically the object holding the `SceneLoader` component that loaded the scene.
- It utilizes the `SceneLoaderManager.GetOwner(scene)` method to retrieve the owner `GameObject` associated with a given scene. This process leverages scene metadata and tracking information within the `SceneLoaderManager`.

This extension method enhances the flexibility of scene management and facilitates interactions with specific scenes and their contents, serving as a convenient tool for developers.

