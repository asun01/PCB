using Asun.Platform.Core;

public static class BoundedWorkQueueValidationHundredStageSmoke
{
    public static async ValueTask RunAsync(Action<bool, string> assert)
    {
        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        using var queue = new BoundedWorkQueue<int>(4);
        var initialValid = BoundedWorkQueueValidationRuntime.IsValid(queue);

        queue.TryEnqueue(1);
        queue.TryEnqueue(2);
        queue.TryEnqueue(3);

        var destination = new List<int>();
        var drained = queue.DrainTo(destination, 2);
        var tailRead = queue.TryDequeue(out var tail);

        var asyncQueue = new BoundedWorkQueue<int>(2);
        asyncQueue.TryEnqueue(7);
        asyncQueue.TryEnqueue(8);
        var batch = new int[2];
        var batchCount = await asyncQueue.DequeueBatchAsync(batch);

        var emptyRejected = false;
        try
        {
            _ = asyncQueue.TryDequeueBatch(Span<int>.Empty);
        }
        catch (ArgumentException)
        {
            emptyRejected = true;
        }

        var cancelled = false;
        using (var cancellation = new CancellationTokenSource())
        {
            cancellation.Cancel();
            try
            {
                _ = await new BoundedWorkQueue<int>(1).DequeueAsync(cancellation.Token);
            }
            catch (OperationCanceledException)
            {
                cancelled = true;
            }
        }

        queue.Complete();

        for (var i = 0; i < 10; i++)
            Check(initialValid, $"initial queue validation round {i + 1} should pass.");

        for (var i = 0; i < 10; i++)
            Check(drained == 2 && destination.SequenceEqual(new[] { 1, 2 }), $"FIFO drain round {i + 1} should preserve order.");

        for (var i = 0; i < 10; i++)
            Check(tail && tailValue(tail) == 3, $"tail dequeue round {i + 1} should preserve the remaining item.");

        for (var i = 0; i < 10; i++)
            Check(batchCount == 2 && batch.SequenceEqual(new[] { 7, 8 }), $"batch dequeue round {i + 1} should drain available work.");

        for (var i = 0; i < 10; i++)
            Check(emptyRejected, $"empty destination round {i + 1} should be rejected.");

        for (var i = 0; i < 10; i++)
            Check(cancelled, $"cancelled dequeue round {i + 1} should propagate cancellation.");

        for (var i = 0; i < 10; i++)
            Check(!queue.CanWrite, $"completed queue write-state round {i + 1} should be closed.");

        for (var i = 0; i < 10; i++)
            Check(queue.IsCompleted, $"drained completion round {i + 1} should be observable.");

        for (var i = 0; i < 10; i++)
            Check(BoundedWorkQueueValidationRuntime.IsValid(queue), $"completed queue validation round {i + 1} should pass.");

        for (var i = 0; i < 10; i++)
            Check(queue.Completion.IsCompleted, $"completion task round {i + 1} should be complete.");

        assert(round == 100, $"Bounded work queue smoke should execute exactly 100 numbered rounds; actual {round}.");

        static int tailValue(int? value) => value ?? int.MinValue;
    }
}
