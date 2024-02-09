# PooledGameObject

#### **Namespace**: Unity.TinyCharacterController.Utility
---

## Summary:
`PooledGameObject` is a component for managing objects pooled by `GameObjectPool`. It can be obtained through `IGameObjectPool.Get()` and released with `IGameObjectPool.Release(IPooledObject)`. 

## Features and Operation:
- **Lifetime Feature Utilization**: Configures the object to be automatically released after a certain period.
- **Release Callback**: Invokes a callback event when the object is released.

## Properties
| Name | Description |
|------|-------------|
| `_isUseLifetime` | Whether to use the lifetime feature or not. |
| `_lifeTime` | The duration the object remains valid. |
| `OnReleaseByLifeTime` | Callback invoked when the object is released due to its lifetime. |
| `IsUsed` | Whether the `PooledGameObject` is in use. |
| `ReleaseTime` | The time when the `PooledGameObject` will be released. |
| `IsPlaying` | Whether the `PooledGameObject` is currently active. |

## Methods
- There are no public methods.

---
## Additional Notes
- `PooledGameObject` implements the `IPooledObject` interface, providing the basic interface for objects managed within the pool system.
- The lifetime feature allows for efficient management of resources by automatically returning objects to the pool after a set duration.
- When objects are released, the `OnReleaseByLifeTime` event is triggered, allowing for additional cleanup or notification processes to be performed.

