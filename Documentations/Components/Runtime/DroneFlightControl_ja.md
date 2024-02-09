# DroneFlightControl

#### **Namespace**: Unity.TinyCharacterController.Control
---

## 概要:
`DroneFlightControl` は、地形に依存せずにキャラクターを移動させるコンポーネントです。`MoveHorizontal` または `MoveVertical` を使用してキャラクターを動かします。

## 機能と操作:
- **移動設定**: 最大速度、加速量、ブレーキ力、旋回速度を設定できます。
- **視線の方向制御**: 入力した方向、または移動方向に向くかを `LookForward` で制御します。
- **水平のみの移動**: カメラのピッチを無視し、水平方向のみを移動するかを `IsOnlyHorizontal` で設定します。
- **移動と旋回の優先度**: 移動と旋回の優先度を設定します。

## プロパティ
| 名前 | 説明 |
|------------------|------|
| `Speed` | 最大速度。 |
| `Accel` | 加速量。 |
| `Brake` | ブレーキ力。 |
| `TurnSpeed` | 旋回速度。 |
| `LookForward` | 移動方向か入力方向かを指定します。 |
| `IsOnlyHorizontal` | 水平方向のみを移動するかどうか。 |
| `MovePriority` | 移動の優先度。 |
| `TurnPriority` | 旋回の優先度。 |

## メソッド
| 名前 | 機能 |
|------------------|------|
|  ``public void`` MoveHorizontal( ``Vector2 input`` )  | 水平方向に移動します。移動方向はカメラの方向によって補正されます。 |
|  ``public void`` MoveVertical( ``float input`` )  | 垂直方向に移動します。プラスで上昇、マイナスで下降。 |

