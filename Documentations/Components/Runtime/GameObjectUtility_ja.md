# GameObjectUtility

#### **Namespace**: Unity.TinyCharacterController.Components.Utility
---

## 概要:
`GameObjectUtility` は、GameObjectのタグをチェックするための静的メソッドを提供するクラスです。このメソッドは、指定されたタグのリストにGameObjectが含まれているかどうかを確認し、リストに何も登録されていない場合は常にTrueを返します。

## 機能と操作:
- **タグのチェック**: GameObjectのタグが指定されたタグのリストに含まれているかどうかをチェックします。

## メソッド
| 名前 | 機能 |
|------------------|------|
|  ``static bool`` ContainTag( ``GameObject obj, in string[] hitTags`` )  | 指定されたタグのリストにGameObjectが含まれているかどうかを確認します。リストに何も登録されていない場合は、常にTrueを返します。 |

---
## その他の注意事項
- このクラスはGameObjectのタグを扱う基本的なユーティリティを提供し、特定のタグに基づいて処理を行う際に便利です。
- `_hitTag` リストが空の場合、このメソッドは常に `true` を返し、これはタグのチェックを行わずに処理を進めることを意味します。