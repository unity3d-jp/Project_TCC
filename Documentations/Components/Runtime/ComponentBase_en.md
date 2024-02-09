# ComponentBase

#### **Namespace**: Unity.TinyCharacterController.Core
---

## Summary:
`ComponentBase` is the base class for components that perform batch processing. This class is intended to be registered with a class that inherits from `SystemBase{TComponent, TSystem}` for use.

## Features and Operation:
- **Component Registration**: Registers the component in the system for batch processing.
- **Index Management**: Maintains the index of the component during batch processing and manages its registration status.

## Properties
| Name | Description |
|------------------|------|
| `Index` | Holds the index of the component during batch processing. |
| `IsRegistered` | Indicates whether the component is registered or not. |

---
## Additional Notes
- `ComponentBase` is an abstract class designed to be inherited by components with specific functionalities.
- Utilizing this class requires implementing the `IComponentIndex` interface to manage the component's index and registration status.
- The `Index` property is used to uniquely identify the component within the batch processing system, and it is set to `-1` by default, indicating that the component has not been registered yet.

