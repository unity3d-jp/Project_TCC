# EarlyUpdateBrainBase

#### **Namespace**: Unity.TinyCharacterController.Core
---

## Summary:
`EarlyUpdateBrainBase` is an abstract base class designed to manage the early update process of characters. It inherits from `MonoBehaviour` and maintains a list of components that implement the `IEarlyUpdateComponent` interface.

## Features and Operation:
- **Collection and Sorting of Early Update Components**: Within the `Awake` method, it collects components that implement the `IEarlyUpdateComponent` interface attached to the same GameObject and sorts them based on their execution order (`Order`).
- **Execution of Update Process**: Through the `OnUpdate` method, it sequentially calls the `OnUpdate` method of all registered early update components, performing per-frame update processes.

## Methods
| Name | Function |
|------------------|------|
|  ``private void`` Awake()  | "Collects and sorts early update components during component initialization." |
|  ``protected void`` OnUpdate( ``float deltaTime`` )  | "Executes the update process for early update components." |

---
## Additional Notes
- `EarlyUpdateBrainBase` is an abstract class that needs to be inherited to implement specific early update processes.
- Utilizing this class allows for managing the execution order among components with different update processes, facilitating efficient early-stage updates per frame.
- The `Awake` method is called only once when the object is loaded, meaning the collection and sorting of components are performed only once at the start of the game.