# RigidbodyBrain

#### **Namespace**: Unity.TinyCharacterController.Brain
---

## 概要:
`RigidbodyBrain`は、`UnityEngine.Rigidbody`を使用して動作するコンポーネントです。このコンポーネントは`UnityEngine.CapsuleCollider`と`UnityEngine.Rigidbody`で動作し、キャラクターは`_stepHeight`値によって定義された高い位置に配置され、移動中に`_skinWidth`値で定義されたマージンで停止します。`CharacterSettings.Height`と`CharacterSettings.Radius`によって`UnityEngine.CapsuleCollider`の高さと幅が決定されます。適切に機能するためには、`IGravity`と`IGroundContact`が必要です。

## 機能と操作:
- **軸固定**: キャラクターが移動できる軸を設定する。
- **スキン幅**: キャラクターと壁の間に設定される幅。
- **ステップ高さ**: キャラクターが乗り越えることができる段差の高さ。
- **位置と回転の更新**: キャラクターの位置と回転を更新する。

## プロパティ
| Name | Description |
|------------------|------|
| `FreezeAxis` | キャラクターの移動を制限する軸を設定する。 |
| `LockAxis` | `FreezeAxis`のVector3形式。 |
| `_skinWidth` | キャラクターと壁の間の幅を設定する。 |
| `_stepHeight` | キャラクターが乗り越えることができる段差の高さを設定する。 |

## メソッド
| Name | Function |
|------------------|------|
| ``public void`` SetFreezeAxis( ``bool x, bool y, bool z`` )  | キャラクターの移動可能な軸を設定する。 |

