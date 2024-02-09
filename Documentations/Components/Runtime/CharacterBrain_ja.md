# CharacterBrain

#### **Namespace**: Unity.TinyCharacterController.Brain
---

## 概要:
`CharacterBrain` は、Unityの`CharacterController`を使用して操作するコンポーネントです。このコンポーネントは、`CharacterSettings.Height`と`CharacterSettings.Radius`によって決定されるエージェントの高さと幅を管理します。

## 機能と操作:
- **軸固定**: キャラクターが移動できる軸を設定する。
- **プッシュ可能性**: Rigidbodyを持つオブジェクトに衝突時に押すことができる。
- **衝突検出**: キャラクターの衝突を検出する。
- **地面の検知**: キャラクターが地面に接触しているかどうかを判定する。
- **位置と回転の更新**: キャラクターの位置と回転を更新する。

## プロパティ
| Name | Description |
|------------------|------|
| `FreezeAxis` | キャラクターの移動を制限する軸を設定する。 |
| `LockAxis` | `FreezeAxis`のVector3形式。 |
| `Pushable` | 他のRigidbodyに衝突した際に押すことが可能かどうか。 |
| `DetectCollisions` | キャラクターの衝突検出を有効または無効にする。 |

## メソッド
| Name | Function |
|------------------|------|
| ``public void`` SetFreezeAxis( ``bool x, bool y, bool z`` )  | キャラクターの移動可能な軸を設定する。 |

