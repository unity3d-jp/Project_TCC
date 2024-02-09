了解しました。`OnChangeAimedGameObject`プロパティを追加して、ステップ1の分析を再実行します。

# AimCheck

#### **Namespace**: Unity.TinyCharacterController.Check
---

## 概要:
`AimCheck`は、画面の中央に位置するオブジェクトを取得するコンポーネントです。このコンポーネントは、有効な間は毎フレーム継続的にチェックを行い、ターゲットオブジェクトやターゲットまでの距離などの情報を取得します。画面上でオブジェクトが見えているがキャラクターの視点からは見えない場合は、キャラクターの視点を優先します。

## 機能と操作:
- **視点からの照準**: キャラクターの視点からスクリーンの中央にあるオブジェクトを検出する。
- **照準のカスタマイズ**: 照準の基点や照準対象のスクリーンポイントをカスタマイズする。
- **追加のヒットマスク**: `CharacterSettings`の地形検出に加えて、追加の接触可能レイヤーを含む。
- **最大範囲**: `AimCheck`によって検出できる最大距離を設定する。
- **Unityイベントの呼び出し**: 視点前のオブジェクトが変わった際にUnityイベントを呼び出す。

## プロパティ
| Name | Description |
|------------------|------|
| `IsUseAimParallax` | キャラクターの視点を含めた視認性チェックを行うかどうか。 |
| `_origin` | `AimCheck`によるレイキャストの起点。 |
| `_aimTargetViewportPoint` | 画面上の照準対象ポイント。 |
| `_additionalHitMask` | `CharacterSettings`の地形検出に加えて、追加の接触可能レイヤー。 |
| `_maxRange` | `AimCheck`によって検出できる最大距離。 |
| `OnChangeAimedGameObject` | 視点前のオブジェクトが変わった際に呼び出されるUnityイベント。 |

## メソッド
| Name | Function |
|------------------|------|
| ``public bool`` HitCheck( ``Vector2 center, out Vector3 point, out Vector3 normal, out float distance, out GameObject hitGameObject, out Vector3 direction`` ) | 画面中央からの照準チェックを行う。 |
| ``public bool`` HitCheck( ``out Vector3 point, out Vector3 normal, out float distance, out GameObject hitGameObject, out Vector3 direction`` ) | 照準チェックを即時に実行する。 |

