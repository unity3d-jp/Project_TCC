# UpdateTimingSingleton

#### **Namespace**: Unity.TinyCharacterController.Core
---

## Summary:
`UpdateTimingSingleton<TSystem>` is an abstract class designed to manage singleton instances corresponding to different update timings, namely `Update`, `FixedUpdate`, and `LateUpdate`. This class facilitates the implementation of singleton patterns that are specific to the update timing in Unity.

## Methods
| Name | Function |
|------------------|------|
|  ``public static bool`` IsCreated( ``UpdateTiming timing`` )  | "Checks if an instance for the specified update timing has already been created." |
|  ``public static TSystem`` GetInstance( ``UpdateTiming timing`` )  | "Retrieves the singleton instance for the specified update timing. If no instance exists, a new one is created." |
|  ``protected virtual void`` OnCreate( ``UpdateTiming timing`` )  | "Callback method executed when an instance is created. Intended for override in subclasses." |

---
## Additional Notes
- As a `ScriptableObject` derivative, this class operates within Unity's lifecycle management, ensuring instances are properly destroyed when the application quits, preventing leftover instances during repeated play mode iterations in the editor.
- The `IsCreated` method is used solely to check the existence of an instance and does not trigger the creation of a new instance.
- The `GetInstance` method dynamically generates instances as needed, providing a tailored singleton management system based on update timing, supporting efficient resource utilization and update cycle alignment in Unity projects.
