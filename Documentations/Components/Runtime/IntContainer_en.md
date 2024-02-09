# IntContainer

#### **Namespace**: Unity.SaveData
---

## Summary:
`IntContainer` is a specific implementation of `DataContainerBase<int>`, designed to store a single integer (`int`) value. This component facilitates the management and association of integer values with GameObjects, allowing for easy updates and access as required.

## Key Features:
- **Integer Value Storage**: Maintains a single `int` value, enabling straightforward access and modifications.
- **Data Identification**: Employs a unique ID (`PropertyName`) for efficient identification of the data container.

## Usage Example:
- Attach the `IntContainer` component to a GameObject in your Unity scene.
- Through the component's properties in the Unity Editor, set the integer value you wish to store.
- Within your game's logic, utilize the `IntContainer`'s `Value` property to access and, if necessary, update the integer value.

## Additional Information:
- `IntContainer` is particularly beneficial for managing parameters based on a single integer value within games, such as levels, scores, points, etc.
- Inherits from `DataContainerBase<int>`, ensuring compatibility with the `SaveDataControl` system for seamless data saving and loading processes.

`IntContainer` offers a potent tool for the efficient management of integer values, enhancing data management flexibility in game development. Leveraging this component promotes a more organized and flexible approach to handling numerical data within games, contributing to more streamlined and maintainable game logic.
