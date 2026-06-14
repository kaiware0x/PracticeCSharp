
async Task HeavyJob(CancellationToken ct)
{
    Console.WriteLine("Start HeavyJob");

    for (int i = 0; i < 10; i++)
    {
        if (ct.IsCancellationRequested)
        {
            Console.WriteLine("Cancelled");
            return;
        }

        Console.WriteLine($"Job {i + 1}");
        await Task.Delay(TimeSpan.FromSeconds(1));
    }
}

CancellationTokenSource cts = new();

var task = HeavyJob(cts.Token);

await Task.Delay(TimeSpan.FromSeconds(2));

Console.WriteLine("Call cancel");

cts.Cancel();

await task;

cts.Dispose();
