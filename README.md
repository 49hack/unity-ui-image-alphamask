Unity-UI-Image-AlphaMask
===
UnityのuGUI ImageにAlphaMaskを適用するための実装です。
![Unity](https://unity3d.com/profiles/unity3d/themes/unity/images/company/brand/logos/primary/unity-master-black.svg "Unity logo")

## 概要
Unity標準のuGUI-Maskコンポーネントは、ステンシルを使って画像をクリップするため境目にジャギーが出てしまいます。
Unity-UI-Image-AlphaMaskは、マスク画像のアルファ値を使って画像をクリップすることで、境目のアルファフェードを実現し、ジャギーのないマスク表現を行うためのコンポーネントです。
<img width="647" alt="2018-07-28 17 58 50" src="https://user-images.githubusercontent.com/30557808/43354909-aedc4ad8-928e-11e8-9e66-527ef830b7bd.png">

## 開発バージョン
Unity2017.3.1f1

## 導入方法
1. Assets/UI-AlphaMask/フォルダをプロジェクト内の任意の場所へコピーしてください。
2. Mask画像をセットしたImageを作成して、UIImageAlphaMaskコンポーネントをアタッチしてください。
3. 2.で作成したマスクオブジェクトの下に、マスクを適用したいImageを作成します。

## 制限事項
このコンポーネントには、以下の制限があります。

* CanvasのRenderModeがScreenSpaceCamera/WorldSpaceであること
* CanvasのworldCameraがOtrhographicであること
* Maskの回転、タイリング、9スライスは使用できない
* Mask画像はWrapModeをClampに設定し、画像端の1ピクセルをアルファ0で描くこと

