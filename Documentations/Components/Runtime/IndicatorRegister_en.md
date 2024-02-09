# IndicatorRegister

#### **Namespace**: Unity.TinyCharacterController.Utility
---

## Summary:
`IndicatorRegister` is a component designed to easily add indicators to characters. Upon activation, it automatically generates and registers the `_ui` object. The objects are created using the `GameObjectPool` functionality, meaning the UI to be generated should contain a `PooledGameObject` component and be registered with the `GameObjectPool`.

## Features and Operation:
- **UI Generation and Registration**: Automatically generates and adds the specified UI to the character.
- **Automatic Indicator Tracking**: If the generated UI has an `Indicator` component, it automatically tracks the target character.
- **UI Pin Configuration**: If the generated UI has a `UiPin` component, it sets up its world position.

## Properties
| Name | Description |
|------|-------------|
| `_ui` | The UI to be generated. |
| `Instance` | The instance of the generated UI. |
| `HasInstance` | Whether an instance of the UI has been generated. |

## Methods
- There are no public methods.

---
## Additional Notes
- This component is useful for adding UI elements with `Indicator` or `UiPin` components to characters, facilitating the addition of various UI elements such as indicators that automatically track target characters or UI pins fixed to specific locations.
- Managing UI through `GameObjectPool` contributes to performance optimization by reusing UI elements efficiently.
