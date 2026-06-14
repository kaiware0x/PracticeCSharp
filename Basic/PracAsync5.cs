

enum ReturnCode
{
    Success,
    Failed,
    Canceled
}

class Program
{
    private static readonly SemaphoreSlim _semaphore = new(3, 3);

    private static int CurrentWorkers => 3 - _semaphore.CurrentCount;

    static async Task Main()
    {
        CancellationTokenSource cts = new();
        const int TaskNum = 20;
        var tasks = new Task<ReturnCode>[TaskNum];

        for (int taskId = 0; taskId < TaskNum; taskId++)
        {
            tasks[taskId] = Job(taskId, cts.Token);
        }

        await Task.Delay(5000);
        cts.Cancel();

        var results = await Task.WhenAll(tasks);

        var successCount = results.Count(x => x == ReturnCode.Success);
        var failedCount = results.Count(x => x == ReturnCode.Failed);
        var canceledCount = results.Count(x => x == ReturnCode.Canceled);

        Console.WriteLine($"Success: {successCount}, Failed: {failedCount}, Canceled: {canceledCount}");
    }

    static async Task<ReturnCode> Job(int taskId, CancellationToken token)
    {
        Console.WriteLine($"Submit: {taskId}");

        await _semaphore.WaitAsync();

        var returnCode = ReturnCode.Success;

        try
        {
            Console.WriteLine($"Working... {taskId}, Current workers: {CurrentWorkers}");
            await Task.Delay(2000, token);
            if (taskId % 5 == 0)
            {
                throw new Exception("Fail");
            }
        }
        catch (OperationCanceledException e)
        {
            Console.WriteLine($"Canceled {taskId}: {e.Message}");
            returnCode = ReturnCode.Canceled;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception {taskId}: {e.Message}");
            returnCode = ReturnCode.Failed;
        }
        finally
        {
            Console.WriteLine($"End: {taskId}");
            _semaphore.Release();
        }

        return returnCode;
    }
}
