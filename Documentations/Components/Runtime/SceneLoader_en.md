# SceneLoader

#### **Namespace**: Unity.SceneManagement
---

## Summary:
`SceneLoader` is a component designed to load a specified asset scene. It manages scene loading and unloading, tracks loading progress, and provides event notifications when a scene is loaded or unloaded.

## Features and Operation:
- **Scene Loading**: Asynchronously loads the specified asset scene.
- **Loading Progress Tracking**: Tracks the progress of the loading process and provides a progress percentage.
- **Load Completion Event**: Triggers an `OnLoaded` event when the scene is loaded.
- **Scene Unloading**: Unloads the specified scene.
- **Unload Event**: Triggers an `OnUnloaded` event when the scene is unloaded.

## Properties
| Name | Description |
|------|-------------|
| `_scene` | The asset reference of the scene to be loaded. |
| `_priority` | Priority of the scene loading. |
| `_isActive` | Whether to set the loaded scene as the active scene. |
| `OnLoaded` | Event triggered when the scene is loaded. |
| `OnUnloaded` | Event triggered when the scene is unloaded. |
| `IsLoaded` | Indicates whether the scene is loaded. |
| `InProgress` | Indicates whether the scene loading is in progress. |
| `Progress` | Current loading progress percentage. |
| `Scene` | The loaded scene. |

## Methods
- There are no public methods.

