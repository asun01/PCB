using Asun.Platform.Core;

public static class ResourceLeasePoolValidationHundredStageSmoke
{
    public static async ValueTask RunAsync(Action<bool, string> assert)
    {
        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        using var pool = new ResourceLeasePool<string>(
            new[]
            {
                new KeyValuePair<string, int>("camera", 1),
                new KeyValuePair<string, int>("gpu", 2)
            });

        var initialValid = ResourceLeasePoolValidationRuntime.IsValid(pool);
        var acquired = pool.TryAcquire("camera", out var activeLease);

        var zeroTimeoutWithoutCapacity =
            await pool.AcquireAsync("camera", TimeSpan.Zero);

        activeLease!.Dispose();

        var zeroTimeoutWithCapacity =
            await pool.AcquireAsync("camera", TimeSpan.Zero);

        var zeroLeaseActive =
            zeroTimeoutWithCapacity is not null &&
            !zeroTimeoutWithCapacity.IsDisposed;

        zeroTimeoutWithCapacity!.Dispose();
        zeroTimeoutWithCapacity.Dispose();

        var cancelled = false;
        using (var cancellation = new CancellationTokenSource())
        {
            cancellation.Cancel();

            try
            {
                _ = await pool.AcquireAsync("gpu", cancellation.Token);
            }
            catch (OperationCanceledException)
            {
                cancelled = true;
            }
        }

        for (var i = 0; i < 10; i++)
            Check(initialValid, $"initial pool validation round {i + 1} should pass.");

        for (var i = 0; i < 10; i++)
            Check(acquired && activeLease is not null, $"first acquisition round {i + 1} should succeed.");

        for (var i = 0; i < 10; i++)
            Check(zeroTimeoutWithoutCapacity is null, $"zero-timeout saturated round {i + 1} should return null.");

        for (var i = 0; i < 10; i++)
            Check(zeroTimeoutWithCapacity is not null, $"zero-timeout available round {i + 1} should acquire immediately.");

        for (var i = 0; i < 10; i++)
            Check(zeroLeaseActive, $"zero-timeout lease activity round {i + 1} should be active.");

        for (var i = 0; i < 10; i++)
            Check(pool.Available("camera") == 1, $"released capacity round {i + 1} should be restored.");

        for (var i = 0; i < 10; i++)
            Check(zeroTimeoutWithCapacity!.IsDisposed, $"double lease disposal round {i + 1} should remain idempotent.");

        for (var i = 0; i < 10; i++)
            Check(cancelled, $"cancelled acquisition round {i + 1} should propagate cancellation.");

        for (var i = 0; i < 10; i++)
            Check(pool.ResourceCount == 2 && pool.Capacity("gpu") == 2, $"resource metadata round {i + 1} should remain stable.");

        for (var i = 0; i < 10; i++)
            Check(ResourceLeasePoolValidationRuntime.IsValid(pool), $"final pool validation round {i + 1} should pass.");

        assert(round == 100, $"Resource lease pool smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
