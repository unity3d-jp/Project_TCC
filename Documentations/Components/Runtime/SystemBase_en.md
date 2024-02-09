# SystemBase

#### **Namespace**: Unity.TinyCharacterController.Core
---

## Summary:
`SystemBase<TComponent, TSystem>` is a base class designed for batch processing, aggregating multiple components to perform collective operations. It extends `MultiPhaseSingleton<TSystem>` and facilitates component registration and deregistration based on specified `UpdateTiming`.

## Key Features and Operations:
- **Component Registration**: Registers components to the system based on the specified `UpdateTiming`.
- **Component Deregistration**: Deregisters components from the system based on the specified `UpdateTiming`.
- **Component Management**: Maintains a list of registered components and performs updates or processes as required.

## Methods
| Name | Function |
|------------------|------|
|  ``public static void`` Register( ``TComponent component, UpdateTiming timing`` )  | "Registers a component with the system for the specified update timing." |
|  ``public static void`` Unregister( ``TComponent component, UpdateTiming timing`` )  | "Deregisters a component from the system for the specified update timing." |
|  ``protected override void`` OnCreate( ``UpdateTiming timing`` )  | "A callback method invoked when an instance is created, used for initial setup." |
|  ``protected virtual void`` OnRegisterComponent( ``TComponent component, int index`` )  | "A callback method invoked when a component is registered, allowing for custom actions upon registration." |
|  ``protected virtual void`` OnUnregisterComponent( ``TComponent component, int index`` )  | "A callback method invoked when a component is deregistered, allowing for custom actions upon deregistration." |

---
## Additional Notes
- `SystemBase<TComponent, TSystem>` is an abstract class that needs to be inherited to implement specific batch processing logic.
- The class aims to centralize management and streamline update processes for various components within the game.
- The logic for component registration and deregistration leverages `ComponentBase`, which implements the `IComponentIndex` interface, ensuring that components are properly indexed and managed within the system.
