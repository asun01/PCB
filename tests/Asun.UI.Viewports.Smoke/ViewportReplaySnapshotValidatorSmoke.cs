using System.Drawing;
using Asun.UI.Viewports;

public static class ViewportReplaySnapshotValidatorSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var operations = new[]
        {
            new ViewportRenderReplayOperation(
                1,
                ViewportRenderReplayOperationKind.Begin,
                7,
                null,
                Guid.Empty,
                new RectangleF(0, 0, 100, 100),
                0,
                null),
            new ViewportRenderReplayOperation(
                2,
                ViewportRenderReplayOperationKind.DrawTile,
                7,
                new TileIndex(0, 0),
                Guid.Empty,
                new RectangleF(0, 0, 50, 50),
                1,
                null),
            new ViewportRenderReplayOperation(
                3,
                ViewportRenderReplayOperationKind.End,
                7,
                null,
                Guid.Empty,
                new RectangleF(0, 0, 100, 100),
                0,
                null),
            new ViewportRenderReplayOperation(
                4,
                ViewportRenderReplayOperationKind.Commit,
                7,
                null,
                Guid.Empty,
                new RectangleF(0, 0, 50, 50),
                1,
                ViewportRenderDeliveryStatus.Succeeded)
        };

        var validSnapshot = new ViewportRenderReplaySnapshot(
            4,
            7,
            1,
            1,
            1,
            0,
            0,
            0,
            1,
            0,
            1,
            operations);

        var valid = ViewportRenderReplaySnapshotValidatorRuntime.Validate(
            validSnapshot);

        assert(
            valid.IsValid &&
            valid.Errors.Count == 0,
            "Replay validator should accept a balanced committed frame.");

        var invalidSequence = validSnapshot with
        {
            Operations = operations
                .Select((operation, index) =>
                    index == 1
                        ? operation with { Sequence = 99 }
                        : operation)
                .ToArray()
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
            Operations = operations.Take(1).ToArray(),
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

        var invalidGeneration = validSnapshot with
        {
            LastGeneration = 999
        };

        var generationResult =
            ViewportRenderReplaySnapshotValidatorRuntime.Validate(
                invalidGeneration);

        assert(
            !generationResult.IsValid &&
            generationResult.Errors.Any(
                error => error.Contains(
                    "last generation",
                    StringComparison.OrdinalIgnoreCase)),
            "Replay validator should reject an inconsistent last generation.");

        sinklessReset:

        var empty = new ViewportRenderReplaySnapshot(
            0,
            null,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            Array.Empty<ViewportRenderReplayOperation>());

        assert(
            ViewportRenderReplaySnapshotValidatorRuntime.Validate(
                empty).IsValid,
            "Replay validator should accept the empty reset state.");
    }
}
