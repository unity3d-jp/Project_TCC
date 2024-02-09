# CharacterSettings

#### **Namespace**: Unity.TinyCharacterController
---

## 概要:
`CharacterSettings` は、`IBrain` や TCC コンポーネントの動作に必要な設定を取得または設定するコンポーネントです。キャラクターの高さや幅は `CharacterSettings.Height` と `CharacterSettings.Radius` によって決定されます。

## 機能と操作:
- **環境設定**: 地形コライダーを認識するためのレイヤーを設定します。
- **キャラクター設定**: 他のリジッドボディと衝突した際に適用される力を設定します。
- **キャラクターの高さ**: キャラクターの高さを設定します。
- **キャラクターの幅**: キャラクターの幅を設定します。
- **コライダーの格納**: GameObject内のColliderコンポーネントを格納するためのリストです。
- **カメラの設定**: キャラクターに関連するカメラの情報を設定します。

## プロパティ
| Name | Description |
|------------------|------|
| `_environmentLayer` | 地形コライダーを認識するためのレイヤーを設定します。 |
| `_mass` | 他のリジッドボディと衝突した際に適用される力を設定します。 |
| `_height` | キャラクターの高さを設定します。 |
| `_radius` | キャラクターの幅を設定します。 |
| `HasCamera` | カメラが設定されているかどうかを取得します。 |
| `CameraMain` | キャラクターに関連するカメラの情報を取得または設定します。 |
| `CameraTransform` | カメラのTransformを取得します。 |
| `CameraYawRotation` | カメラのY軸回転を取得します。 |

## メソッド
| Name | Function |
|------------------|------|
| ``public void`` GatherOwnColliders() | キャラクターに属するコライダーを収集します。 |
| ``public bool`` IsOwnCollider(``Collider col``)` | 指定されたコライダーがキャラクターに属するかどうかを確認します。
| ``public bool`` ClosestHit(``RaycastHit[] hits, int count, float maxDistance, out RaycastHit closestHit``)` | キャラクター自身のコライダーを除外した最も近いRaycastHitを取得します。
| ``public Vector3`` PlayerInputToWorldSpaceDirection(``Vector2 input``)` | プレイヤーの入力を画面上の世界空間方向に変換します。

---
## その他の注意事項
- `CharacterSettings` は、キャラクターの物理的特性やカメラの動作を制御する重要なコンポーネントです。高さ、幅、質量の設定に加えて、キャラクターが操作する地形や環境に関連するレイヤー設定も行います。
- キャラクターの動きやカメラの回転など、ゲームプレイにおける基本的な動作を管理するための設定を提供します。これにより、ゲームの物理エンジンを活用し、自然で滑らかなキャラクターの動きを実現することが可能です。
- キャラクター自身のコライダーの管理や、プレイヤーの入力に基づく移動方向の計算など、ゲーム内のキャラクター制御に必要な機能を提供します。

