# DataContainerBase<T>

#### **Namespace**: Unity.SaveData.Core
---

## Summary:
`DataContainerBase<T>` serves as an abstract base component for storing data of any type `T`. It is responsible for saving and identifying data managed by `SaveDataControl`.

## Features and Operation:
- **Data Identification**: Holds an ID to uniquely identify the data.
- **Data Storage**: Stores and provides access to a value of the specific data type `T`.
- **Automatic Registration**: Automatically adds a `SaveDataControl` component if the component does not exist in the parent.

## Properties
| Name | Description |
|------|-------------|
| `_id` | The ID used for data identification. |
| `_value` | The value of the stored data. |
| `Value` | A public property for `_value`, allowing for getting and setting the data. |

## Usage Example
- Create a custom component that inherits from `DataContainerBase<T>` and specify a concrete data type `T`.
- Add this component to a GameObject in the Unity Editor.
- Set the `_id` field to uniquely identify the data, and specify the value of the data to be stored in the `_value` field.
- Use the `Value` property within scripts to access or modify the stored data.

---
## Additional Notes
- This component is an abstract class and cannot be instantiated directly. To use it, you must create a subclass with a concrete data type `T`.
- The `Reset` method is automatically called when the component is added to a GameObject, attempting to auto-register `SaveDataControl`. This facilitates the process of saving and loading data.

`DataContainerBase<T>` provides a flexible foundation for efficiently managing and storing data in game development. The customizable data type and automatic management of `SaveDataControl` simplify data handling, making it a valuable tool for developers.

