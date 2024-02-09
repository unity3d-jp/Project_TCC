# CompositeSceneLoader

#### **Namespace**: Unity.SceneManagement
---

## Summary:
`CompositeSceneLoader` is a component designed to load multiple scenes in a sequential order. This component asynchronously loads a series of specified scenes and triggers an event once all scenes are loaded.

## Features and Operation:
- **Sequential Scene Loading**: `CompositeSceneLoader` loads scenes set in the `_sceneLoaders` array in the specified order.
- **Load Completion Event**: An `_onLoaded` event is triggered after all scenes have been loaded.
- **Load Progress Tracking**: The `PercentComplete` property allows tracking the current progress of the scene loading process.

## Properties
| Name | Description |
|------|-------------|
| `_sceneLoaders` | Sets the list of scenes to be loaded. |
| `_onLoaded` | The event that is triggered after all scenes have been loaded. |
| `IsLoaded` | Indicates whether all scenes have been loaded or not. |
| `PercentComplete` | Returns the current progress of the scene loading process as a percentage. |

## Methods
- There are no public methods.

---
## Additional Notes
- `CompositeSceneLoader` serves as a powerful tool for efficient scene management, particularly in large-scale applications or games. It simplifies the management of multiple scenes for developers, enhancing the user experience.

