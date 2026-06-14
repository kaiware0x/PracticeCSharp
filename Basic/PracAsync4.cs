

class Program
{
    private static SemaphoreSlim _semaphore = new(3, 3);

    private static int CurrentWorkers => 3 - _semaphore.CurrentCount;

    static async Task Main()
    {
        const int TaskNum = 20;
        var tasks = new Task[TaskNum];

        for (int taskId = 0; taskId < TaskNum; taskId++)
        {
            tasks[taskId] = HeavyJob(taskId);
        }

        await Task.WhenAll(tasks);

        _semaphore.Dispose();
    }

    static async Task HeavyJob(int taskId)
    {
        Console.WriteLine($"Submit {taskId}");

        await _semaphore.WaitAsync();

        try
        {
            Console.WriteLine($"Working... {taskId}, Current workers: {CurrentWorkers}");
            await Task.Delay(2000);
        }
        finally
        {
            Console.WriteLine($"End {taskId}");
            _semaphore.Release();
        }
    }
}
