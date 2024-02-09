# TpsCameraControl

#### **Namespace**: Unity.TinyCharacterController.Control
---

## 概要:
`TpsCameraControl` はカメラの視点変更を管理するコンポーネントです。このコンポーネントは、`RotateCamera`を使用して`CameraRoot`で指定されたTransformの方向を更新し、`CameraPitch`で指定された角度で垂直方向の変更を制限します。カメラの視点移動の速度は`CameraUserSettings`によって補正されます。`TurnPriority`が高い場合、キャラクターはカメラで指定された方向を向きます。

## 機能と操作:
- **カメラ視点の更新**: `UpdateCamera`を使用して、カメラの根元のTransformの方向を更新します。
- **カメラ視点の制限**: 垂直方向の変更は`CameraPitch`で指定された角度によって制限されます。
- **視点移動速度の補正**: `CameraUserSettings`を使用して、カメラの視点移動速度を補正します。
- **キャラクターの向き変更**: `TurnPriority`が高い場合、キャラクターはカメラで指定された方向に向きます。

## プロパティ
| 名前 | 説明 |
|------------------|------|
| `_cameraRoot` | `UpdateCamera`によって更新されるTransformです。 |
| `_cameraPitch` | `CameraRoot`の最大仰角です。 |
| `_userSettings` | カメラを操作する際のユーザー設定です。 |
| `TurnPriority` | 回転の優先度です。このコンポーネントが最高優先度を持つ場合、キャラクターはカメラの方向を向きます。 |
| `_characterTurnSpeed` | キャラクターの回転速度です。この値が-1の場合、キャラクターは即座に方向を変更します。 |

## メソッド
| 名前 | 機能 |
|------------------|------|
| ``void`` RotateCamera( ``Vector2 inputLook`` ) | カメラの回転を更新します。`inputLook`はマウスのデルタ位置です。 |
| ``void`` ForceUpdateRotation( ``Quaternion rotation`` ) | カメラを指定された方向に強制的に向けます。`CameraRoot`も即座に更新されます。 |
| ``void`` OnUpdate( ``float deltaTime`` ) | カメラの更新を行います。 |

---
## その他の注意事項
- カメラの操作性は解像度に依存しないように設計されています。
- カメラのルートは、キャラクターの方向変更がカメラの方向変更に干渉しないように、別のオブジェクトに移動されています。
