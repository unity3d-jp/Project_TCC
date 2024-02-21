# My First TCC チュートリアル

このチュートリアルでは、TCCを利用したゲームシーンのセットアップから、プレイヤーの操作、重力やジャンプなどの挙動の追加、そしてControlのプライオリティの切り替えによる挙動の変化など、TCCの根幹となる内容を学びます。

## 参考ムービー
[![](https://img.youtube.com/vi/qSAUngXAQkI/0.jpg)](https://www.youtube.com/watch?v=qSAUngXAQkI)


## 1.新しいシーンの作成

-  UnityのメインメニューからFile > New Sceneを選択して、新しいシーンを作成します。シーンにはNewSceneという名前をつけましょう。
-  ヒエラルキーパネルで右クリックし、Folderを選択してから、新しく作成された空のオブジェクトの名前をLevelに変更します。これは、ワールドを構成を管理するためのコンテナとなります。
-  Levelオブジェクトの下に、平面を表すPlaneオブジェクトを追加します。この平面がゲームの地面となります。PlaneのTransformの座標を(0,0,0)に設定して、中心に配置します。
-  Levelオブジェクトの下にDirectionalLightを移動します。

>Folderの階層下に配置したオブジェクトはゲームのビルド時（あるいはゲームの再生時）にルートに再配置されます。

## 2.Playerの作成

- ヒエラルキーで右クリックし、Create Emptyを選択して新しいGameObjectを作成します。このGameObjectの名前をPlayerに変更し、Transformの座標を(0,0,0)に設定します。
-  Playerオブジェクトの子として、Capsuleオブジェクトを作成し、これをキャラクターのモデルとして使用します。CapsuleのTransformの座標を(0, 1, 0)に設定して、地面から適切な高さに配置します。
-  同様に、Playerオブジェクトの子としてSphereオブジェクトを追加し、これをキャラクターの頭部または視覚的なアクセントとして使用します。SphereのTransformの座標を(0, 1.4, 0.5)に設定します。
-  次に、プレイヤーの入力を処理するために、PlayerにPlayerInputコンポーネントを追加します。Create Actionsボタンを押して新しいInputActionAssetを作成し、プレイヤーの操作を設定します。
-  CharacterBrainコンポーネントを追加します。これによりTCCのベクトルの管理対象をCharacterControllerとします。
-  キャラクターのサイズや設定を管理するためのCharacterSettingsコンポーネントを選択し、Radiusを0.6、Heightを2に設定して、キャラクターの形状に合わせます。

## 3.キャラクターに移動処理を追加

最も単純なキャラクターの移動処理を追加します。

-  PlayerにMoveControlスクリプトを追加します。これは、プレイヤーの移動を制御するカスタムスクリプトです。
-  ScriptMachineコンポーネントをPlayerに追加し、このコンポーネントでEdit Graphボタンを押して、ビジュアルスクリプティング環境を開きます。
-  グラフビュー内で、プレイヤーの入力に応じて移動処理を行うためのノードを配置し設定します。具体的には、OnInputSystemEventVector2ノードを使用して入力イベントを捉え、それをMoveControlの移動ロジックに接続します。
    -  OnInputSystemEventVector2ノードを追加。動作のタイミングはOnHold、InputActionはMoveを指定。
    -  MoveControlのMoveを追加し、OnInputSystemEventVector2のタイミングで動作するように接続。また入力結果も接続する。
    -  OnInputSystemEventVector2ノードを追加。動作のタイミングはOnRelease、InputActionはMoveを指定。
    -  同様に、MoveControlのMoveを追加し追加したノードに接続。

>Tips : キャラクターの移動方向はスクリーンの方向を参考に変化します。

## 4.重力の追加

キャラクターに重力などのエフェクトを追加します。

-  PlayerにGravityを追加します。
-  Gravityの地面との衝突判定で利用するために、GroundCheckまたはGroundHeightCheckスクリプトをPlayerに追加します。
    -  GroundCheck : Physicsを利用した地面との接地判定を行う。
    -  GroundHeightCheck : 座標と想定する地面の高さで地面との接地判定を行う。

>Tips : GroundCheckではなくGroundHeightCheckを使用した場合、地形の外に移動しても落下することはありません。
    
## 5.ジャンプの処理を追加

Move以外のControlを追加します。

-  プレイヤーがジャンプできるようにするため、PlayerにJumpControlスクリプトを追加します。
-  頭上の障害物の検出にはHeadContactCheckまたはReachHeightCheckスクリプトを使用します。
-  グラフビュー内で、プレイヤーの入力に応じて移動処理を行うためのノードを配置し設定します。具体的には、OnInputSystemEventButtonノードを使用して入力イベントを捉え、それをJumpControlのロジックに接続します。
    -  OnInputSystemEventButtonノードを追加。動作のタイミングはOnPressed、InputActionはFireを指定。
    -  追加したノードにJumpControlのJumpを設定。

>Tips : TCCはオールインワンの全てが揃う機能ではなく、組み合わせて挙動を実装します。

## 6.常に特定のオブジェクトを向く処理の追加

シーン内にTargetオブジェクトを追加し、プレイヤーが常にこのオブジェクトを向くようにします。

-  Sphereオブジェクトを追加し、Transformコンポーネントの座標に(0, 0.5, 3)を指定します。またSphereColliderコンポーネントのIsTriggerにチェックを入れ、Radiusを5に設定します。
-  PlayerにLookTargetControlコンポーネントを指定し、LookTargetControlコンポーネントのTargetに、先ほど作成したSphereを設定します。
-  LookTargetControlのプライオリティを10に設定します。

>Tips : プライオリティを利用してキャラクターの移動量、もしくは向きを上書きします。

## 7.近づいた時だけ特定のオブジェクトを向く処理の追加

プレイヤーがTargetオブジェクトの近くにいる時だけ（Triggerに入った時のみ）向くようにする処理を追加します。

-  グラフビュー内で、プレイヤーの入力に応じて移動処理を行うためのノードを配置し設定します。具体的には、OnTriggerEnter/OnTriggerExitのタイミングで、LookTargetControlのプライオリティを変更します。
    -  OnTriggerEnterノードを追加。LookTargetControlのSetPriorityノードを追加しOnTriggerEnterノードと接続。SetPriorityの値は10とする。
    -  OnTriggerExitノードを追加。LookTargetControlのSetPriorityノードを追加しOnTriggerEnterノードと接続。SetPriorityの値は0とする。

>Tips : プライオリティを切り替えてキャラクターの取るべき挙動を切り替えます。