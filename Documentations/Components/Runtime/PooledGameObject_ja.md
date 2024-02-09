# PooledGameObject

#### **Namespace**: Unity.TinyCharacterController.Utility
---

## 概要:
`PooledGameObject` は、`GameObjectPool` によってプールされるオブジェクトを管理するためのコンポーネントです。`IGameObjectPool.Get()` で取得し、`IGameObjectPool.Release(IPooledObject)` でリリースすることができます。

## 機能と操作:
- **ライフタイム機能の利用**: オブジェクトが一定期間後に自動的にリリースされるように設定します。
- **リリース時のコールバック**: オブジェクトがリリースされるときにコールバックイベントを呼び出します。

## プロパティ
| 名前 | 説明 |
|------------------|------|
| `_isUseLifetime` | ライフタイム機能を使用するかどうか。 |
| `_lifeTime` | オブジェクトが有効である期間。 |
| `OnReleaseByLifeTime` | オブジェクトがライフタイムによってリリースされたときに呼び出されるコールバック。 |
| `IsUsed` | `PooledGameObject` が使用中かどうか。 |
| `ReleaseTime` | `PooledGameObject` がリリースされる時間。 |
| `IsPlaying` | `PooledGameObject` が現在再生中かどうか。 |

## メソッド
- 公開されているメソッドはありません。

---
## その他の注意事項
- `PooledGameObject` は `IPooledObject` インターフェイスを実装しており、プールシステム内で管理されるオブジェクトの基本的なインターフェースを提供します。
- ライフタイム機能を使用すると、オブジェクトが一定時間後に自動的にプールに戻されるため、リソースの効率的な管理が可能になります。
- オブジェクトがリリースされるときには、`OnReleaseByLifeTime` イベントが発火し、追加のクリーンアップ処理や通知処理を行うことができます。

