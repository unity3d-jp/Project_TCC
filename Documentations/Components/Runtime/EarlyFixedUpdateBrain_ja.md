# EarlyFixedUpdateBrain

#### **Namespace**: Unity.TinyCharacterController.Core
---

## 概要:
`EarlyFixedUpdateBrain` は `EarlyUpdateBrainBase` を継承したクラスで、物理更新のタイミングである `FixedUpdate` メソッド内でキャラクターの更新処理を行います。

## 機能と操作:
- **物理更新の同期**: `FixedUpdate` メソッドを使用して、物理計算と同期されたタイミングでキャラクターの更新処理を実行します。

## メソッド
| 名前 | 機能 |
|------------------|------|
|  ``private void`` FixedUpdate()  | "物理更新のタイミングでキャラクターの更新処理を行います。" |

---
## その他の注意事項
- `EarlyFixedUpdateBrain` は、Unityの物理システムと同期するために特定の更新処理を `FixedUpdate` 内で実行する必要がある場合に使用されます。
- このクラスは `DefaultExecutionOrder` 属性を使用して、実行順序が `Order.EarlyUpdateBrain` に設定されています。これにより、他の `FixedUpdate` メソッドよりも先に実行されることが保証されます。
- `AddComponentMenu("")` 属性により、エディタのコンポーネントメニューには表示されません。

続けて英語の解析結果をご覧になりますか？