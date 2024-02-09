# HighPriorityTargetSearch

#### **Namespace**: Unity.TinyCharacterController.Check
---

## 概要:
`HighPriorityTargetSearch` は、最低コストの対象を発見するコンポーネントです。このコンポーネントは、対象までの角度、距離、対象が見えるかどうか、そしてメインカメラに捉えられているかどうかを考慮したコストを計算します。コストの計算はカスタマイズ可能です。たとえば、敵を発見した際にどの敵を攻撃するかを決定するために使用されます。

計算は `Find` が実行されたときのみ行われます。

## 機能と操作:
- **視界の範囲と角度**: 対象を検出するための視界の範囲（`Range`）と角度（`MaxAngle`）を設定できます。
- **視認性とコストの計算**: 対象がメインカメラによって視認されているか、及びその他の条件に基づいてコストを計算します。
- **カスタマイズ可能なコスト計算**: `OnCalculatePriority` アクションを使用して、発見された要素に対するコストの計算方法をカスタマイズできます。
- **最低コストの対象の発見**: `Find` メソッドを使って、条件に合致する最低コストの対象を範囲内で発見します。

## プロパティ
| Name | Description |
|------------------|------|
| `HeadOffset` | 視認性を判断するための頭の高さ。 |
| `Range` | 視界の範囲。 |
| `MaxAngle` | 検出に含めるオブジェクトの角度の範囲。 |
| `CostLimit` | 計算されたコストの上限。この値より大きいコストを持つ対象はリストから削除されます。 |
| `RadiusForIsInsightCheck` | 視認性チェックのためのレイの幅。 |
| `HitLayer` | 検索対象に含めるレイヤー。 |
| `TargetTags` | 検索対象に含めるタグ。 |
| `IsCalculateDistance` | キャラクターまでの距離判定を計算するかどうか。 |
| `IsCalculateAngle` | 方向からの角度を計算するかどうか。 |
| `IsCalculateIsInsight` | キャラクターからの視認性を計算するかどうか。 |
| `IsCalculateVisible` | メインカメラによって見られるオブジェクトのみを含めるかどうか。 |

## メソッド
| Name | Function |
|------------------|------|
|  ``public void`` SetCost( ``int index, int cost`` )  | "指定された要素のコストを直接設定します。主にVisualScriptingでの使用を目的としています。" |
|  ``public bool`` Find( ``out Transform target`` )  | "範囲内の条件に合致する最低コストの対象を発見します。TransformのPositionとForwardを使用して判断します。" |
|  ``public bool`` Find( ``out Transform target, Vector3 direction`` )  | "範囲内の条件に合致する最低コストの対象を発見します。TransformのPositionを使用して判断します。" |

