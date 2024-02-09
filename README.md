# Project_TCC v.1.0.0
TCC stands for **Tiny Character Controller**. TCC provides a comprehensive solution for creating your own game.

This repository contains all packages and examples for TCC projects.

Project_TCC is the last Unity-Chan! project. Enjoy yourselves!

## 2024/02/09: Information and disclaimer
We appreciate your interest in Project_TCC.

This project and repository "Project_TCC" is provided as-is, without any maintenance or release plan.

Therefore, we are unable to monitor bug reports, accept feature requests, or review pull requests for this package.

However, we recognize that some users may wish to continue utilizing and enhancing Project_TCC. In that case, we recommend that you fork the repository. This will allow you to make changes and enhancements as you see fit.

## Release Information
* 2024/02/09 : Project_TCC v.1.0.0 : First Release.


## License Notice
* Project_TCC is licensed by Unity Companion License v.1.3
https://unity.com/ja/legal/licenses/unity-companion-license.

* All character assets "Unity-Chan!" are licensed by Unity-Chan License 2.0(UCL 2.0)
https://unity-chan.com/contents/guideline/
https://unity-chan.com/contents/guideline_en/

* Some sound effects data that contains in this project is made by GameSynth.
The official GameSynth URL is below:
http://tsugi-studio.com/web/jp/products-gamesynth.html

* This project contains the font 'Source Han Sans' font. This font is licensed by Adobe and SIL OPEN FONT LICENSE v.1.1
https://github.com/adobe-fonts/source-han-sans?tab=License-1-ov-file#License-1-ov-file


---
# Project TCC 解説ドキュメント

## 概要

* Tiny Character Controller (TCC) は、キャラクターの挙動を複数の小さなコンポーネントを組み合わせて実現するシステムです。

* TCCでは、キャラクターの移動、ジャンプ、カメラ制御などの基本動作をはじめとした多様な機能を提供し、柔軟なキャラクター表現を実現します。各コンポーネントが持つプライオリティ（優先順位）を設定することで、応用の効くキャラクター制御を可能にします。

* TCCは外部からの入力に対応し、Visual Scriptingからの制御もサポートしています。

* また、ゲーム開発を支援するための様々な補助機能も提供しており、UI表示、オブジェクトプール、シナリオインポーター、ゲームデータの保存、シーンの切り替えを簡単かつ安全に行い、シーンのレイヤー化や段階的なロードを容易にするシーンローダーなどのユーティリティが含まれます。

## TCCの基本概念

TCCには、キャラクターを制御するための4つの基本的なコンポーネントがあります。

1. **Brain**
   - キャラクターの最終的な座標を更新するコンポーネントです。
   - Check、Effect、Controlの結果を集約し、Transformに書き込みます。
   - 移動・ベクトル計算・センサー処理を集中管理します。

2. **Check**
   - 周囲の情報を収集するセンサーコンポーネントです。
   - 地面の接地判定、頭上の接触判定、視界判定などを行います。
   - 更新時に値をキャッシュし、他コンポーネントへ処理結果を提供します。

3. **Control**
   - プレイヤーの入力に応じてキャラクターの動きを制御するコンポーネントです。
   - 移動、ジャンプ、カメラ制御などのキャラクター操作を管理します。
   - 移動方向や移動速度、ジャンプの高さなどを調整します。

4. **Effect**
   - キャラクターに追加の動きや影響を与えるコンポーネントです。
   - 重力、プラットフォームとの相互作用、追加力などを扱います。
   - キャラクターの動きにバリエーションやリアリズムを加える働きをします。

これらのコンポーネントを組み合わせることで、複雑なキャラクター挙動を簡単に構築し、カスタマイズすることが可能となっています。

各コンポーネントは以下のようなネットワークで、キャラクター制御に必要な情報を収集し、座標や動作を更新する処理命令を通知しています。

![TccBasic4Components.png](./Documentations/Images/TccBasic4Components.png "TccBasic4Components")

またControl系のコンポーネントには、各コンポーネントごとにそのキャラクターの位置（Transform）や向き（Rotation）変化に与える影響の優先度を設定する、プライオリティがありますので、1体のキャラクターゲームオブジェクトに複数のControlをアタッチし、状況に応じて切り替えることが可能です。


## Project_TCC に含まれる、様々な機能コンポーネント群

Project_TCCに含まれる主要なコンポーネント群について、その概要と主なコンポーネント名および機能を以下にまとめます。いずれもがゲーム制作に便利なコンポーネントとなっています。

なお全てのRuntimeコンポーネントのリストは、**【プロジェクト内のドキュメントセクション（[日本語](./Documentations/Componentlist_ja.md) / [English](./Documentations/Componentlist_en.md)）】** にあります。


