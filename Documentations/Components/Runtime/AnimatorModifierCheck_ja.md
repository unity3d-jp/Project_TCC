# AnimatorModifierCheck

#### **Namespace**: Unity.TinyCharacterController.Check
---

## 概要:
`AnimatorModifierCheck` は、格納された情報の変更を監視し、追加または削除された要素を抽出するコンポーネントです。このコンポーネントは、キーに変更があった場合にコールバックを発行します。

## 機能と操作:
- **キーの追加と削除**: AnimationModifierBehaviour からキーの追加や削除が行われることを想定しています。
- **キーの変更の監視**: フレーム中にキーが追加または削除されたかどうかを監視します。
- **アップデートタイミングの指定**: アニメーターが物理で動作している場合は FixedUpdate を、それ以外の場合は Update を指定します。

## プロパティ
| 名前 | 説明 |
|------------------|------|
| `_updateMode` | アップデートのタイミングを指定します。 |
| `OnChangeKey` | キーが追加または削除されたときのコールバックです。 |
| `ChangeKey` | フレーム中にいずれかのキーが追加または削除されたかどうかを示します。 |
| `CurrentKeys` | 現在アクティブなキーのリストです。 |

## メソッド
| 名前 | 機能 |
|------------------|------|
|  ``public bool`` HasKey( ``PropertyName key`` )  | "指定されたキーを保持しているかどうかを確認します。" |
|  ``public bool`` HasKey( ``string key`` )  | "指定されたキーを保持しているかどうかを確認します。" |
|  ``public bool`` IsRemoved( ``PropertyName key`` )  | "指定されたキーがこのフレーム中に削除されたかどうかを確認します。" |
|  ``public bool`` IsRemoved( ``string key`` )  | "指定されたキーがこのフレーム中に削除されたかどうかを確認します。" |
|  ``public bool`` IsAdded( ``PropertyName key`` )  | "指定されたキーがこのフレーム中に追加されたかどうかを確認します。" |
|  ``public bool`` IsAdded( ``string key`` )  | "指定されたキーがこのフレーム中に追加されたかどうかを確認します。" |


---
## その他の注意事項
- キーの追加、削除、変更を監視し、適宜コールバックを実行することで、アニメーションの動的な修正をサポートします。