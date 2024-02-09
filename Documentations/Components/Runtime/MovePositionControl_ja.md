# MovePositionControl

#### **Namespace**: Unity.TinyCharacterController.Control
---

## 概要:
`MovePositionControl` は、指定された位置へのキャラクターの移動を行うコンポーネントです。`SetTargetPosition(Vector3)` によって指定された座標にキャラクターを移動させます。`IMove`の文脈で動作します。

## 機能と操作:
- **移動**： `SetTargetPosition(Vector3)` メソッドを使用してターゲット位置を設定し、キャラクターを指定された位置に移動させます。
- **回転**： `TurnPriority` が高い場合、キャラクターは移動方向を向きます。
- **地形に沿った移動**：`_ignoreYAxis` が `false` に設定されている場合、キャラクターは地形に沿って移動します。
- **障害物回避の欠如**：このコンポーネントは障害物を回避する機能はありません。障害物を回避するためには、別のコンポーネントが必要です。

## プロパティ
| 名前                 | 説明                                      |
|----------------------|------------------------------------------|
| `Speed`              | キャラクターの最大移動速度                |
| `TurnSpeed`          | キャラクターの回転速度                    |
| `MovePriority`       | 移動の優先度                              |
| `TurnPriority`       | 回転の優先度                              |
| `OnArrivedAtDestination` | 目的地に到着した際のコールバック       |
| `_maxSlope`          | 移動可能な斜面の角度                      |
| `_ignoreYAxis`       | Y軸を無視するかどうか                      |
| `_currentSpeed`      | 現在の移動速度                            |
| `IsArrived`          | 目的地に到着したかどうか                   |

## メソッド
| 名前                                             | 機能                                   |
|--------------------------------------------------|----------------------------------------|
| ``public void`` SetTargetPosition( ``Vector3 position`` )  | 指定された位置にキャラクターを移動させる |
| ``public void`` SetTargetPosition( ``Vector3 position, bool ignoreYAxis`` )  | 指定された位置にキャラクターを移動させる（Y軸無視オプション付き） |
| ``public void`` SetTargetPosition( ``Vector3 position, float distance`` )  | 指定された位置の周囲を移動させる（一定距離を保ちながら） |
| ``public void`` SetTargetPosition( ``Vector3 position, float distance, bool ignoreYAxis`` )  | 指定された位置の周囲を移動させる（Y軸無視、距離保持オプション付き） |
| ``public void`` SetTargetPosition( ``Vector3 position, float distance, bool ignoreYAxis, int cycleAround`` )  | 指定された位置の周囲を移動させる（Y軸無視、距離保持、周回するオプションを含みます） |

---
## その他の注意事項
- このコンポーネントは障害物を回避する機能はありません。障害物を回避するためには、別のコンポーネントが必要です。
- ターゲット位置は、`SetTargetPosition` メソッドによって動的に設定されます。
- 移動と回転の優先度は、他の移動コンポーネントとの相互作用に影響を与えることがあります。

