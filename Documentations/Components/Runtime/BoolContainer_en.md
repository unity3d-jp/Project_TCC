# BoolContainer

#### **Namespace**: Unity.SaveData
---

## Summary:
`BoolContainer` is a specific implementation of `DataContainerBase<bool>`, serving as a data container for a single boolean value. This component allows for the association, management, and easy update or reference of boolean values tied to GameObjects.

## Key Features:
- **Boolean Value Storage**: Stores a single boolean (`bool`) value, facilitating easy access and modification.
- **Data Identification**: Provides an ID (`PropertyName`) for uniquely identifying the data container.

## Usage Example:
- Add the `BoolContainer` component to a GameObject in the Unity Editor.
- Set the desired boolean value via the component's properties.
- Within your game logic, use the `Value` property of this component to retrieve or set the boolean value.

## Additional Information:
- `BoolContainer` is particularly useful for implementing features based on a single boolean value, such as toggling game states or settings on/off.
- Inherits from `DataContainerBase<bool>`, making it compatible with the `SaveDataControl` system for easy data saving and loading.

`BoolContainer` provides a simple yet effective tool for the efficient management and persistence of boolean values in game development. Utilizing this component facilitates a more flexible and organized approach to using boolean values within games, enhancing the structure and maintainability of game logic.