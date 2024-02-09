# HeadPositionCheck (HeadCollisionCheck.cs)

#### **Namespace**: Unity.TinyCharacterController.Check
---

## 概要:
`HeadCollisionCheck`は、上方向のオブジェクト検出を行うコンポーネントです。`CharacterSettings`で設定された高さを考慮した上方向の検出を行い、他のオブジェクトとの完全な接触だけでなく、わずかに曖昧な検出も行い、衝突時にUnityイベントを呼び出します。

## 機能と操作:
- **上方向の検出**: キャラクターの頭部上方にあるオブジェクトの検出を行う。
- **接触と距離の計算**: オブジェクトとの接触有無や距離を計算し、それに基づいてプロパティを設定する。
- **衝突時のイベント呼び出し**: 頭部がオブジェクトに接触した際にUnityイベントを発行する。

## プロパティ
| Name | Description |
|------------------|------|
| `_headPositionOffset` | 頭部位置からのオフセット。オブジェクト上方の接触判定に使用される。 |
| `MaxHeight` | 上方向のオブジェクトを検出できる最大距離。 |
| `_onContact` | 頭部がこのフレーム中に接触した際に呼び出されるUnityイベント。 |
| `_onChangeInRange` | `InRange`の値が変更された際に実行されるUnityイベント。 |
| `IsHitCollisionInThisFrame` | このフレーム中に他のオブジェクトとの接触があったかどうか。 |
| `DistanceFromRootPosition` | 接触点からの距離。 |
| `IsHeadContact` | 他のオブジェクトとの接触があるかどうか。 |
| `IsObjectOverhead` | 頭部上方にコライダーが存在するかどうか。 |
| `ContactPoint` | 接触点。 |
| `ContactedObject` | 接触したオブジェクト。 |

## メソッド
- 公開されているメソッドはありません。

