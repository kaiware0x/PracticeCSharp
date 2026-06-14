using System;
using System.Security.Cryptography;
using Reactive.Bindings;

enum JobStatus
{
    Waiting,
    Running,
    Done,
    Canceled
}

class ScopeExitAction : IDisposable
{
    private readonly Action _action;

    public ScopeExitAction(Action action) => _action = action;

    // スコープを抜けた瞬間に、C#のシステムがこのメソッドを「絶対に」呼んでくれる
    public void Dispose() => _action?.Invoke();
}

class JobItem
{
    public string Name { get; set; }

    public ReactivePropertySlim<int> Progress { get; }

    public ReactivePropertySlim<JobStatus> Status { get; }

    public JobItem(string name)
    {
        Name = name;
        Progress = new(0);
        Status = new(JobStatus.Waiting);
    }

    public async Task RunAsync(CancellationToken token, SemaphoreSlim semaphore)
    {
        if (Status.Value == JobStatus.Running)
        {
            return;
        }

        Status.Value = JobStatus.Waiting;
        Progress.Value = 0;

        await semaphore.WaitAsync();

        // この関数を抜けたら自動で Release されるようにする
        using var _ = new ScopeExitAction(() => { semaphore.Release(); });

        Status.Value = JobStatus.Running;

        for (int i = 0; i < 100; i++)
        {
            try
            {
                var delayTimeMs = RandomNumberGenerator.GetInt32(10, 100);
                await Task.Delay(delayTimeMs, token);
            }
            catch (OperationCanceledException)
            {
                Status.Value = JobStatus.Canceled;
                Progress.Value = 0;
                throw;
            }

            Progress.Value = i + 1;
        }

        Status.Value = JobStatus.Done;
    }
}
