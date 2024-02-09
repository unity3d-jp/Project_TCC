# Vector3Container

#### **Namespace**: Unity.SaveData
---

## Summary:
`Vector3Container` is a concrete implementation of `DataContainerBase<Vector3>`, designed to store a single 3D vector (`Vector3`). This component enables the association, management, and easy update or reference of vector data such as positions, velocities, or directions with GameObjects.

## Key Features:
- **3D Vector Storage**: Maintains a single `Vector3` value, facilitating easy access and modifications.
- **Data Identification**: Employs a unique ID (`PropertyName`) for efficient identification of the data container.

## Usage Example:
- Attach the `Vector3Container` component to a GameObject within your Unity scene.
- Through the component's properties in the Unity Editor, set the vector value you wish to store.
- Access and modify the vector value within your game's logic using the `Vector3Container`'s `Value` property as required.

## Additional Information:
- `Vector3Container` is particularly useful for managing elements in 3D space within games, such as for physics simulations, character movements, or object placements.
- Inherits from `DataContainerBase<Vector3>`, ensuring compatibility with the `SaveDataControl` system for straightforward data persistence and retrieval.

`Vector3Container` offers a robust solution for efficiently managing 3D vector data, enhancing data management flexibility in game development. By employing this component, handling vector data within games becomes more organized and flexible, leading to cleaner and more maintainable game logic.