# AssetReferenceScene

#### **Namespace**: Unity.SceneManagement
---

## 概要:
`AssetReferenceScene` は、シーン参照のアセットリファレンスを表すコンポーネントです。このコンポーネントは、シーンアセットへの参照を管理し、アセットの検証機能を提供します。

## 機能と操作:
- **シーンアセットへの参照**: `AssetReferenceScene`は`SceneReference`タイプのアセットへの参照を保持します。
- **アセット検証**: Unityエディタ内で使用される際、アセットのパスがシーンアセットとして有効であるかどうかを検証します。
- **エディタ専用アセット取得**: Unityエディタ内でのみ使用され、指定されたGUIDに基づいてシーンアセットを取得します。

## プロパティ
| Name | Description |
|------------------|------|
| `editorAsset` | Unityエディタで使用され、GUIDに基づいて指定されたシーンアセットを取得します。  |

## メソッド
| Name | Function |
|------------------|------|
|  ``public`` AssetReferenceScene( ``string guid`` )  | 指定されたGUIDを使用して新しいシーンアセットリファレンスを構築します。 |
|  ``public override bool`` ValidateAsset( ``string path`` )  | 指定されたパスのアセットがシーンアセットとして有効かどうかを検証します。 |

---
## その他の注意事項
- `AssetReferenceScene` および `SceneReference` クラスは、Addressable Asset Systemを使用してシーンアセットへの参照を管理するために設計されています。これにより、ゲームのシーン管理がより柔軟かつ効率的になります。