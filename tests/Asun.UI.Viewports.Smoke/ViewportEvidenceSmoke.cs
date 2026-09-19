using System.Drawing;
using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportEvidenceSmoke
{
    public static async ValueTask RunAsync(Action<bool, string> assert)
    {
        using var pipeline = new ViewportRenderPipelineRuntime<string>(
            new Vector2(800, 600),
            new Vector2(300, 200),
            new Vector2(100, 100),
            0,
            16,
            2,
            new StableTileSource(),
            new ViewportRenderBudget(16, 32, 8, 64),
            120);

        pipeline.Composite.AddRoi(
            RoiGeometry.CreateRectangle(
                new Vector2(120, 80),
                new Vector2(40, 30)));

        var frame = await pipeline.RefreshAsync(
            DateTimeOffset.UtcNow.AddSeconds(1));

        assert(frame is not null, "Evidence smoke should create a pipeline frame.");

        if (frame is null)
            return;

        var batchHash = ViewportRenderEvidenceRuntime.ComputeBatchHash(frame.Batch);
        var commandHash = ViewportRenderEvidenceRuntime.ComputeCommandStreamHash(
            frame.CommandStream);
        var frameHash = ViewportRenderEvidenceRuntime.ComputePipelineFrameHash(frame);

        assert(
            batchHash.Length == 64 &&
            commandHash.Length == 64 &&
            frameHash.Length == 64,
            "Frame, batch, and command evidence hashes should be fixed-length SHA-256 digests.");

        assert(
            batchHash != commandHash &&
            frameHash != batchHash,
            "Distinct logical render artifacts should not collapse to the same evidence identity.");

        var directReplay = new ViewportRenderReplaySink<string>();
        var direct = await ViewportRenderDeliveryRuntime.TryDeliverAsync(
            frame,
            directReplay);

        assert(
            direct.Succeeded &&
            directReplay.Snapshot.CommitCount == 0 &&
            directReplay.Snapshot.DiscardCount == 0,
            "Direct delivery without a surface should end cleanly without inventing a commit or discard transaction.");

        var directHash = directReplay.Snapshot.EvidenceHash;

        using var surface = new ViewportRenderSurfaceRuntime();
        var transactionalReplay = new ViewportRenderReplaySink<string>();
        var transactional = await ViewportRenderDeliveryRuntime.TryDeliverAsync(
            frame,
            transactionalReplay,
            surface: surface);

        var transactionalSnapshot = transactionalReplay.Snapshot;
        var surfaceSnapshot = surface.Snapshot;

        assert(
            transactional.Succeeded &&
            transactionalSnapshot.CommitCount == 1 &&
            transactionalSnapshot.DiscardCount == 0 &&
            surfaceSnapshot.State == ViewportRenderSurfaceState.Presented,
            "Successful transactional delivery should produce one committed replay operation and one presented surface.");

        assert(
            transactionalSnapshot.EvidenceHash != directHash,
            "Adding a presentation transaction must change replay evidence because Commit is part of the logical delivery trace.");

        assert(
            surfaceSnapshot.PresentedRegionCount == frame.Batch.RegionCount &&
            surfaceSnapshot.LastPlannedUnits == frame.Batch.ItemCount &&
            surfaceSnapshot.LastRenderedUnits == transactional.RenderedUnits,
            "Surface evidence should preserve exact batch region and layer-unit accounting.");

        assert(
            ViewportInvariantRuntime.ValidateSurfaceSnapshot(surfaceSnapshot).Count == 0 &&
            ViewportInvariantRuntime.ValidateReplaySnapshot(transactionalSnapshot).Count == 0,
            "Committed surface and replay evidence should satisfy structural invariants.");

        var failingReplay = new ViewportRenderReplaySink<string>();
        using var failingSurface = new ViewportRenderSurfaceRuntime();
        var failure = await ViewportRenderDeliveryRuntime.TryDeliverAsync(
            frame,
            new FailingCommitSink(failingReplay),
            surface: failingSurface);

        var failedReplaySnapshot = failingReplay.Snapshot;
        var failedSurfaceSnapshot = failingSurface.Snapshot;

        assert(
            !failure.Succeeded &&
            failure.Status == ViewportRenderDeliveryStatus.Failed &&
            failedReplaySnapshot.DiscardCount == 1 &&
            failedReplaySnapshot.CommitCount == 0 &&
            failedSurfaceSnapshot.State == ViewportRenderSurfaceState.Discarded,
            "A commit failure should leave a discard evidence record and never report a committed surface.");

        assert(
            failedReplaySnapshot.LastGeneration == frame.Composite.Generation &&
            failedSurfaceSnapshot.DiscardedGeneration == frame.Composite.Generation,
            "Failure evidence should retain the exact logical generation being discarded.");

        var retryHash = ViewportRenderEvidenceRuntime.ComputeReplayHash(
            failedReplaySnapshot.Operations);

        assert(
            retryHash == failedReplaySnapshot.EvidenceHash,
            "Replay snapshot evidence should be self-consistent with its operation log.");

        assert(
            failedReplaySnapshot.EvidenceHash.Length == 64 &&
            ViewportInvariantRuntime.ValidateReplaySnapshot(failedReplaySnapshot).Count == 0,
            "Discard evidence should remain structurally valid and fingerprintable.");
    }

    private sealed class FailingCommitSink : IViewportRenderSink<string>
    {
        private readonly ViewportRenderReplaySink<string> _inner;

        public FailingCommitSink(ViewportRenderReplaySink<string> inner)
        {
            _inner = inner;
        }

        public ValueTask BeginFrameAsync(
            ViewportRenderFrameContext context,
            CancellationToken cancellationToken = default) =>
            _inner.BeginFrameAsync(context, cancellationToken);

        public ValueTask DrawTileAsync(
            ViewportRenderTileContext<string> tile,
            CancellationToken cancellationToken = default) =>
            _inner.DrawTileAsync(tile, cancellationToken);

        public ValueTask DrawRoiAsync(
            ViewportRenderRoiContext roi,
            CancellationToken cancellationToken = default) =>
            _inner.DrawRoiAsync(roi, cancellationToken);

        public ValueTask DrawOverlayAsync(
            ViewportRenderOverlayContext overlay,
            CancellationToken cancellationToken = default) =>
            _inner.DrawOverlayAsync(overlay, cancellationToken);

        public ValueTask ClearInvalidatedRegionAsync(
            ViewportRenderInvalidationContext invalidation,
            CancellationToken cancellationToken = default) =>
            _inner.ClearInvalidatedRegionAsync(invalidation, cancellationToken);

        public ValueTask EndFrameAsync(
            ViewportRenderFrameContext context,
            CancellationToken cancellationToken = default) =>
            _inner.EndFrameAsync(context, cancellationToken);

        public ValueTask CommitFrameAsync(
            ViewportRenderCommitContext context,
            CancellationToken cancellationToken = default) =>
            throw new InvalidOperationException("synthetic evidence commit failure");

        public ValueTask DiscardFrameAsync(
            ViewportRenderDiscardContext context,
            CancellationToken cancellationToken = default) =>
            _inner.DiscardFrameAsync(context, cancellationToken);
    }

    private sealed class StableTileSource : ITileSource<string>
    {
        public ValueTask<string> LoadAsync(
            TileRequest request,
            RectangleF imageRectangle,
            CancellationToken cancellationToken = default) =>
            ValueTask.FromResult(
                $"tile:{request.Index.X},{request.Index.Y}");
    }
}
