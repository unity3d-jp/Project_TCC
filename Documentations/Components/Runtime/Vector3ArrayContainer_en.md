# Vector3ArrayContainer

#### **Namespace**: Unity.SaveData
---

## Summary:
`Vector3ArrayContainer` is a specific implementation of `DataContainerBase<Vector3[]>`, designed to store arrays of 3D vectors (`Vector3[]`). This component enables the structured management and association of vector data such as positions, velocities, and directions with GameObjects, allowing for easy updates and access as required.

## Key Features:
- **Vector Array Storage**: Maintains data of type `Vector3[]`, facilitating straightforward access and modifications.
- **Data Identification**: Utilizes a unique ID (`PropertyName`) for efficient identification of the data container.

## Usage Example:
- Attach the `Vector3ArrayContainer` component to a GameObject in your Unity scene.
- Set up the array of vectors you wish to store via the component's properties in the Unity Editor.
- Access and modify the vector array within your game's logic using the `Vector3ArrayContainer`'s `Value` property as needed.

## Additional Information:
- `Vector3ArrayContainer` is particularly useful for managing elements in 3D space within games, such as for physics simulations, character movements, or object placements.
- Inherits from `DataContainerBase<Vector3[]>`, ensuring compatibility with the `SaveDataControl` system for easy data persistence and retrieval.

`Vector3ArrayContainer` offers a robust solution for efficiently managing arrays of 3D vectors, enhancing data management flexibility in game development. By employing this component, handling vector data within games becomes more organized and flexible, leading to cleaner and more maintainable game logic.