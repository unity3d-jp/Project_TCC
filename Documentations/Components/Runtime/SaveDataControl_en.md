# SaveDataControl

#### **Namespace**: Unity.SaveData
---

## Summary:
`SaveDataControl` is a Unity component for aggregating `IDataContainer` components on the same GameObject and its children, enabling the saving and loading of their data as a single JSON file.

## Features and Operation:
- **Component Aggregation**: Collects `IDataContainer` components from the GameObject and its children into a list.
- **File Operations**: Facilitates saving, loading, and deleting data based on the specified file and folder names.
- **Container Retrieval**: Allows for the retrieval of specific `IDataContainer` components by their ID.

## Properties
| Name | Description |
|------|-------------|
| `_fileName` | The file name used for saving data. |

## Methods
| Name | Function |
|------|----------|
|  ``public Component`` GetContainer( ``string id`` )  | Retrieves a specific `IDataContainer` component by its string ID. |
|  ``public Component`` GetContainer( ``PropertyName id`` )  | Retrieves a specific `IDataContainer` component by its `PropertyName` ID. |
|  ``public bool`` Exists( ``string folder`` )  | Checks if data exists in the specified folder. |
|  ``public void`` Remove( ``string folder`` )  | Deletes data from the specified folder. |
|  ``public void`` Save( ``string folder`` )  | Saves all `IDataContainer` components to a file in the specified folder. |
|  ``public void`` Load( ``string folder`` )  | Loads data from a file in the specified folder and updates the `IDataContainer` components. |

## Usage Example:
- Attach the `SaveDataControl` component to a GameObject that contains one or more `IDataContainer` components.
- Use the `Save` method to persist all `IDataContainer` component data into a single file.
- Use the `Load` method to restore `IDataContainer` component data from the saved file.

## Additional Notes:
- The `SaveData` struct, used internally, formats the data for saving, containing arrays of `PropertyName` IDs and JSON strings for each `IDataContainer` component.
- The `FileName` property ensures that the file name is never empty, defaulting to the GameObject's name if `_fileName` is not specified.
- This component simplifies the process of persisting complex data structures, making it easier to manage game states or settings across sessions.

`SaveDataControl` enhances Unity's game data management capabilities by simplifying the serialization and deserialization processes for multiple data containers, thereby improving the data management workflow in game development projects.

