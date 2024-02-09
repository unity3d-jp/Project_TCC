# MoveNavmeshControl

#### **Namespace**: Unity.TinyCharacterController.Control
---

## 概要:
`MoveNavmeshControl` は、Navmeshに基づいてキャラクターを移動させるコンポーネントです。`_agent`によって指定されたコンポーネントを使用して、`SetTargetPosition(Vector3)`によって指定された座標へのパス検索を行い、`IMove`のコンテキスト内でキャラクターを移動させます。`MovePriority`が高い場合、キャラクターはNavmeshAgentによって設定された最短のパス上で移動します。`TurnPriority`が高い場合、キャラクターは目的地の方向に転向します。

## 機能と操作:
- **移動エージェント設定**: キャラクターの移動を制御するために使用されるエージェント。非同期でパス計算を行います。
- **移動優先度と転向優先度**: 移動や転向の優先度を設定し、それに応じてキャラクターが動作します。
- **目的地への移動**: `SetTargetPosition(Vector3)` を使って目的地の座標を設定し、そこへ移動します。

## プロパティ
| 名前 | 説明 |
|------------------|------|
| `_agent` | キャラクターの移動を制御するためのNavMeshAgent。 |
| `_speed` | キャラクターの最大移動速度。 |
| `TurnSpeed` | キャラクターの転向速度。 |
| `MovePriority` | 移動の優先度。 |
| `TurnPriority` | 転向の優先度。 |
| `OnArrivedAtDestination` | 目的地に到着したときのコールバックイベント。 |

## メソッド
| 名前 | 機能 |
|------------------|------|
| ``public void`` SetTargetPosition( ``Vector3 position`` ) | 移動先の座標を設定します。 |
| ``public void`` SetTargetPosition( ``Vector3 position``, ``float distance`` ) | 目的地との距離を維持しながら移動先の座標を設定します。 |

