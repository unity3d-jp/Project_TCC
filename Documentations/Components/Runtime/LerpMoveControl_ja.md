# LerpMoveControl

#### **Namespace**: Unity.TinyCharacterController.Control
---

## 概要:
`LerpMoveControl` は、キャラクターを現在の位置から目標位置へ移動させるコンポーネントです。ジャンプや指定された場所への移動など、特定の場所へ移動する必要があるアクションで使用されます。目標位置は `SetTarget` を使用して設定され、アニメーター内の `MatchTargetBehaviour` と連携して使用されます。

## 機能と操作:
- **目標位置への移動**: キャラクターを線形補間を使用して目標位置へ移動させます。
- **回転の更新**: 目標の回転に合わせてキャラクターの向きを更新します。
- **優先度制御**: 移動と回転に優先度を設定し、他のコンポーネントとの相互作用を管理します。

## プロパティ
| 名前 | 説明 |
|------------------|------|
| `Priority` | 移動と回転のための優先度です。 |
| `IsPlaying` | `LerpMoveControl` が現在進行中かどうかを示します。 |

## メソッド
| 名前 | 機能 |
|------------------|------|
|  ``public void`` Play( ``PropertyName id`` )  | "指定された ID のターゲットプロセスを開始します。" |
|  ``public void`` Stop( ``PropertyName id`` )  | "指定された ID のターゲットプロセスを停止します。" |
|  ``public void`` SetNormalizedTime( ``float moveAmount, float turnAmount`` )  | "目標位置に対する正規化された時間を設定します。" |
|  ``public void`` SetTarget( ``PropertyName id, Vector3 position`` )  | "目標位置を設定します。" |
|  ``public void`` SetTarget( ``PropertyName id, Quaternion rotation`` )  | "目標回転を設定します。" |
|  ``public void`` SetTarget( ``PropertyName id, Vector3 position, Quaternion rotation`` )  | "目標位置と回転を設定します。" |
|  ``public void`` SetTarget( ``PropertyName id, Vector3 position, Vector3 offset`` )  | "オフセットを考慮した目標位置を設定します。" |
|  ``public void`` SetTarget( ``PropertyName id, Vector3 position, Quaternion rotation, Vector3 offset`` )  | "オフセットを考慮した目標位置と回転を設定します。" |
|  ``public void`` RemoveTarget( ``PropertyName id`` )  | "目標位置情報を削除します。" |
|  ``public void`` Cancel( ``PropertyName id`` )  | "現在のマッチングターゲットプロセスをキャンセルします。" |

---
## その他の注意事項
- このコンポーネントは、キャラクターを滑らかに目標位置へと移動させることを目的としており、移動と回転のための線形補間を提供します。