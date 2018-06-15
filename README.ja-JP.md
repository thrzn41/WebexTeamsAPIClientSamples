# Samples for Webex Teams API Client

`Samples for Webex Teams API Client`は、`Webex Teams API Client for .NET(Thrzn41.WebexTeams)`のサンプルです。

`Webex Teams API Client for .NET(Thrzn41.WebexTeams)`のリポジトリは、[こちら](https://github.com/thrzn41/WebexTeamsAPIClient)

`Thrzn41.WebexTeams` packageは、NuGetから入手できます:

[![nuget](https://img.shields.io/nuget/v/Thrzn41.WebexTeams.svg)](https://www.nuget.org/packages/Thrzn41.WebexTeams)

#### ほかの言語のREADME
* [English README is here](https://github.com/thrzn41/WebexTeamsAPIClientSamples/blob/master/README.md) ([英語のREADMEはこちら](https://github.com/thrzn41/WebexTeamsAPIClientSamples/blob/master/README.md))

---

## 現在のサンプル

他のサンプルを動かす前に、まず`S0010SetupSamples`を実行する必要があります。  
`S0010SetupSamples`は、Botトークンの暗号化と、サンプルのためのスペースを作成します。

| SampleのID | タイトル          | 概要 |
| :-------- | :------------- |:------------- |
| S0010     | サンプルをセットアップ  | サンプルセットアップのため、まずこれを動かす必要がある。 |
| S1010     | メッセージの投稿  | サンプルスペースにメッセージを投稿します。 |
| S1020     | 成功したかどうかの判定  | リクエストが成功したかどうかチェックします。また、TeamsResultExceptionを扱います。 |
| S1030     | Markdownビルダー  | Markdownビルダーの使い方。 |
| S1040     | ListResult Enumerator(Pagination機能)  | まず、最初のリストを取得して、その後、次のリストを取得します。 |
