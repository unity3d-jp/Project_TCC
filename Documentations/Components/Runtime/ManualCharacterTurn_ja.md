# ManualCharacterTurn (ManualTurn.cs)

#### **Namespace**: Unity.TinyCharacterController.Control
---

## 概要:
`ManualCharacterTurn (ManualTurn.cs)` は、キャラクターの向きを手動で設定するコンポーネントです。`ManualControl` の使用が推奨されています。

## 機能と操作:
- **キャラクターの向きの設定**: `_direction` ベクトルを使用してキャラクターの向きを指定します。
- **転向優先度**: `_direction` が `Vector3.zero` の場合、優先度は0になります。それ以外の場合は `TurnPriority` に従います。
- **転向速度**: `TurnSpeed` で方向転換の速度を設定します。範囲は-1から50です。

## プロパティ
| 名前 | 説明 |
|------------------|------|
| `_direction` | キャラクターの向きを表すベクトル。 |
| `TurnPriority` | 転向の優先度。 |
| `TurnSpeed` | 方向転換の速度。 |


## メソッド
- 公開されているメソッドはありません。

