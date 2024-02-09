# UpdateTimingSingleton

#### **Namespace**: Unity.TinyCharacterController.Core
---

## 概要:
`UpdateTimingSingleton<TSystem>` は、`Update`, `FixedUpdate`, `LateUpdate` の各更新タイミングに対応するシングルトンインスタンスを管理する抽象クラスです。このクラスを継承することで、更新タイミングごとに異なるシングルトンの実装を簡単に行うことができます。

## メソッド
| 名前 | 機能 |
|------------------|------|
|  ``public static bool`` IsCreated( ``UpdateTiming timing`` )  | "指定された更新タイミングのインスタンスが作成されているかどうかをチェックします。" |
|  ``public static TSystem`` GetInstance( ``UpdateTiming timing`` )  | "指定された更新タイミングのシングルトンインスタンスを取得します。インスタンスが存在しない場合は新しく作成されます。" |
|  ``protected virtual void`` OnCreate( ``UpdateTiming timing`` )  | "インスタンスが作成されたときに実行されるコールバックです。サブクラスでのオーバーライドを想定しています。" |

---
## その他の注意事項
- このクラスは `ScriptableObject` を継承しています。Unityのライフサイクル管理の下で動作し、アプリケーションの終了時に自動的にインスタンスが破棄されるように設計されています。
- `IsCreated` メソッドはインスタンスの存在を確認するためだけに使用され、新しいインスタンスの作成は行いません。
- `GetInstance` メソッドは、必要に応じてインスタンスを動的に生成し、更新タイミングに応じたシングルトン管理を実現します。
