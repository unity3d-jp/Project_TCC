# IndicatorPin

#### **Namespace**: Unity.TinyCharacterController.UI
---

## 概要:
`IndicatorPin` は、UIを3D空間の座標と同期させるためのコンポーネントです。`WorldPosition` で指定された座標にUIの位置を合わせて調整します。`CanvasGroup` を使用して、UIが画面外に出たときに非表示にします。

## 機能と操作:
- **3D座標との同期**: 3D空間の指定された座標にUIの位置を合わせます。
- **オフスクリーン非表示**: UIが画面外に出ると自動的に非表示になります。
- **位置のオフセット**: `WorldPosition` に加えるオフセットを指定でき、UIの表示位置を微調整できます。

## プロパティ
| 名前 | 説明 |
|------------------|------|
| `_cameraUpdateTiming` | コンポーネントが更新されるタイミングです。カメラがUpdateフレーム中に動く場合はUpdate、Physicsフレーム中に動く場合はFixedUpdateに設定します。 |
| `_worldPosition` | UIのワールド座標です。 |
| `_positionOffset` | UIのワールド座標に加えるオフセットです。 |
| `WorldPosition` | UIが表示される位置です。外部から設定することで、UIの位置を更新できます。 |
| `CorrectedPosition` | オフセットを加味したUIのワールド座標です。 |
| `UiSize` | UIのサイズです。 |

## メソッド
- コンポーネントは、`WorldPosition` の値が変更されると、その位置にUIを配置するように内部的に動作します。UIの表示/非表示は自動的に制御され、開発者が直接制御するメソッドは公開されていません。

---
## その他の注意事項
- `IndicatorPin` は `CanvasGroup` および `RectTransform` コンポーネントを必要とします。これらのコンポーネントは、UIの表示・非表示制御と、UIの位置調整に利用されます。
- UIが画面外に出た場合の非表示処理は `CanvasGroup` の透明度を0にすることで行われます。
- UIの位置更新は、設定された `WorldPosition` と `PositionOffset` を基に計算され、UIが3D空間内の指定された位置に表示されるようになります。
