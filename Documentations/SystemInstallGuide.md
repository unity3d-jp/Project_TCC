# Project_TCCシステムインストールガイド

Project_TCCに含まれるシステムを、ご自身のUnityプロジェクトへインストールするための手順を説明します。

## インストール手順

1. **Project_TCCをダウンロード**  
   `Project_TCC`を[Zip形式でダウンロード](https://github.com/unity3d-jp/Project_TCC/archive/refs/heads/main.zip)します。

2. **Zipフォルダを解凍**  
   ダウンロードしたZipフォルダを解凍します。

3. **パッケージをプロジェクトに配置**  
   解凍したフォルダから以下のパッケージを、自身のプロジェクトの`Packages`フォルダ以下に配置します。（あるいは、パッケージを参照します。）
   - `com.utj.charactercontroller`（キャラクター制御）
   - `com.utj.gameobjectfolder`（フォルダとして扱えるGameObject）
   - `com.utj.savedata`（ゲームの進行データを保存する）
   - `com.utj.scenarioimporter`（テキストからゲームで扱いやすい台本を抽出する）
   - `com.utj.sceneloader`（シーンをGameObjectのように読込・解放する）

4. **Unityエディターの再起動**  
   インポート完了後、Unityエディターを再起動します。  
   **注意:** インポート直後は`UIElements`などの項目でエラーが発生することがあります。例えば、`TypeLoadException: Could not load type 'Utj.SceneManagement.SceneLoadType' from assembly 'GOSubSceneEditor'.` などですが、このエラーは次回起動以降は発生しませんので、無視してかまいません。

## Visual Scriptingでの使用

Visual Scriptingを使用する場合の設定手順です。

1. **Visual Scriptingの初期化**  
   - メニューから`Edit > Project Settings > Visual Scripting`に進みます。
   - `Visual Scripting`タブを開き、`Initialize Visual Scripting`ボタンを押します。

   ![SysInst_Image01.png](./Images/SysInst_Image01.png "SysInst_Image01")

2. **パッケージが使用するコンポーネントを使用可能にする**  
   - `Generate Nodes`メニューから`Node Library`を開きます。

   ![SysInst_Image02.png](./Images/SysInst_Image02.png "SysInst_Image02")


   - 右端の「＋」ボタンで新規スロットを追加し、以下の各アセンブリを登録します。
     - `Utj.SceneLoader`
     - `Utj.GameDataSave`
     - `Utj.ScenarioImporter`
     - `Utj.TinyCharacterController`

    ![SysInst_Image03.png](./Images/SysInst_Image03.png "SysInst_Image03")


## SceneLoaderの使用

SceneLoaderを使用する前に、Addressableが初期化されている必要があります。  
**注意:** Addressableが初期化されていない場合、SceneLoaderにシーンを登録する事ができません。

1. **Addressableの有効化**  
   - メニューから`Window > Asset Management > Addressable > Group`を開きます。
   - `Create Addressable Settings`ボタンを押してAddressableを有効にします。

   ![SysInst_Image04.png](./Images/SysInst_Image04.png "SysInst_Image04")

---
