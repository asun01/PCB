using Asun.UI.Viewports;

public static class ViewportReplaySnapshotValidatorSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var sink = new ViewportRenderReplaySink<string>();
        var context = new ViewportRenderFrameContext(
            7,
            new System.Drawing.RectangleF(0, 0, 100, 100),
            1,
            1);

        sink.BeginFrameAsync(context).GetAwaiter().GetResult();
        sink.DrawTileAsync(
            new ViewportRenderTileContext<string>(
                7,
                new TileIndex(0, 0),
                "tile",
                new System.Drawing.RectangleF(0, 0, 50, 50))).GetAwaiter().GetResult();
        sink.EndFrameAsync(context).GetAwaiter().GetResult();
        sink.CommitFrameAsync(
            new ViewportRenderCommitContext(
                7,
                1,
                1,
                1,
                new[]
                {
                    new System.Drawing.RectangleF(0, 0, 50, 50)
                },
                1,
                0,
                0,
                0,
                0)).GetAwaiter().GetResult();

        var valid = ViewportRenderReplaySnapshotValidatorRuntime.Validate(
            sink.Snapshot);

        assert(
            valid.IsValid &&
            valid.Errors.Count == 0,
            "Replay sink output should satisfy lifecycle and counter invariants.");

        var validSnapshot = sink.Snapshot;

        var reorderedOperations = validSnapshot.Operations
            .Select((operation, index) =>
                index == 0 && validSnapshot.Operations.Count > 1
                    ? validSnapshot.Operations[1] with { Sequence = 1 }
                    : operation)
            .ToArray();

        var invalidSequence = validSnapshot with
        {
            Operations = reorderedOperations
        };

        var sequenceResult =
            ViewportRenderReplaySnapshotValidatorRuntime.Validate(
                invalidSequence);

        assert(
            !sequenceResult.IsValid &&
            sequenceResult.Errors.Any(
                error => error.Contains(
                    "sequence",
                    StringComparison.OrdinalIgnoreCase)),
            "Replay validator should reject broken operation sequencing.");

        var invalidCounts = validSnapshot with
        {
            TileCount = validSnapshot.TileCount + 1
        };

        var countResult =
            ViewportRenderReplaySnapshotValidatorRuntime.Validate(
                invalidCounts);

        assert(
            !countResult.IsValid &&
            countResult.Errors.Any(
                error => error.Contains(
                    "tile count",
                    StringComparison.OrdinalIgnoreCase)),
            "Replay validator should reject inconsistent tile counters.");

        var openFrame = validSnapshot with
        {
            Operations = validSnapshot.Operations.Take(1).ToArray(),
            OperationCount = 1,
            BeginCount = 1,
            EndCount = 0,
            CommitCount = 0,
            RenderedUnits = 0
        };

        var openFrameResult =
            ViewportRenderReplaySnapshotValidatorRuntime.Validate(
                openFrame);

        assert(
            !openFrameResult.IsValid &&
            openFrameResult.Errors.Any(
                error => error.Contains(
                    "open frame",
                    StringComparison.OrdinalIgnoreCase)),
            "Replay validator should reject an unfinished frame.");

        sink.Reset();

        assert(
            ViewportRenderReplaySnapshotValidatorRuntime.Validate(
                sink.Snapshot).IsValid,
            "Replay validator should accept the empty reset state.");
    }
}
