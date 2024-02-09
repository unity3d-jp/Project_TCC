# SceneLoaderExtensions

#### **Namespace**: Unity.SceneManagement
---

## 概要:
`SceneLoaderExtensions` は、`GameObject`に対する拡張メソッドを提供する静的クラスです。この拡張機能を使用して、`SceneLoader`によって追加されたシーンのオーナー`GameObject`を取得できます。

## 機能と操作:
- **オーナーの取得**: `GameObject`が属するシーンのオーナーを取得します。

## メソッド
| Name | Function |
|------|----------|
|  ``public static GameObject`` GetOwner( ``this GameObject obj`` )  | 指定された`GameObject`が属するシーンのオーナー`GameObject`を返します。 |

---
## その他の注意事項
- この拡張メソッドは、特に`SceneLoader`を使用して動的に読み込まれるシーンにおいて、そのシーンに属する任意の`GameObject`からオーナーを簡単に参照できるようにすることを目的としています。
- オーナーとは、シーンを管理する責任を持つ`GameObject`であり、通常はシーンを読み込む`SceneLoader`コンポーネントを持つオブジェクトを指します。
- `SceneLoaderManager.GetOwner(scene)`メソッドを使用して、指定されたシーンに関連付けられているオーナー`GameObject`を取得します。このプロセスは、シーンのメタデータと`SceneLoaderManager`内の追跡情報を利用します。

この拡張メソッドは、シーン管理の柔軟性を高め、特定のシーンやそのコンテンツとの相互作用を容易にするための便利なツールです。

