# IntArrayContainer

#### **Namespace**: Unity.SaveData
---

## Summary:
`IntArrayContainer` is a concrete implementation of `DataContainerBase<int[]>`, specifically designed to store arrays of integers (`int[]`). This component enables the structured management and association of integer arrays with GameObjects, facilitating easy updates and access when required.

## Key Features:
- **Integer Array Storage**: Maintains data of type `int[]`, allowing for straightforward access and updates.
- **Data Identification**: Utilizes a unique ID (`PropertyName`) to identify the data container efficiently.

## Usage Example:
- Attach the `IntArrayContainer` component to a GameObject within your Unity scene.
- Set up the integer array you wish to store via the component's properties in the Unity Editor.
- Access and modify the integer array within your game's logic using the `IntArrayContainer`'s `Value` property, adjusting values as necessary.

## Additional Information:
- `IntArrayContainer` is particularly useful for managing sequential integer values in games, such as scores, item counts in an inventory, or achievement points.
- Inherits from `DataContainerBase<int[]>`, making it seamlessly integrate with the `SaveDataControl` system for easy data persistence and retrieval.

`IntArrayContainer` offers a robust tool for efficiently managing integer arrays, enhancing data management flexibility in game development. By employing this component, the handling of numerical data within games becomes more organized and flexible, leading to cleaner and more maintainable game logic.
