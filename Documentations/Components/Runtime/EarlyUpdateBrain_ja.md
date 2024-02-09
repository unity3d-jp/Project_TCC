# EarlyUpdateBrain

#### **Namespace**: Unity.TinyCharacterController.Core
---

## 概要:
`EarlyUpdateBrain` は `EarlyUpdateBrainBase` を継承したクラスで、フレームの更新タイミングである `Update` メソッド内でキャラクターの更新処理を行います。

## 機能と操作:
- **フレーム更新の同期**: `Update` メソッドを使用して、フレームごとの更新タイミングでキャラクターの更新処理を実行します。

## メソッド
| 名前 | 機能 |
|------------------|------|
|  ``private void`` Update()  | "フレームの更新タイミングでキャラクターの更新処理を行います。" |

---
## その他の注意事項
- `EarlyUpdateBrain` は、フレームごとの更新が必要な処理を `Update` メソッド内で実行する場合に使用されます。
- このクラスは `DefaultExecutionOrder` 属性を使用して、実行順序が `Order.EarlyUpdateBrain` に設定されています。これにより、他の `Update` メソッドよりも先に実行されることが保証されます。
- `AddComponentMenu("")` 属性により、エディタのコンポーネントメニューには表示されません。

