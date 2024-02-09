# GameObjectFolder

#### **Namespace**: Unity.Utility
---

## Summary:
`GameObjectFolder` is a component designed for organizing the hierarchical structure of GameObjects in the Unity Editor. It treats GameObjects as "folders" within the hierarchy, facilitating organization and adding comments.

## Features and Operation:
- **Comment Addition**: Allows adding comments to GameObjects in the editor.
- **Hierarchy Color Customization**: Enables customization of the display color in the hierarchy view.
- **Child Object Visibility Control**: Allows toggling the visibility of child objects within the GameObject folder.

## Properties
| Name | Description |
|------|-------------|
| `_comment` | Comments associated with the GameObject. |
| `_menuColor` | The display color in the hierarchy view. |
| `_isVisible` | The visibility state of child objects. |
| `ChildObjects` | A list of child objects. |

## Usage Example
- Add the `GameObjectFolder` component to a GameObject to use it as a "folder" in the hierarchy.
- Use the `_comment` property to add notes or descriptions related to the GameObject.
- Change the display color in the hierarchy view using the `_menuColor` property.
- Toggle the visibility of child objects in this folder using the `_isVisible` property.

---
## Additional Notes
- This component is an extension of Unity Editor functionality and is primarily intended for use within the editor. It does not affect runtime behavior.
- The `Reset` method is automatically called when the component is added to a GameObject, performing initial setup.
- The `ChildObjects` list is intended for tracking child objects included in this folder, expected to be dynamically managed through editor scripts.

The `GameObjectFolder` component is a useful tool for managing large scenes or projects with complex hierarchical structures, helping in organizing and visualizing the hierarchy.
