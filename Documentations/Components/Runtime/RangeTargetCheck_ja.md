# RangeTargetCheck

#### **Namespace**: Unity.TinyCharacterController.Check
---

## 概要:
`RangeTargetCheck` は、視界と障害物を考慮して特定のタグを持つオブジェクトを範囲内で検出するコンポーネントです。このコンポーネントは、毎フレーム実行され、内容に変更がある場合はコールバックを呼び出します。主に一定範囲内のオブジェクトを検索するために使用されます。

## 機能と操作:
- **センサーの中心点**: センサーの位置を指定します。
- **検出可能なレイヤー**: センサーが検出できるレイヤーを指定します。
- **透明レイヤーの無視**: 環境レイヤー内で透明なオブジェクト（例えば透明な窓）を無視するために使用されます。
- **ターゲットの検索設定**: ターゲットのタグ、検索範囲、視界の考慮などを設定できます。
- **ターゲットの取得**: 特定のタグを持つオブジェクトのリストを取得します。
- **ターゲットの空判定**: 指定したタグを持つオブジェクトが存在しないかどうかを確認します。
- **最も近いターゲットの取得**: 指定したタグを持つ最も近いオブジェクトを取得します。
- **ターゲットの変更検出**: 指定したタグの範囲内で新しく追加または削除されたターゲットを取得します。

## プロパティ
| Name | Description |
|------------------|------|
| `_sensorOffset` | センサーの中心点のオフセットを指定します。 |
| `_hitLayer` | センサーが検出可能なレイヤーを指定します。 |
| `_transparentLayer` | 透明なオブジェクトを無視するためのレイヤーを指定します。 |
| `_searchData` | ターゲットの検索に関する設定データです。 |

## メソッド
| Name | Function |
|------------------|------|
| ``public List<Transform>`` GetTargets(``string tagName``) | 指定したタグを持つオブジェクトのリストを返します。 |
| ``public List<Transform>`` GetTargets(``int index``) | 指定したインデックスのタグを持つオブジェクトのリストを返します。 |
| ``public bool`` IsEmpty(``int tagIndex``) | 指定したタグのオブジェクトが存在しないかどうかを確認します。 |
| ``public bool`` IsEmpty(``string tagName``) | 指定したタグ名のオブジェクトが存在しないかどうかを確認します。 |
| ``public bool`` TryGetClosestTarget(``string tagName, out Transform target``) | 指定したタグを持つ最も近いオブジェクトを取得します。 |
| ``public int`` GetTagIndex(``string tagName``) | 指定したタグ名のインデックスを取得します。 |
| ``public bool`` TryGetClosestTarget(``string tagName, out Transform target, out Transform preTarget``) | 指定したタグを持つ最も近いオブジェクトとその前のオブジェクトを取得します。 |
| ``public bool`` ChangedValues(``string tagName, out List<Transform> added, out List<Transform> removed``) | 指定したタグの範囲内で新しく追加または削除されたオブジェクトを取得します。 |

---
## その他の注意事項
このコンポーネントは、Unityのゲーム開発において、特定のタグを持つオブジェクトを効率的に検出し、管理するための高度な機能を提供します。特に、キャラクターの近くにあるオブジェクトを検出し、それらの状態変化に応じた処理を実行する際に有用です。また、レイヤーや視界の制御を通じて、より精密なターゲット検出が可能です。このような機能は、敵の探知、アイテムの検出、環境の認識など、さまざまなシナリオで応用できるでしょう。