---
### 1. Brain
**概要:** 
- TCCのControlやEffectなどからの情報を集約し、キャラクターの座標を更新するコンポーネント群です。

**代表的なコンポーネント:**

- **CharacterBrain**: CharacterControllerを使用して座標を更新。アクションゲームに適しており、CharacterControllerの機能を利用します。
- **RigidbodyBrain**: Rigidbodyを使用して座標を更新し、物理演算に基づいた動作を行います。物理演算が必要なゲームに適しています。
- **NavmeshBrain**: NavmeshAgentを利用して座標を更新し、RPGなどNavmeshで移動範囲を制限するゲームに適しています。
- **TransformBrain**: Transformを使用してキャラクターの座標を更新。PhysicsやColliderの判定は行わず、主にストラテジーゲームやインタラクションの少ないゲームで使用。


---
### 2. Check
**概要:** 
- キャラクターの周辺情報を収集するセンサーコンポーネント群です。

**代表的なコンポーネント:**
- **Ground Check**: 地面との接地判定を行います。地面までの距離や勾配、地面オブジェクトの種類などを取得できます。
- **Head Collision Check**: 頭上のオブジェクトとの接触判定を行います。キャラクターの高さを考慮した頭上判定を行い、接触時にイベントを発火します。

---
### 3. Control
**概要:** 
- キャラクターの移動や向きを制御するコンポーネント群です。

**代表的なコンポーネント:**
- **Move Control**: キャラクターの地上移動を制御します。スクリーン上の指定した方向に移動させることができます。
- **Ladder Move Control**: はしごを登る処理を実現します。Ladderコンポーネントで指定した移動範囲に対してキャラクターを移動させます。

---
### 4. Effect
**概要:** 
- キャラクターに追加の動きや影響を与えるコンポーネント群です。

**代表的なコンポーネント:**
- **Gravity**: キャラクターに重力加速度による落下を追加します。空中にいる間は下方向に加速し、地面に接触すると加速度が0になります。
- **MoveWithPlatform**: 移動する地形にキャラクターを追随させます。MovePlatformコンポーネントを持つ地形の上にいるとき、その地形の加速度をキャラクターに加算します。

これらのコンポーネント群によって、TCCは複雑なキャラクターの動きを簡単に構築し、カスタマイズすることが可能になります。

---
### 5. Utility
**概要:** 
- ゲーム開発における一般的なニーズに応えるための補助的な機能やコンポーネントを提供します。

**代表的なコンポーネント:**
- **Swing Hit Detector**: スイングや攻撃などのアクションによる移動判定と、他のColliderとの接触判定を検出し通知するコンポーネントです。特定の範囲内での判定や、接触ポイントの移動経路での接触判定を行い、接触があった場合にイベントを発火します。
- **Sequential Collision Detector**: 複数の当たり判定を持ち、これらを段階的に有効にすることで、他のオブジェクトとの接触判定を行うコンポーネントです。特定のタイミングでのみ当たり判定を有効化し、モーションに合わせた攻撃範囲の設定などに利用されます。
- **GameObjectPool**: 特定のPrefabに基づいてGameObjectをプールし管理するコンポーネントです。プールからオブジェクトを取得し、利用後にはプールに戻すことで、オブジェクトの生成と破棄のコストを削減します。

- **Indicator**: ゲームやアプリケーション内で特定のターゲットを画面上で視覚的に追跡するためのユーティリティを提供します。ターゲットの位置を表すアイコンをUI要素として表示し、ターゲットが画面内にあるか画面外にあるかに基づいてアイコンの位置と状態を調整します。

- **IndicatorPin**: UIを3D空間の座標と同期させるためのコンポーネントです。指定された座標にUIの位置を合わせて調整します。

### その他の特徴的な機能
- **Scene Loader**: 指定されたアセットシーンを読み込むためのコンポーネントです。このコンポーネントは、シーンの読み込みとアンロードの管理、読み込み進捗の追跡、およびシーン読み込み完了時やアンロード時のイベント通知を提供します。

- **Game Object Folder**: Unity エディタ内でゲームオブジェクトの階層構造を整理するためのコンポーネントです。このコンポーネントは、階層内のゲームオブジェクトを「フォルダ」として扱い、整理やコメントの追加を容易にします。

- **Save System**: 同一 GameObject 及びその子 GameObject に設定された `IDataContainer` コンポーネントを取得し、単一の Json として保存または読み込むためのコンポーネントです。


---
これらのコンポーネント群を組み合わせることで、複雑なキャラクターの挙動や、様々なゲームシステムを柔軟に実装することが可能になります。TCCは、これらの基本コンポーネントをベースとして、開発者が独自の挙動や機能を追加しやすい構造になっている点も大きな特徴です。

