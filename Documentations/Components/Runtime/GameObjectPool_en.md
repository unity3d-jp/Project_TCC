# GameObjectPool

#### **Namespace**: Unity.TinyCharacterController.Utility
---

## Summary:
`GameObjectPool` is a component for reusing GameObjects. It retrieves objects using `Get()`, and returns them using `Release()`, reducing object creation costs. Components should be prepared for each prefab, and prefabs can be referenced from `GameObjectPoolManager`. Instances of `GameObjectPool` are created in the scene where they belong. If `_isActiveOnGet` is false, objects will not be automatically activated upon retrieval.

## Features and Operation:
- **Object Reuse**: Reduces object creation costs by reusing existing objects.
- **Prefab Management**: Manages the creation and management of instances from specified prefabs.
- **Automatic Activation**: Configures whether objects are automatically activated upon retrieval.
- **Parent Object Specification**: Allows specifying a parent object for instances at the time of creation.
- **Prewarming**: Pre-creates a specified number of objects at startup for optimization.
- **Hide Spawned Object**: Configures whether spawned objects are hidden in the editor.

## Properties
| Name | Description |
|------------------|------|
| `_prefab` | The prefab from which instances are created. |
| `_isActiveOnGet` | Whether objects are automatically activated upon retrieval. |
| `_parent` | The parent object under which instances are created. |
| `_prewarmCount` | The number of objects to pre-create at startup. |
| `_hideSpawnObject` | Whether spawned objects are hidden in the editor. |

## Methods
| Name | Function |
|------------------|------|
|  ``static GameObject`` Get( ``PooledGameObject prefab`` )  | Retrieves a reusable instance from the pool. If no instances are available, a new one is created. |
|  ``static void`` Release( ``GameObject obj`` )  | Returns a retrieved object back to the pool. |

---
## Additional Notes
- The class implements `IGameObjectPool` and `IEquatable<GameObjectPool>` interfaces, providing object pool functionality.
- The component is managed by `GameObjectPoolManager`, handling registration and unregistration of object pools within the scene.
- The prewarming feature (`_prewarmCount`) allows for performance optimization by pre-creating objects.
- `_hideSpawnObject` is used only in the editor to hide spawned objects from the hierarchy, keeping the workspace clean.