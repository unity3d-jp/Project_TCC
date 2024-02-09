# FileUtility

#### **Namespace**: Unity.SaveData.Core
---

## Summary:
`FileUtility` is a utility class designed for saving and loading data in a platform-compatible manner. It handles data in JSON format and persists data using either the local file system or `PlayerPrefs`, depending on the platform.

## Features and Operation:
- **Path Generation**: Generates a file path suitable for the current platform based on the specified file name and path.
- **Data Deletion**: Deletes a data file with the specified path and name.
- **Data Saving**: Saves the given value at the specified path and file name in JSON format.
- **Data Loading**: Loads and deserializes data from the specified path and file name into the specified type. Returns `true` if the data exists.

## Methods
| Name | Function |
|------|----------|
| `Save<TValue>` | Saves the given value in JSON format. |
| `Load<TValue>` | Loads data in JSON format and deserializes it into the specified type. |
| `Remove` | Deletes the data file with the specified path and name. |
| `IsExists` | Checks if data with the specified path and name exists. |

## Usage Example
- Use the `Save` method to store data like game settings or player progress.
- Use the `Load` method to load settings or progress at the start of the game or specific points.
- Use the `Remove` method to delete data that is no longer needed.
- Use the `IsExists` method to check for the existence of data.

---
## Additional Notes
- The directives `UNITY_WEBGL` and `UNITY_EDITOR` are used to differentiate the saving method based on the platform. For WebGL, `PlayerPrefs` is used, while other platforms use the local file system.
- The `DataStore<T>` structure is utilized to address the issue where `JsonUtility` cannot directly serialize lists. It allows for the serialization/deserialization of data of any type in JSON format.

`FileUtility` serves as a convenient tool for easily and efficiently saving and loading data. It is particularly useful for managing information that needs to be persisted, such as game settings or user progress, across different platforms.

