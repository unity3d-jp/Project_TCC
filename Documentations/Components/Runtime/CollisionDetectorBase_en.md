# CollisionDetectorBase

#### **Namespace**: Unity.TinyCharacterController.Components.Utility
---

## Summary:
`CollisionDetectorBase` is a component that provides basic collision detection functionality. This component detects collisions with objects that have specific layers or tags and can trigger events when a collision occurs. It has an `Owner` property set to a `CharacterSettings` component, ensuring it does not collide with the collider held by the owner.

## Features and Operation:
- **Collision Detection Timing and Layers**: You can set the timing for collision detection execution and the layers of the objects to be detected.
- **Collision Event Callback**: You can set an event to be triggered when a collision is detected.
- **Caching of Collision Objects**: It maintains a list of objects that collided in the current frame and the objects that collided while the component is active.
- **Clearing Collision Detection Cache**: Provides the functionality to clear the cache of collision detection.

## Properties
| Name | Description |
|------------------|------|
| `Timing` | The execution timing of collision detection. |
| `Owner` | The owner of the collision detection. The owner's collider will not collide. |
| `CacheTargetType` | The type of target to cache during collision detection. |
| `HitLayer` | The layer for performing collision detection. |
| `HitTags` | The tags of the objects to detect collisions with. |
| `OnHitObjects` | The event that is fired when a collision is detected. |

## Methods
| Name | Function |
|------------------|------|
|  ``public void`` ClearHitObjectsCache()  | Clears the cache of collision detection. |
|  ``public Collider`` GetColliderForGameObject( ``GameObject obj`` )  | Retrieves the Collider associated with the specified GameObject. Returns null if it does not exist. |

---
## Additional Notes
- The `HitColliders` and `HitObjects` lists track the objects with which the component has detected collisions. These lists are updated at the start of a new frame or when the component becomes active.
- The `OnHitObjects` event is a `UnityEvent` that can be used as a callback when a collision is detected.
- This component serves as a foundation for implementing custom collision detection logic. You can extend this base class in derived classes to implement specific collision detection behaviors as needed.

