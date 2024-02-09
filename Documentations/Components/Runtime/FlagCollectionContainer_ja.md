# FlagCollectionContainer

#### **Namespace**: Unity.SaveData
---

## 概要:
`FlagCollectionContainer` は、フラグ（ブール値）のコレクションを管理するためのコンポーネントです。各フラグは `FlagInfo` オブジェクトによって表され、一意の ID（`PropertyName`）によって識別されます。

## 主な特徴:
- **データ識別**: フラグコレクションに一意の ID を割り当てます。
- **フラグ値の取得と設定**: 指定されたフラグ名に対応するブール値を取得または設定します。
- **フラグ情報の管理**: フラグのリストを保持し、各フラグは `FlagInfo` クラスで表されます。

## プロパティ
| Name | Description |
|------|-------------|
| `_id` | データコンテナの ID。 |
| `_values` | フラグのリスト。 |

## メソッド
| Name | Function |
|------|----------|
|  ``public bool`` GetValue( ``string flagName`` )  | 指定されたフラグ名に対応するブール値を取得します。 |
|  ``public void`` SetValue( ``string flagName, bool newValue`` )  | 指定されたフラグ名に新しいブール値を設定します。 |

## 内部クラス
- **FlagInfo**: フラグを表す内部クラス。フラグの ID と値（ブール値）を保持します。

## 使用例:
- `FlagCollectionContainer` コンポーネントをゲームオブジェクトに追加します。
- Unity エディタで、フラグのリストを設定し、各フラグに一意の名前（ID）とデフォルト値を割り当てます。
- ゲームのロジック内で `GetValue` と `SetValue` メソッドを使用して、フラグの値を取得または更新します。

## 追加情報:
- フラグが見つからない場合、`GetValue` と `SetValue` メソッドは例外をスローします。これにより、存在しないフラグへのアクセスを防ぎます。
- `FlagCollectionContainer` は、ゲームの状態や設定を管理する際に便利です。特に、多数のオン/オフ設定を一元管理する場合に有効です。

`FlagCollectionContainer` は、フラグのコレクションを効率的に管理し、ゲーム開発におけるデータ管理の柔軟性を向上させるための強力なツールです。
