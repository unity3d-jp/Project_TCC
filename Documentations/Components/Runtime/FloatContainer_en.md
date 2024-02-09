# FloatContainer

#### **Namespace**: Unity.SaveData
---

## Summary:
`FloatContainer` is a concrete implementation of `DataContainerBase<float>`, designed to store a single floating-point number (`float`). This component allows for the association, management, and easy update or reference of float values tied to GameObjects.

## Key Features:
- **Floating-Point Number Storage**: Stores a single `float` value, facilitating easy access and modification.
- **Data Identification**: Provides an ID (`PropertyName`) for uniquely identifying the data container.

## Usage Example:
- Add the `FloatContainer` component to a GameObject within your Unity scene.
- Configure the float value you wish to store using the component's properties in the Unity Editor.
- Access and modify the float value within your game's logic through the `FloatContainer`'s `Value` property, as needed.

## Additional Information:
- `FloatContainer` is particularly useful for managing specific parameters within a game, such as player health, speed, or other measurable attributes that can be represented by a floating-point number.
- Inherits from `DataContainerBase<float>`, making it compatible with the `SaveDataControl` system for straightforward data persistence and retrieval.

`FloatContainer` offers a robust solution for efficiently managing floating-point numbers, enhancing data management flexibility in game development. Utilizing this component promotes a more organized and flexible approach to handling numerical data within games, contributing to cleaner and more maintainable game logic.
