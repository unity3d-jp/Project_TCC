# SystemBase

#### **Namespace**: Unity.TinyCharacterController.Core
---

## 概要:
`SystemBase<TComponent, TSystem>` は、複数のコンポーネントを集めて一括処理を行うための基底クラスです。これは `MultiPhaseSingleton<TSystem>` を継承し、指定された `UpdateTiming` に基づいてコンポーネントの登録・登録解除を行います。

## 主要機能と操作:
- **コンポーネントの登録**: 指定された `UpdateTiming` に基づいて、コンポーネントをシステムに登録します。
- **コンポーネントの登録解除**: 指定された `UpdateTiming` に基づいて、コンポーネントをシステムから登録解除します。
- **コンポーネントの管理**: 登録されたコンポーネントのリストを保持し、必要に応じて更新や処理を行います。

## メソッド
| 名前 | 機能 |
|------------------|------|
|  ``public static void`` Register( ``TComponent component, UpdateTiming timing`` )  | "指定された更新タイミングでコンポーネントをシステムに登録します。" |
|  ``public static void`` Unregister( ``TComponent component, UpdateTiming timing`` )  | "指定された更新タイミングでコンポーネントをシステムから登録解除します。" |
|  ``protected override void`` OnCreate( ``UpdateTiming timing`` )  | "インスタンスが作成された際に呼び出されるコールバックメソッドです。" |
|  ``protected virtual void`` OnRegisterComponent( ``TComponent component, int index`` )  | "コンポーネントが登録された際に呼び出されるコールバックメソッドです。" |
|  ``protected virtual void`` OnUnregisterComponent( ``TComponent component, int index`` )  | "コンポーネントが登録解除された際に呼び出されるコールバックメソッドです。" |

---
## その他の注意事項
- `SystemBase<TComponent, TSystem>` は抽象クラスであり、具体的な一括処理ロジックを実装するために継承される必要があります。
- このクラスは、ゲーム内でのさまざまなコンポーネントの一元管理と更新処理の効率化を目的としています。
- コンポーネントの登録と登録解除のロジックには、`IComponentIndex` インターフェースを実装した `ComponentBase` が使用されます。

