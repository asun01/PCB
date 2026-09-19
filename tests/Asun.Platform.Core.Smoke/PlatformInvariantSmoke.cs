using Asun.Platform.Core;

public static class PlatformInvariantSmoke
{
    public static async ValueTask RunAsync(Action<bool, string> assert)
    {
        var signal = new AsyncSignal();
        var timedOut = false;

        try
        {
            await signal.WaitAsync(TimeSpan.FromMilliseconds(5));
        }
        catch (TimeoutException)
        {
            timedOut = true;
        }

        assert(
            timedOut &&
            !signal.IsSignaled,
            "AsyncSignal timeout should surface TimeoutException without changing the signal state.");

        using var signalCancellation = new CancellationTokenSource();
        signalCancellation.Cancel();

        var cancelled = false;
        try
        {
            await signal.WaitAsync(signalCancellation.Token);
        }
        catch (OperationCanceledException)
        {
            cancelled = true;
        }

        assert(
            cancelled,
            "AsyncSignal cancellation should propagate OperationCanceledException.");

        var pipeline = new AsyncPipeline<object>(
            new[]
            {
                new AsyncPipeline<object>.Node(
                    "A",
                    (_, _) => ValueTask.CompletedTask)
            });

        assert(
            !pipeline.ContainsNode(string.Empty) &&
            !pipeline.ContainsNode("Missing") &&
            pipeline.ContainsNode("A"),
            "AsyncPipeline node lookup should distinguish empty, unknown, and configured identifiers.");

        var duplicateDependencyRejected = false;

        try
        {
            _ = new AsyncPipeline<object>(
                new[]
                {
                    new AsyncPipeline<object>.Node(
                        "A",
                        (_, _) => ValueTask.CompletedTask),
                    new AsyncPipeline<object>.Node(
                        "B",
                        new[] { "A", "A" },
                        (_, _) => ValueTask.CompletedTask)
                });
        }
        catch (ArgumentException)
        {
            duplicateDependencyRejected = true;
        }

        assert(
            duplicateDependencyRejected,
            "AsyncPipeline should reject duplicate dependencies structurally.");

        var unknownDependencyRejected = false;

        try
        {
            _ = new AsyncPipeline<object>(
                new[]
                {
                    new AsyncPipeline<object>.Node(
                        "A",
                        new[] { "Missing" },
                        (_, _) => ValueTask.CompletedTask)
                });
        }
        catch (ArgumentException)
        {
            unknownDependencyRejected = true;
        }

        assert(
            unknownDependencyRejected,
            "AsyncPipeline should reject dependencies that do not name configured nodes.");

        var cycleRejected = false;

        try
        {
            _ = new AsyncPipeline<object>(
                new[]
                {
                    new AsyncPipeline<object>.Node(
                        "A",
                        new[] { "B" },
                        (_, _) => ValueTask.CompletedTask),
                    new AsyncPipeline<object>.Node(
                        "B",
                        new[] { "A" },
                        (_, _) => ValueTask.CompletedTask)
                });
        }
        catch (ArgumentException)
        {
            cycleRejected = true;
        }

        assert(
            cycleRejected,
            "AsyncPipeline should reject cyclic dependency graphs.");

        var queue = new BoundedWorkQueue<int>(4);
        queue.TryEnqueue(1);
        queue.TryEnqueue(2);
        queue.TryEnqueue(3);

        var drained = new List<int>();
        var drainedCount = queue.DrainTo(drained, 2);

        assert(
            drainedCount == 2 &&
            drained.SequenceEqual(new[] { 1, 2 }) &&
            queue.TryDequeue(out var tail) &&
            tail == 3,
            "BoundedWorkQueue DrainTo should preserve FIFO order and leave remaining work available.");

        using var resources = new ResourceLeasePool<string>(
            new[]
            {
                new KeyValuePair<string, int>("camera", 1)
            });

        AssertAcquire(resources, assert);

        var samples = new[] { 5d, 1d, 3d, 2d };
        var original = samples.ToArray();
        _ = Percentiles.Calculate(samples, 50);

        assert(
            samples.SequenceEqual(original),
            "Percentile calculation must never mutate caller-owned sample ordering.");

        var statistics = new RunningStatistics();
        var nonFiniteRejected = false;

        try
        {
            statistics.Add(double.NaN);
        }
        catch (ArgumentOutOfRangeException)
        {
            nonFiniteRejected = true;
        }

        assert(
            nonFiniteRejected &&
            statistics.Count == 0,
            "RunningStatistics should reject non-finite samples without mutating accumulated state.");

        var timeoutOperationCancelled = false;
        using var callerCancellation = new CancellationTokenSource();
        callerCancellation.Cancel();

        try
        {
            await OperationTimeout.ExecuteAsync(
                async token =>
                {
                    await Task.Delay(1, token);
                    return 42;
                },
                TimeSpan.FromSeconds(1),
                callerCancellation.Token);
        }
        catch (OperationCanceledException)
        {
            timeoutOperationCancelled = true;
        }

        assert(
            timeoutOperationCancelled,
            "OperationTimeout should preserve caller cancellation as cancellation rather than misclassifying it as a timeout.");
    }

    private static void AssertAcquire(
        ResourceLeasePool<string> resources,
        Action<bool, string> assert)
    {
        var acquired = resources.TryAcquire(
            "camera",
            out var lease);

        assert(
            acquired &&
            lease is not null &&
            resources.Available("camera") == 0,
            "ResourceLeasePool should reduce availability when a lease is acquired.");

        lease!.Dispose();
        lease.Dispose();

        assert(
            lease.IsDisposed &&
            resources.Available("camera") == 1,
            "Disposing the same resource lease twice must release capacity only once.");
    }
}
