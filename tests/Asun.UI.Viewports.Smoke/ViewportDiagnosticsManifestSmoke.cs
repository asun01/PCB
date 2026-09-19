using System.Drawing;
using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportDiagnosticsManifestSmoke
{
    public static async ValueTask RunAsync(Action<bool, string> assert)
    {
        using var pipeline = new ViewportRenderPipelineRuntime<string>(
            new Vector2(900, 700),
            new Vector2(360, 240),
            new Vector2(90, 90),
            0,
            32,
            2,
            new StableTileSource(),
            new ViewportRenderBudget(16, 32, 8, 64),
            120);

        pipeline.Composite.AddRoi(
            RoiGeometry.CreateRectangle(
                new Vector2(160, 110),
                new Vector2(70, 50)));

        var frame = await pipeline.RefreshAsync(
            DateTimeOffset.UtcNow.AddSeconds(1));

        assert(
            frame is not null,
            "Diagnostics manifest smoke should create a pipeline frame.");

        if (frame is null)
            return;

        var replay = new ViewportRenderReplaySink<string>();

        var delivery = await ViewportRenderDeliveryRuntime.TryDeliverAsync(
            frame,
            replay);

        var manifest = ViewportRenderDiagnosticsRuntime.BuildManifest(
            frame,
            delivery.FrameState,
            replay.Snapshot);

        var validation =
            ViewportRenderDiagnosticsRuntime.ValidateManifest(manifest);

        assert(
            delivery.Succeeded &&
            validation.Count == 0,
            "A successful delivery should produce a structurally valid evidence manifest.");

        assert(
            manifest.Generation == frame.Composite.Generation &&
            manifest.SubmissionSequence == frame.Submission.Sequence &&
            manifest.BatchItemCount == frame.Batch.ItemCount &&
            manifest.RegionCount == frame.Batch.RegionCount,
            "Evidence manifest identity must agree with the originating pipeline frame.");

        assert(
            manifest.BatchHash == ViewportRenderEvidenceRuntime.ComputeBatchHash(frame.Batch) &&
            manifest.CommandHash == ViewportRenderEvidenceRuntime.ComputeCommandStreamHash(frame.CommandStream) &&
            manifest.FrameHash == ViewportRenderEvidenceRuntime.ComputePipelineFrameHash(frame) &&
            manifest.ReplayHash == replay.Snapshot.EvidenceHash,
            "Evidence manifest hashes must reconcile exactly to the underlying artifacts.");

        var json = manifest.ToJson();

        assert(
            json.Contains(""generation":") &&
            json.Contains(""frameHash":") &&
            !json.Contains("\n"),
            "Evidence manifest JSON should be compact and expose stable camel-case keys.");

        assert(
            manifest.StableKey.Contains(
                manifest.Generation.ToString()) &&
            manifest.StableKey.EndsWith(manifest.FrameHash),
            "Evidence manifest StableKey should bind generation and frame evidence.");

        var audit = new ViewportPresentationAuditTrace();

        audit.Record(
            "FrameBuilt",
            manifest.Generation,
            manifest.SubmissionSequence,
            renderedUnits: manifest.RenderedUnits,
            deferredUnits: manifest.DeferredUnits,
            evidenceKey: manifest.StableKey);

        audit.Record(
            "Delivered",
            manifest.Generation,
            manifest.SubmissionSequence,
            delivery.Status,
            delivery.RenderedUnits,
            delivery.FrameState.DeferredUnits,
            manifest.StableKey);

        audit.Record(
            "Observed",
            manifest.Generation,
            manifest.SubmissionSequence,
            delivery.Status,
            delivery.RenderedUnits,
            delivery.FrameState.DeferredUnits,
            manifest.StableKey);

        assert(
            audit.Count == 3 &&
            audit.IsStrictlyOrdered() &&
            audit.Validate().Count == 0,
            "Presentation audit events should be strictly ordered and structurally valid.");

        var events = audit.Snapshot();

        assert(
            events[0].Sequence < events[1].Sequence &&
            events[1].Sequence < events[2].Sequence &&
            events.All(item =>
                item.Generation == manifest.Generation &&
                item.SubmissionSequence == manifest.SubmissionSequence),
            "Audit event ordering should remain tied to the same presentation transaction.");

        audit.Reset();

        assert(
            audit.Count == 0 &&
            audit.IsStrictlyOrdered() &&
            audit.Validate().Count == 0,
            "Audit reset should deterministically clear the trace.");

        var invalidManifest = manifest with
        {
            BatchHash = "short",
            RenderedUnits = manifest.PlannedUnits + 1
        };

        assert(
            ViewportRenderDiagnosticsRuntime.ValidateManifest(
                invalidManifest).Count >= 2,
            "Diagnostics validation should reject invalid evidence hashes and impossible counters.");
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
