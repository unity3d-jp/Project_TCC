# BrainBase

#### **Namespace**: Unity.TinyCharacterController.Core
---

## 概要:
`BrainBase` は、キャラクターを駆動する基本的なコンポーネントです。このコンポーネントは、同じオブジェクト内の `IMove` と `ITurn` コンポーネントから情報を収集し、それらをキャラクターに適用します。最優先の特別な処理が必要な場合は、`IMoveUpdate` と `ITurnUpdate` が使用されます。位置と回転を反映する具体的な方法は、継承されたコンポーネントによって決まります。例えば、`TransformBrain` であれば `UnityEngine.Transform.position` を使用し、`RigidbodyBrain` であれば `UnityEngine.Rigidbody.position` を使用します。位置の強制的な更新は `Warp` や `Warp(UnityEngine.Vector3)` を使用して行われます。Transform の Position と Rotation はキャッシュされ、再利用されます。このコンポーネントから継承するコンポーネントは `IEarlyUpdateComponent` を追加するべきです。これにより、ユーザーはキャラクターの事前計算（Check, Effect）とキャラクター反映（Brain）の間に自分の処理を挿入できます。

## 機能と操作:
- **キャラクターのワープ**: 新しい位置や向きにキャラクターを瞬時に移動させます。
- **コンポーネントの動的更新**: 実行時にコンポーネントを更新し、キャラクターの動きや向きを制御します。
- **衝突の回避**: 親オブジェクトと子コライダー間の接触を避け、キャラクターの無限の上昇を防ぎます。
- **キャラクターの速度と方向制御**: 現在の移動速度や向きを調整し、追加の速度（例: 重力や衝撃）を考慮に入れます。
- **カメラの更新**: キャラクターの位置や向きの更新後に、カメラの位置と向きを更新します。

## プロパティ
| 名前 | 説明 |
|------------------|------|
| `Settings` | キャラクターの設定を保持します。 |
| `CachedTransform` | キャラクターのTransformをキャッシュします。 |
| `Position` | キャラクターの現在位置を表します。 |
| `Rotation` | キャラクターの現在の回転を表します。 |
| `CurrentSpeed` | 現在のキャラクターの移動速度を返します。 |
| `TurnSpeed` | キャラクターの回転速度を返します。 |
| `YawAngle` | キャラクターの世界空間における向きを返します。 |
| `LocalVelocity` | キャラクターが向いている方向のローカルベクトルを返します。 |
| `ControlVelocity` | キャラクターの世界空間における移動ベクトルを返します。 |
| `EffectVelocity` | 追加される移動ベクトル（例: 重力や衝撃）を返します。 |
| `TotalVelocity` | キャラクターの移動ベクトルと追加の移動ベクトルの合計を返します。 |
| `DeltaTurnAngle` | 現在の方向と目標方向の差を返します。 |

## メソッド
| 名前 | 機能 |
|------------------|------|
|  ``void`` Warp( ``Vector3 position, Vector3 direction`` )  | "新しい位置と向きにキャラクターをワープします。directionがVector3.zeroの場合、現在の向きを維持します。" |
|  ``void`` Warp( ``Vector3 position, Quaternion rotation`` )  | "新しい位置と新しい回転でキャラクターをワープします。" |
|  ``void`` Warp( ``Vector3 position`` )  | "新しい位置にキャラクターをワープします。" |
|  ``void`` Warp( ``Quaternion rotation`` )  | "新しい回転でキャラクターをワープします。" |
|  ``void`` Move( ``Vector3 position`` )  | "指定された位置にキャラクターを移動させます。" |
|  ``protected abstract void`` ApplyPosition( ``in Vector3 totalVelocity, float deltaTime`` )  | "最終的な位置を適用します。" |
|  ``protected abstract Vector3`` AdjustMovementVector( ``Vector3 controlVelocity, Vector3 externalVelocity`` )  | "移動ベクトルを調整します。" |
|  ``private void`` UpdateComponents( ``float deltaTime`` )  | "コンポーネントを更新します。" |
|  ``protected void`` UpdateBrain( ``float deltaTime`` )  | "Brainの情報を更新します。" |
|  ``protected abstract void`` ApplyRotation( ``Quaternion rotation`` )  | "回転を適用します。" |
|  ``protected abstract void`` SetPositionDirectly( ``in Vector3 newPosition`` )  | "指定された位置にキャラクターを直接移動させます。" |
|  ``protected abstract void`` SetRotationDirectly( ``in Quaternion newRotation`` )  | "指定された回転でキャラクターの向きを直接設定します。" |
|  ``protected abstract void`` MovePosition( ``in Vector3 newPosition`` )  | "指定された位置にキャラクターを移動させます。" |
|  ``private void`` UpdateCamera( ``float deltaTime`` )  | "カメラの位置と向きを更新します。" |
|  ``protected abstract void`` OnUpdateCachedPosition( ``out Vector3 position, out Quaternion rotation`` )  | "継承されたBrainごとにPositionとRotationをキャッシュします。" |

---
## その他の注意事項
- `BrainBase` は抽象クラスであり、具体的なキャラクターの制御ロジックを実装するために継承される必要があります。
- キャラクターの移動や回転を管理するために、複数のインターフェイスが使用されています。
- このコードには、キャラクターの移動や回転を具体的に実装するための具体的なメソッド (`ApplyPosition`, `ApplyRotation` など) が含まれていません。これらは継承されたクラスで実装されるべきです。

