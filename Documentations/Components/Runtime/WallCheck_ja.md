了解しました。指定されたフォーマットに従い、`WallCheck`コンポーネントの分析を再度行い、メソッドを表形式で出力します。

# WallCheck

#### **Namespace**: Unity.TinyCharacterController.Check
---

## 概要:
`WallCheck`は、壁との衝突検出を行うコンポーネントです。キャラクターの移動方向にある壁を検出し、壁との衝突時に複数のUnityイベントを発行します。

## 機能と操作:
- **壁との衝突検出**: キャラクターの移動方向にある壁を検出し、接触有無を判断する。
- **衝突時のコールバック**: 壁との衝突時に複数のUnityイベントを発行する。

## プロパティ
| Name | Description |
|------------------|------|
| `_wallAngleRange` | 壁と認識される角度の範囲。 |
| `_wallDetectionDistance` | 壁検出距離。 |
| `OnWallContacted` | 壁に接触した際に発生するイベント。 |
| `OnWallLeft` | 壁から離れた際に発生するイベント。 |
| `OnWallStuck` | 壁と接触中に発生するイベント。 |
| `IsContact` | 壁と接触しているかどうか。 |
| `Normal` | 接触面の法線ベクトル。 |
| `ContactObject` | 接触したオブジェクト。 |
| `HitPoint` | 接触点。 |

## メソッド
| Name | Function |
|------------------|------|
| ``public bool`` HitCheck( ``Vector3 direction, out Vector3 normal, out Vector3 point, out Collider contactCollider`` ) | 指定された方向で壁検出を行い、接触した場合に法線、接触点、接触オブジェクトを返す。 |

