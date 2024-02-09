# IDataContainer

#### **Namespace**: Unity.SaveData.Core
---

## Summary:
`IDataContainer` is an interface designed to provide an ID for data containers, facilitating efficient identification and differentiation of data containers within a system.

## Key Features:
- **Object Identification**: Utilizes the `PropertyName` type for the unique identification of objects. The `PropertyName` structure is employed to accelerate lookup operations.

## Properties
| Name | Description |
|------|-------------|
| `Id` | The ID used for object identification, utilizing the `PropertyName` type for efficiency. |

## Usage Example:
- By implementing the `IDataContainer` interface, you can assign a unique identifier to any data container, enhancing data management within your system.
- The use of this interface enables efficient searching and access to data containers within a data management framework.

---
## Additional Information:
- `PropertyName` is a structure provided by Unity to efficiently handle property names, improving performance for searches and comparisons over using strings.
- The `IDataContainer` interface is intended for broad use within systems or frameworks that involve data storage, retrieval, and management, offering a standardized approach to data container identification.

`IDataContainer` provides a foundational element for data-driven applications and game development, simplifying and streamlining data organization and access. Its integration into data management systems enhances efficiency and consistency in handling diverse data containers.
