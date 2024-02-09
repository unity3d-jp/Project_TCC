# InventoryContainer

#### **Namespace**: Unity.SaveData
---

## 概要:
`InventoryContainer` は、インベントリシステム内のアイテムを管理するためのコンポーネントです。各アイテムは `Item` クラスのインスタンスによって表され、一意の ID (`PropertyName`) で識別されます。

## 主な特徴:
- **アイテムの保持**: インベントリ内のアイテムをリストで管理します。
- **アイテムの存在確認**: 指定されたアイテム名のアイテムがインベントリ内に存在するかを確認します。
- **アイテム数の取得と設定**: アイテムの数を取得、追加、設定、減少させるメソッドを提供します。
- **データ識別**: インベントリを一意に識別するための ID を提供します。

## メソッド
| Name | Function |
|------|----------|
| ``public IEnumerable<string>`` ItemNames | インベントリ内のアイテム名のリストを取得します。 |
| ``public bool`` ExistsItem( ``string itemName`` ) | 指定されたアイテムが存在するかを確認します。 |
| ``public int`` GetItemCount( ``string itemName`` ) | 指定されたアイテムの数を取得します。 |
| ``public void`` AddItem( ``string itemName, int count`` ) | アイテムをインベントリに追加します。 |
| ``public void`` SetItemCount( ``string itemName, int count`` ) | アイテムの数を設定します。 |
| ``public void`` SubtractItem( ``string itemName, int count`` ) | アイテムの数を減少させます。 |

## 内部クラス
- **Item**: インベントリ内の個々のアイテムを表すクラス。アイテム名、ID、およびカウントを保持します。

## 使用例:
- `InventoryContainer` コンポーネントをゲームオブジェクトに追加し、インベントリシステムを構築します。
- Unity エディタで、インベントリに含めたいアイテムを `_items` リストに追加します。
- ゲームのロジック内で、`AddItem`、`SetItemCount`、`SubtractItem` メソッドを使用して、インベントリ内のアイテムを管理します。

## 追加情報:
- アイテムが存在しない場合、`SubtractItem` メソッドは何も行いません。また、アイテムの数が 0 以下になった場合、そのアイテムはインベントリから削除されます。
- `InventoryContainer` は、ゲーム内でのアイテム管理やリソース追跡に最適なツールです。アイテムの追加、削除、数量の調整を簡単に行えます。

`InventoryContainer` により、ゲーム開発におけるインベントリ管理が容易になり、より整理された形でアイテムの追跡が可能になります。