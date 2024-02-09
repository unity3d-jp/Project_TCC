# SaveDataControl

#### **Namespace**: Unity.SaveData
---

## 概要:
`SaveDataControl` は、同一 GameObject 及びその子 GameObject に設定された `IDataContainer` コンポーネントを取得し、単一の Json として保存または読み込むためのコンポーネントです。

## 機能と操作:
- **コンポーネントの収集**: GameObject とその子から `IDataContainer` コンポーネントをリストに収集します。
- **ファイル管理**: 指定されたファイル名とフォルダ名を基にデータの保存、読み込み、削除を行います。
- **データの取得**: 特定の `IDataContainer` コンポーネントを ID によって取得できます。

## プロパティ
| Name | Description |
|------|-------------|
| `_fileName` | データを保存する際のファイル名。 |

## メソッド
| Name | Function |
|------|----------|
|  ``public Component`` GetContainer( ``string id`` )  | 指定された文字列 ID によって特定の `IDataContainer` コンポーネントを取得します。 |
|  ``public Component`` GetContainer( ``PropertyName id`` )  | 指定された `PropertyName` ID によって特定の `IDataContainer` コンポーネントを取得します。 |
|  ``public bool`` Exists( ``string folder`` )  | 指定されたフォルダに保存されたデータが存在するかどうかを確認します。 |
|  ``public void`` Remove( ``string folder`` )  | 指定されたフォルダから保存されたデータを削除します。 |
|  ``public void`` Save( ``string folder`` )  | 指定されたフォルダにすべての `IDataContainer` コンポーネントをファイルとして保存します。 |
|  ``public void`` Load( ``string folder`` )  | 指定されたフォルダからファイルを読み込み、`IDataContainer` コンポーネントのデータを更新します。 |

## 使用例:
- `SaveDataControl` を `IDataContainer` コンポーネントを含む GameObject にアタッチします。
- `Save` メソッドを使用して、すべての `IDataContainer` コンポーネントのデータを単一のファイルに保存します。
- `Load` メソッドを使用して、保存されたファイルから `IDataContainer` コンポーネントのデータを復元します。

## 追加情報:
- 内部で使用される `SaveData` 構造体は、`IDataContainer` コンポーネントごとに ID と JSON 文字列の配列を含む保存データのフォーマットを定義しています。
- `FileName` プロパティは `_fileName` が指定されていない場合に GameObject の名前をデフォルトのファイル名として使用します。
- このコンポーネントは、複雑なデータ構造の永続化を容易にし、セッション間でのゲーム状態や設定の管理を簡素化します。

`SaveDataControl` は、Unity 内でのゲームデータ管理を強化し、複数のデータコンテナのシリアライズとデシリアライズプロセスを簡素化し、ゲーム開発プロジェクトの全体的なデータ管理ワークフローを向上させるソリューションを提供します。

