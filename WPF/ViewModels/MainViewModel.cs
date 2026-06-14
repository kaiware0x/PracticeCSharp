
using System.Collections.ObjectModel;
using Reactive.Bindings;

class MainViewModel
{
    // ジョブ一覧
    // ObservableCollection<T> は 要素の追加、削除を View に自動通知してくれる
    public ObservableCollection<JobItem> Jobs { get; }
    // Start All ボタン
    public ReactiveCommandSlim StartCommand { get; }
    // Cancel All ボタン
    public ReactiveCommandSlim CancelCommand { get; }
    // 集計結果の表示
    public ReactivePropertySlim<string> ResultMessage { get; } = new();

    private CancellationTokenSource _cts;

    private SemaphoreSlim _semaphoreSlim = new(5, 5);

    public MainViewModel()
    {
        Jobs = new();
        for (int i = 0; i < 10; i++)
        {
            Jobs.Add(new JobItem($"Job{i + 1}"));
        }

        StartCommand = new();
        var a = StartCommand.WithSubscribe(async (_) =>
            {
                if (_cts != null)
                {
                    _cts.Dispose();
                }
                _cts = new();

                List<Task> tasks = new();
                foreach (var jobItem in Jobs)
                {
                    var t = jobItem.RunAsync(_cts.Token, _semaphoreSlim);
                    tasks.Add(t);
                }

                try
                {
                    await Task.WhenAll(tasks);
                }
                catch (OperationCanceledException e)
                {
                    Console.WriteLine($"Canceled: {e.Message}");
                }

                int doneCount = Jobs.Count(x => x.Status.Value == JobStatus.Done);
                int canceledCount = Jobs.Count(x => x.Status.Value == JobStatus.Canceled);
                ResultMessage.Value = $"Done: {doneCount}, Canceled: {canceledCount}";
            }
        );

        CancelCommand = new();
        CancelCommand.WithSubscribe(_ =>
            {
                if (_cts != null)
                {
                    _cts.Cancel();
                }
            }
        );
    }
}
