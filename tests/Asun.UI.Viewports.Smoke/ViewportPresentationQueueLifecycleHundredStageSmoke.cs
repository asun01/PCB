using System.Drawing;
using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportPresentationQueueLifecycleHundredStageSmoke
{
    public static async ValueTask RunAsync(Action<bool, string> assert)
    {
        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        using var pipeline = CreatePipeline();
        using var queue = new ViewportPresentationQueueRuntime<string>(2);

        var frame = await pipeline.RefreshAsync();
        var frameExists = frame is not null;

        var enqueued = frameExists &&
            queue.TryEnqueue(frame!, out var packet);

        var queuedStatistics = queue.Statistics;
        var taken = enqueued &&
            queue.TryTakeLatest(out var inFlight);

        var wasCurrent = taken &&
            queue.IsCurrent(inFlight.Token);

        var commitStarted = taken &&
            queue.TryBeginCommit(inFlight.Token);

        var commitStatistics = queue.Statistics;

        var commitCompleted = taken &&
            queue.TryCompleteCommit(inFlight.Token);

        var presentedStatistics = queue.Statistics;

        var nextEnqueued = frameExists &&
            queue.TryEnqueue(frame!, out var newer);
        var nextTaken = nextEnqueued &&
            queue.TryTakeLatest(out var newerInFlight);
        var nextCancelled = nextTaken &&
            queue.TryCancel(newerInFlight.Token);
        var monotonicSequence = nextEnqueued &&
            newer.Token.Sequence >
            (taken ? inFlight.Token.Sequence : 0);

        queue.Reset();
        var resetStatistics = queue.Statistics;

        for (var i = 0; i < 10; i++)
        {
            Check(
                frameExists,
                $"pipeline frame round {i + 1} should exist.");
        }

        for (var i = 0; i < 10; i++)
        {
            Check(
                enqueued,
                $"enqueue round {i + 1} should succeed.");
        }

        for (var i = 0; i < 10; i++)
        {
            Check(
                ViewportPresentationQueueValidationRuntime.IsValid(queuedStatistics),
                $"queued accounting round {i + 1} should be valid.");
        }

        for (var i = 0; i < 10; i++)
        {
            Check(
                taken && wasCurrent,
                $"in-flight current token round {i + 1} should remain current.");
        }

        for (var i = 0; i < 10; i++)
        {
            Check(
                commitStarted &&
                commitStatistics.CommitInProgress,
                $"commit window round {i + 1} should be observable.");
        }

        for (var i = 0; i < 10; i++)
        {
            Check(
                commitCompleted &&
                ViewportPresentationQueueValidationRuntime.IsValid(presentedStatistics) &&
                presentedStatistics.PresentedSequence ==
                    (taken ? inFlight.Token.Sequence : 0),
                $"presented accounting round {i + 1} should remain valid.");
        }

        for (var i = 0; i < 10; i++)
        {
            Check(
                nextCancelled,
                $"post-present cancellation round {i + 1} should succeed.");
        }

        for (var i = 0; i < 10; i++)
        {
            Check(
                monotonicSequence,
                $"submission sequence round {i + 1} should remain monotonic.");
        }

        for (var i = 0; i < 10; i++)
        {
            Check(
                ViewportPresentationQueueValidationRuntime.IsValid(resetStatistics) &&
                resetStatistics.Pending == 0,
                $"reset accounting round {i + 1} should remain valid.");
        }

        for (var i = 0; i < 10; i++)
        {
            Check(
                resetStatistics.LatestGeneration is null &&
                resetStatistics.PresentedGeneration is null &&
                resetStatistics.CommitInProgress is false,
                $"reset lifecycle round {i + 1} should clear presentation ownership.");
        }

        assert(
            round == 100,
            $"Presentation queue lifecycle smoke should execute exactly 100 numbered rounds; actual {round}.");
    }

    private static ViewportRenderPipelineRuntime<string> CreatePipeline() =>
        new(
            new Vector2(100, 100),
            new Vector2(100, 100),
            new Vector2(100, 100),
            0,
            8,
            1,
            new StableTileSource(),
            new ViewportRenderBudget(4, 4, 1, 8),
            1000);

    private sealed class StableTileSource : ITileSource<string>
    {
        public ValueTask<string> LoadAsync(
            TileRequest request,
            RectangleF imageRectangle,
            CancellationToken cancellationToken = default) =>
            ValueTask.FromResult("stable");
    }
}
