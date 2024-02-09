# FloatArrayContainer

#### **Namespace**: Unity.SaveData
---

## Summary:
`FloatArrayContainer` is a specific implementation of `DataContainerBase<float[]>`, tailored for storing arrays of floating-point numbers (`float[]`). This component simplifies the management, storage, and access of numerical data arrays tied to GameObjects.

## Key Features:
- **Floating-Point Number Array Storage**: Holds data of type `float[]`, allowing for easy access and updates as needed.
- **Data Identification**: Uses a unique ID (`PropertyName`) to identify the data container for efficient data management.

## Usage Example:
- Add the `FloatArrayContainer` component to a GameObject within your Unity scene.
- Use the component's properties to set the array of floating-point numbers you wish to store.
- Access and modify the array within your game's logic through the `FloatArrayContainer`'s `Value` property, reading or updating values as necessary.

## Additional Information:
- `FloatArrayContainer` is particularly useful for managing sequential numerical data in games, such as scores, distances, times, etc.
- Inherits from `DataContainerBase<float[]>`, making it compatible with the `SaveDataControl` system for straightforward data persistence and retrieval.

`FloatArrayContainer` offers a robust solution for efficiently managing arrays of floating-point numbers, enhancing data management flexibility in game development. Utilizing this component facilitates a more organized and flexible approach to handling numerical data within games, contributing to cleaner and more maintainable game logic.
