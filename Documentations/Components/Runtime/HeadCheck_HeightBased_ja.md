# HeadCheck (HeightBased) (HeightBasedHeadCheck.cs)

#### **Namespace**: Unity.TinyCharacterController.Check
---

## 概要:
`HeadCheck (HeightBased) (HeightBasedHeadCheck.cs)`は、頭上のオブジェクトとの接触チェックを行うコンポーネントです。しかし、この頭上チェックはレイキャストではなく、高さによって行われます。これにより処理時間が削減されます。これは、地形が単純なゲームや天井のないゲームでの使用を想定しています。

## 機能と操作:
- **高さに基づいた頭上検出**: 高さを基準にして頭上のオブジェクトとの接触を検出する。
- **接触判定の簡素化**: レイキャストではなく高さのみを利用して頭上の接触を判断することで、処理の負荷を軽減する。

## プロパティ
| Name | Description |
|------------------|------|
| `RoofHeight` | 天井と判断される高さ。 |
| `MaxHeight` | 検出可能な最大の高さ。 |
| `IsObjectOverhead` | 頭上にオブジェクトがあるかどうかを示す。 |
| `RoofObject` | 頭上判定時に接触していると判断されるオブジェクト。 |

## メソッド
- 公開されているメソッドはありません。

