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

        for (var i = 0; i < 10; i++)
        {
            using var pipeline = CreatePipeline();
            using var queue = new ViewportPresentationQueueRuntime<string>(2);

            var frame = await pipeline.RefreshAsync();

            Check(
                frame is not null,
                $"pipeline frame {i + 1} should exist.");

            if (frame is null)
                continue;

            Check(
                queue.TryEnqueue(frame, out var packet),
                $"enqueue {i + 1} should succeed.");

            Check(
                ViewportPresentationQueueValidationRuntime.IsValid(queue.Statistics),
                $"queued state {i + 1} should satisfy accounting invariants.");

            Check(
                queue.TryTakeLatest(out var inFlight) &&
                inFlight.Token == packet.Token,
                $"latest take {i + 1} should select the exact packet.");

            Check(
                ViewportPresentationQueueValidationRuntime.IsValid(queue.Statistics) &&
                queue.IsCurrent(inFlight.Token),
                $"in-flight state {i + 1} should retain the current token.");

            Check(
                queue.TryBeginCommit(inFlight.Token),
                $"commit window {i + 1} should open for the current token.");

            Check(
                queue.Statistics.CommitInProgress &&
                ViewportPresentationQueueValidationRuntime.IsValid(queue.Statistics),
                $"commit diagnostics {i + 1} should remain coherent.");

            Check(
                queue.TryCompleteCommit(inFlight.Token),
                $"commit completion {i + 1} should acknowledge the token.");

            Check(
                ViewportPresentationQueueValidationRuntime.IsValid(queue.Statistics) &&
                queue.Statistics.PresentedSequence == inFlight.Token.Sequence,
                $"presented state {i + 1} should preserve the published sequence.");

            Check(
                queue.TryEnqueue(frame, out var newer) &&
                newer.Token.Sequence > inFlight.Token.Sequence &&
                queue.TryTakeLatest(out var newerInFlight) &&
                queue.TryCancel(newerInFlight.Token),
                $"post-present cancel cycle {i + 1} should preserve monotonic sequencing.");

            queue.Reset();

            Check(
                ViewportPresentationQueueValidationRuntime.IsValid(queue.Statistics) &&
                queue.Statistics.Pending == 0 &&
                queue.Statistics.LatestGeneration is null,
                $"reset state {i + 1} should clear lifecycle state without breaking fence accounting.");
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
