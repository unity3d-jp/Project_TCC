# GameControllerManager

#### **Namespace**: Unity.TinyCharacterController.Core
---

## 概要:
`GameControllerManager` は `MultiPhaseSingleton<GameControllerManager>` を継承したクラスで、任意のタイミングで処理を実行するためのコールバックを `MultiPhaseSingleton` に追加します。例えば、`IEarlyUpdate` を実装したシステムを `UpdateTiming.Update` に追加すると、`Update` の前に処理が呼び出されます。

## 機能と操作:
- **システムの登録**: 処理を実行するシステムを登録し、指定されたタイミングでコールバックを実行します。
- **早期更新と遅延更新の管理**: `IEarlyUpdate` を実装したクラスは指定されたタイミングの前に、`IPostUpdate` を実装したクラスは指定されたタイミングの後に追加されます。

## メソッド
| 名前 | 機能 |
|------------------|------|
|  ``public static void`` Register( ``ScriptableObject system, UpdateTiming timing`` )  | "処理を実行するシステムを登録します。" |
|  ``protected override void`` OnCreate( ``UpdateTiming timing`` )  | "クラスが作成されたときのコールバックです。" |
|  ``private void`` OnDestroy()  | "クラスが破棄されたときのコールバックです。" |
|  ``private void`` OnUpdate()  | "指定されたタイミングの前に呼び出されるコールバックです。" |
|  ``private void`` OnLateUpdate()  | "指定されたタイミングの後に呼び出されるコールバックです。" |

---
## その他の注意事項
- `GameControllerManager` は、Unityの `PlayerLoop` システムをカスタマイズして、特定のゲームシステムの更新処理を管理するために使用されます。
- このクラスでは、Unityのフレームワーク内で特定の更新タイミング（例：`Update`, `FixedUpdate`, `LateUpdate`）の前後にカスタム処理を挿入する機能を提供します。
- `Register` メソッドを通じてシステムを登録することで、ゲームのロジックに合わせて更新処理の順序を柔軟に制御できます。

