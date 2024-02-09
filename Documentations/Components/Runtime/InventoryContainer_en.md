# InventoryContainer

#### **Namespace**: Unity.SaveData
---

## Summary:
`InventoryContainer` is a component designed to manage items within an inventory system. Each item is represented by an instance of the `Item` class and identified by a unique ID (`PropertyName`). This component facilitates the organization, addition, removal, and modification of items within an inventory.

## Key Features:
- **Item Storage**: Manages a list of items within the inventory.
- **Item Existence Check**: Verifies the existence of an item with a specified name in the inventory.
- **Item Count Operations**: Provides methods to get, add, set, and subtract the count of items.
- **Data Identification**: Supplies an ID to uniquely identify the inventory.

## Methods
| Name | Function |
|------|----------|
| ``public IEnumerable<string>`` ItemNames | Retrieves a list of item names in the inventory. |
| ``public bool`` ExistsItem( ``string itemName`` ) | Checks if an item with the specified name exists. |
| ``public int`` GetItemCount( ``string itemName`` ) | Retrieves the count of a specified item. |
| ``public void`` AddItem( ``string itemName, int count`` ) | Adds an item to the inventory. |
| ``public void`` SetItemCount( ``string itemName, int count`` ) | Sets the count of an item. |
| ``public void`` SubtractItem( ``string itemName, int count`` ) | Reduces the count of an item. |

## Internal Class
- **Item**: Represents an individual item within the inventory, holding the item's name, ID, and count.

## Usage Example:
- Add the `InventoryContainer` component to a GameObject to set up an inventory system.
- In the Unity Editor, add items you want to include in the inventory to the `_items` list.
- Use methods like `AddItem`, `SetItemCount`, and `SubtractItem` within your game logic to manage items in the inventory.

## Additional Information:
- The `SubtractItem` method does nothing if the item does not exist. If an item's count drops to 0 or below, it is removed from the inventory.
- `InventoryContainer` is an ideal tool for managing items and tracking resources in games, simplifying the addition, deletion, and adjustment of item quantities.

`InventoryContainer` enhances inventory management in game development, enabling organized tracking and manipulation of items, thereby contributing to a more structured and manageable game environment.
