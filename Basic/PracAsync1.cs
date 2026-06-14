using System.Diagnostics;

void SyncJob(string name)
{
    Console.WriteLine($"SyncJob {name} start");
    Thread.Sleep(TimeSpan.FromSeconds(2));
    Console.WriteLine($"SyncJob {name} end");
}

Stopwatch stopwatch = new();

stopwatch.Start();

SyncJob("job 1");
SyncJob("job 2");
SyncJob("job 3");

stopwatch.Stop();

Console.WriteLine($"Sync elapsed time: {stopwatch.Elapsed}");

//@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
// Async
//@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

async Task AsyncJob(string name)
{
    Console.WriteLine($"AsyncJob {name} start");
    await Task.Delay(TimeSpan.FromSeconds(2));
    Console.WriteLine($"AsyncJob {name} end");
}

stopwatch.Restart();

var task1 = AsyncJob("job 1");
var task2 = AsyncJob("job 2");
var task3 = AsyncJob("job 3");

await Task.WhenAll([task1, task2, task3]);

stopwatch.Stop();

Console.WriteLine($"Async elapsed time: {stopwatch.Elapsed}");
