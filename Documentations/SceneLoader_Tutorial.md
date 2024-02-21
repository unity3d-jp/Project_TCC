# SceneLoader チュートリアル

## 参考ムービー

[![](https://img.youtube.com/vi/Dp1vMD3wCTw/0.jpg)](https://www.youtube.com/watch?v=Dp1vMD3wCTw)

## 1.新しいシーンの作成

-  UnityのメインメニューからAssets > Create > Scene を選択して、新しいシーンを作成します。シーンにはSubSceneという名前をつけましょう。
-  SubSceneを開き、DirectionalLightとMainCameraを削除します。
-  Cubeを作成し、Cube_1という名前をつけます。またTransformの座標を(3,1,3)に設定します。
-  CubeのBoxColliderコンポーネントのIsTriggerにチェックを入れます。

## 2.SceneLoaderによるシーンの展開

-  元のシーン（NewScene.unity）を開きます。
-  HierarchyのメニューからSceneLoaderを選択し、SceneLoaderオブジェクトを作成します。
-  SceneLoaderコンポーネントのSceneAssetに、先ほど作成したSubScene.unityをドラッグ&ドロップします。この動作によりAddressableに自動的に先ほど作成したシーンが登録されます
-  先ほど登録したシーンを展開します。SceneLoaderコンポーネントのEditSettings>Openボタンを押すか、Hierarchyのチェックボックスを有効にします。
-  エディターでシーンを開いた際に自動的にシーンが展開されるようにします。SceneLoaderコンポーネントのEditSettings>SceneOpenBehaviourをLoadAndEditに変更します。

>Tips : エディター上でのシーンの展開は、GameObjectの有効/無効に関わらず動作します。

## 3.実行時のSceneの読み込み・解放

プレイヤーがトリガーの範囲に入った時に自動的にシーンを読み込み、離れると解放する処理を追加します。

-  SceneLoaderコンポーネントを持つオブジェクトのTransformの座標を（3, 1, 3）に設定します。
-  このオブジェクトにBoxColliderを追加し、IsTriggerにチェックを入れ、Sizeを(5,2,5)に設定します。
-  SceneLoaderコンポーネントのチェックボックスを外して無効にし、シーンが自動的に読み込まれないようにします。
-  ScriptMachineを追加し、グラフビュー内でプレイヤーがトリガーの範囲に入った際にSceneLoaderを有効・無効にする処理を作成します。
    -  OnTriggerEnterノードとSceneLoaderのOnEnableノードを追加し、接続する。OnEnableの値はTrueを設定します。
    -  OnTriggerExitノードとSceneLoaderのOnEnableノードを追加し、接続する。OnEnableの値はFalseを設定します。

> Tips : SceneLoaderがインストールされている場合、シーンはゲームの再生時に親シーンを残して解放される。  
> この挙動はSceneViewのSceneLoaderOverlayで切り替える事が可能。
> RootOnly : 親シーンを残して解放する。実際にゲームを再生した際の挙動。  
> UseStart : 指定のシーンで起動する。  
> KeepAll : 現在のシーンを維持する。コンポーネントやScriptMachineの動作を確認したい場合に有効。

## 4.シーンの引数

シーンの読み込み元の情報を元に、シーンの動作を切り替えます。

-  SceneLoaderコンポーネントを持つオブジェクトにFlagCollectionContainerコンポーネントとSaveDataControlコンポーネントを追加します。
-  FlagCollectionContainerコンポーネントのValuesを増やし、チェックはFalse、フラグ名は"Cube_1"を設定します。
-  SubSceneのCube_1を選択し、オーナー（シーンの読み込みを行ったSceneLoader）の持つフラグで、Cubeの表示を切り替える処理を作成します。具体的には以下のようなグラフを作成します。
    -  Cube_1にScriptMachineを追加し、このコンポーネントでEdit Graphボタンを押して、ビジュアルスクリプティング環境を開きます。
    -  オーナーのSceneLoaderを取得します。  
    SceneLoaderManager.GetOwnerノードを追加し、引数にThisを設定します。
    -  オーナーの持つフラグ情報を確認し、無効ならばCube_1を非アクティブにします。  
    SceneLoaderManager.GetOwnerの戻り値とFlagCollectionContainer.GetFlag("Cube_1")を接続し、オーナーの持つフラグ情報を取得します。この処理はOnStartから接続します。
    -  取得したフラグがTrueの場合、Cube_1オブジェクトのマテリアルを変更します。 Ifノードを追加し、先ほどのFlagCollectionContainer.GetFlagの戻り値と接続。Falseの先にMeshRenderer.SetMaterialノードを追加し、マテリアルを適当なマテリアルを設定。
-  シーン内の変更をオーナーに反映します。具体的にはキャラクターがCube_1と接続した際にオーナーのフラグを更新します。
    -  OnTriggerEnterノードを追加します。
    -  SceneLoaderManager.GetOwnerの戻り値とFlagCollectionContainer.SetFlag("Cube_1", True)を接続します。
