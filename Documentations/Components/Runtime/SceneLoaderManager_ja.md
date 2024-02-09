# SceneLoaderManager

#### **Namespace**: Unity.SceneManagement
---

## 概要:
`SceneLoaderManager` は、アセットシーンの読み込みとアンロードを管理する静的クラスです。このマネージャーは、読み込まれたシーンとそれらのオーナーオブジェクトを追跡し、シーンの読み込みとアンロードの進捗を管理します。

## 機能と操作:
- **シーンの読み込み**: アセットリファレンスを使用してシーンを非同期に読み込みます。
- **シーンのアンロード**: 指定されたシーンを非同期にアンロードします。
- **読み込み進捗の追跡**: 読み込まれている全てのシーンの進捗を集約して返します。
- **オーナーオブジェクトの管理**: 各シーンに対するオーナーオブジェクトを登録し、追跡します。

## メソッド
| Name | Function |
|------|----------|
|  ``public static AsyncOperationHandle<SceneInstance>`` Load( ``AssetReferenceScene scene, string sceneName, GameObject ownerObject, int priority = 100`` )  | 指定されたアセットリファレンスを使用してシーンを読み込み、読み込み操作のハンドルを返します。 |
|  ``public static void`` Unload( ``AssetReferenceScene scene, string sceneName, Action onComplete = null`` )  | 指定されたアセットリファレンスを使用してシーンをアンロードします。 |
|  ``public static GameObject`` GetOwner( ``Scene scene`` )  | 指定されたシーンのオーナーオブジェクトを取得します。 |
|  ``public static bool`` HasProgress  | 読み込み中のシーンが存在するかどうかを示します。 |
|  ``public static float`` Progress(  )  | 現在のシーン読み込みの進捗をパーセンテージで返します。 |

---
## その他の注意事項
- このマネージャーは、`AssetReferenceScene`オブジェクトのリストを保持し、読み込まれたシーンの追跡を行います。また、`PropertyName`をキーとする辞書を使用して、各シーンのオーナーオブジェクトを管理します。
- `Load` メソッドと `Unload` メソッドは、`AddressableAssets`システムを使用してシーンの読み込みとアンロードを行います。これにより、効率的なアセット管理と動的なコンテンツのロードが可能になります。

