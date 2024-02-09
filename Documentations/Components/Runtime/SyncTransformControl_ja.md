# SyncTransformControl

#### **Namespace**: Unity.TinyCharacterController.Control
---

## 概要:
`SyncTransformControl` は、キャラクターの位置を特定の Transform の位置に合わせるコンポーネントです。`MovePriority` が高い場合、キャラクターは `Target` 座標に一致する位置に移行します。また、`TurnPriority` が高い場合、キャラクターの向きを更新します。

## 機能と操作:
- **位置同期**: 指定した Transform の位置にキャラクターの位置を同期させます。
- **移動と回転の優先度**: 移動と回転の操作に優先度を設定し、他のコンポーネントとの相互作用を管理します。
- **滑らかな遷移**: `MoveTransitionTime` と `TurnTransitionTime` を使用して、目標点への移動や向きの変更を滑らかに遷移させます。

## プロパティ
| 名前 | 説明 |
|------------------|------|
| `_targetTransform` | 位置を同期させる対象の Transform です。 |
| `MoveTransitionTime` | 移動優先度が他のコンポーネントより高い場合に、ターゲットポイントに到達するまでの時間です。 |
| `TurnTransitionTime` | 回転優先度が他のコンポーネントより高い場合に、ターゲット方向に向きを変えるまでの時間です。 |
| `MovePriority` | キャラクターの移動優先度です。 |
| `TurnPriority` | キャラクターの向きを変える優先度です。 |

## メソッド
- 公開されているメソッドはありません。

---
## その他の注意事項
- 移動と回転の遷移を管理するために、滑らかな遷移を可能にするためのパラメータが用意されています。ターゲットの Transform が設定されていない場合、優先度は 0 に設定されます。