
class Program
{
    static async Task Job1()
    {
        Console.WriteLine("Start Job1");
        await Task.Delay(TimeSpan.FromSeconds(2));
        Console.WriteLine("End Job1");
    }
    static async Task Job2(CancellationToken token)
    {
        Console.WriteLine("Start Job2");
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(5), token);
        }
        catch (OperationCanceledException e)
        {
            Console.WriteLine(e.Message);
            return;
        }
        Console.WriteLine("End Job2");
    }

    static async Task Job3()
    {
        Console.WriteLine("Start Job3");
        await Task.Delay(TimeSpan.FromSeconds(2));
        throw new Exception("Some error occurred.");
    }

    static async Task Main()
    {
        // 正常終了 パターン
        var task1 = Job1();
        await task1.WaitAsync(TimeSpan.FromSeconds(3));
        Console.WriteLine("Success");

        // タイムアウトパターン
        CancellationTokenSource cts = new();
        var task2 = Job2(cts.Token);
        try
        {
            await task2.WaitAsync(TimeSpan.FromSeconds(2));
        }
        catch (TimeoutException e)
        {
            cts.Cancel();
            Console.WriteLine($"Timeout: {e.Message}");
        }

        // 例外発生パターン
        var task3 = Job3();
        try
        {
            await task3;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
