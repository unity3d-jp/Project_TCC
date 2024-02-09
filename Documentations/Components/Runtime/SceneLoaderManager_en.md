# SceneLoaderManager

#### **Namespace**: Unity.SceneManagement
---

## Summary:
`SceneLoaderManager` is a static class responsible for managing the loading and unloading of asset scenes. It tracks loaded scenes and their owner objects, overseeing the progress of scene loading and unloading operations.

## Features and Operation:
- **Scene Loading**: Asynchronously loads scenes using asset references.
- **Scene Unloading**: Asynchronously unloads specified scenes.
- **Loading Progress Tracking**: Aggregates and returns the progress of all currently loading scenes.
- **Owner Object Management**: Registers and tracks owner objects for each scene.

## Methods
| Name | Function |
|------|----------|
|  ``public static AsyncOperationHandle<SceneInstance>`` Load( ``AssetReferenceScene scene, string sceneName, GameObject ownerObject, int priority = 100`` )  | Loads a scene using the specified asset reference and returns a handle to the loading operation. |
|  ``public static void`` Unload( ``AssetReferenceScene scene, string sceneName, Action onComplete = null`` )  | Unloads a scene using the specified asset reference. |
|  ``public static GameObject`` GetOwner( ``Scene scene`` )  | Retrieves the owner object for the specified scene. |
|  ``public static bool`` HasProgress  | Indicates whether there are scenes currently loading. |
|  ``public static float`` Progress(  )  | Returns the current progress of scene loading as a percentage. |

---
## Additional Notes
- The manager maintains a list of `AssetReferenceScene` objects to track loaded scenes and uses a dictionary with `PropertyName` keys to manage owner objects for each scene.
- The `Load` and `Unload` methods utilize the `AddressableAssets` system for efficient asset management and dynamic content loading, enabling efficient asset handling and dynamic loading of content.

