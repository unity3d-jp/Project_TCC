# GroundCheck

#### **Namespace**: Unity.TinyCharacterController.Check
---

## 概要:
`GroundCheck`は地面との衝突検出を行うコンポーネントです。地面が存在するかどうか、接触面の向き、接触オブジェクトに関する情報を判断し、地面オブジェクトに変更があった場合に通知します。このコンポーネントの計算は`OnUpdate`のタイミングで実行されます。

## 機能と操作:
- **地面検出**: 地面との衝突を検出し、接触しているかどうかを判断する。
- **地面接触情報の取得**: 地面の存在、接触面の方向、接触オブジェクトに関する情報を提供する。
- **地面オブジェクトの変更通知**: 地面オブジェクトに変更があった場合にUnityイベントを発行する。

## プロパティ
| Name | Description |
|------------------|------|
| `_ambiguousDistance` | 地面に接触していると認識される最大距離（曖昧な地面検出に使用）。 |
| `_preciseDistance` | 地面に接触していると厳密に認識される距離。 |
| `_maxSlope` | 地面と認識される最大の傾斜角度。 |
| `_onChangeGroundObject` | 地面のコライダーが変更された際に呼び出されるUnityイベント。 |
| `IsOnGround` | 地面に接触しているかどうかを示す。 |
| `IsFirmlyOnGround` | 厳密に地面に接触しているかどうかを示す。 |
| `MaxGroundCheckDistance` | 地面までの最大検出距離。 |
| `GroundCollider` | 現在の地面コライダー。 |
| `GroundSurfaceNormal` | 地面表面の向き。 |
| `GroundContactPoint` | 地面との接触点。 |
| `DistanceFromGround` | 地面までの距離。 |
| `GroundObject` | 地面オブジェクト。 |
| `IsChangeGroundObject` | 地面オブジェクトが現在のフレームで変更されたかどうか。 |

## メソッド
| Name | Function |
|------------------|------|
| ``public bool`` Raycast( ``Vector3 position, float distance, out RaycastHit hit`` ) | キャラクターのコライダーを無視してレイキャストを行う。 |

