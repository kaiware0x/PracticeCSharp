# WPF Practice

## 課題: ファイルダウンローダー風 UI

**テーマ**: 複数の「ダウンロードジョブ」を非同期で実行し、進捗をリアルタイムで表示する

## 要件

### 機能要件

- 複数のジョブ（例: 20件）を一覧表示する
- 各ジョブは以下の情報を持つ
  - ジョブ名
  - 進捗バー（0〜100%）
  - 状態（Waiting / Running / Done / Canceled）
- `Start All` ボタンで全ジョブを非同期で並列実行する
- `Cancel All` ボタンで実行中の全ジョブをキャンセルする
- 同時実行数は最大3件に制限する（SemaphoreSlim）
- 実行完了後、Success / Canceled の件数を集計して表示する

### 技術要件

- MVVM パターンで実装する
- ReactiveProperty（ReactivePropertySlim / ReactiveCommandSlim）を使用する
- 非同期処理には `async/await` + `CancellationToken` を使用する
- UI スレッドへの進捗反映には `IProgress<T>` または `ReactivePropertySlim` を使用する
- コードビハインド（xaml.cs）はできるだけ最小限にする

## 技術スタック

- .NET 10 / WPF
- ReactiveProperty 9.8.0
