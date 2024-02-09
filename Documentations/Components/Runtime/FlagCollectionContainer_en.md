# FlagCollectionContainer

#### **Namespace**: Unity.SaveData
---

## Summary:
`FlagCollectionContainer` is a component designed to manage a collection of flags, where each flag represents a boolean value. Each flag is represented by a `FlagInfo` object, identified by a unique ID (`PropertyName`).

## Key Features:
- **Data Identification**: Assigns a unique ID to the flag collection for identification purposes.
- **Flag Value Retrieval and Assignment**: Retrieves or sets boolean values corresponding to specified flag names.
- **Flag Information Management**: Maintains a list of flags, each represented by the `FlagInfo` class.

## Properties
| Name | Description |
|------|-------------|
| `_id` | The ID of the data container. |
| `_values` | The list of flags. |

## Methods
| Name | Function |
|------|----------|
|  ``public bool`` GetValue( ``string flagName`` )  | Retrieves the boolean value associated with the specified flag name. |
|  ``public void`` SetValue( ``string flagName, bool newValue`` )  | Sets a new boolean value for the specified flag name. |

## Internal Class
- **FlagInfo**: An internal class representing a flag. It holds the flag's ID and its value (boolean).

## Usage Example:
- Add the `FlagCollectionContainer` component to a GameObject.
- In the Unity Editor, configure the list of flags, assigning a unique name (ID) and default value to each flag.
- Use the `GetValue` and `SetValue` methods within your game's logic to retrieve or update flag values.

## Additional Information:
- If a flag is not found, the `GetValue` and `SetValue` methods throw an exception, preventing access to non-existent flags.
- `FlagCollectionContainer` is particularly useful for managing game states or settings, especially when centralizing control over numerous on/off configurations.

`FlagCollectionContainer` provides a robust tool for efficiently managing collections of flags, enhancing data management flexibility in game development. Its structured approach to flag management simplifies the handling of boolean states, contributing to more organized and maintainable game logic.
