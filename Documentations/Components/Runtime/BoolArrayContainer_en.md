# BoolArrayContainer

#### **Namespace**: Unity.SaveData
---

## Summary:
`BoolArrayContainer` is a concrete implementation of `DataContainerBase<bool[]>`, designed to store and manage an array of boolean values as a single data container. This component enables the organization and persistence of multiple boolean values within your Unity projects.

## Key Features:
- **Boolean Array Storage**: Stores an array of boolean (`bool`) values, allowing for easy access and modification as needed.
- **Data Identification**: Utilizes a unique ID (`PropertyName`) to identify the data container, facilitating efficient data management and retrieval.

## Usage Example:
- Add the `BoolArrayContainer` component to a GameObject within your Unity scene.
- Configure the boolean array data you wish to store using the Unity Editor by setting up the values in the component.
- Access and manipulate the boolean array within your game logic through the `Value` property, reading or writing values as required.

---
## Additional Information:
- `BoolArrayContainer` is particularly useful for managing collections of boolean values, such as game settings, state flags, or feature toggles, in a consolidated manner.
- By inheriting from `DataContainerBase<bool[]>`, it seamlessly integrates with the `SaveDataControl` system for straightforward data saving and loading operations.

`BoolArrayContainer` offers a simple yet effective solution for efficiently handling arrays of boolean values, enhancing data management flexibility in game development scenarios. Its integration into data storage systems simplifies the persistence of boolean state information, contributing to more organized and maintainable game codebases.