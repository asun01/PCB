using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportReplayCheckpointSequenceHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var sparse = new[]
        {
            new ViewportInputEvent(10, ViewportInputEventKind.PointerMove, new Vector2(10, 20), 0, ViewportMouseButton.Left),
            new ViewportInputEvent(20, ViewportInputEventKind.PointerMove, new Vector2(20, 30), 0, ViewportMouseButton.Left),
            new ViewportInputEvent(40, ViewportInputEventKind.Wheel, new Vector2(30, 40), 120, ViewportMouseButton.Left)
        };

        static ViewportCompositeRuntime<string> CreateComposite()
        {
            var composite = new ViewportCompositeRuntime<string>(
                new Vector2(1200, 900),
                new Vector2(400, 300),
                new Vector2(100, 100),
                1,
                32,
                2,
                new LocalTileSource());

            return composite;
        }

        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        for (var i = 0; i < 10; i++)
        {
            using var composite = CreateComposite();
            var input = new ViewportCompositeInputRuntime<string>(composite);
            var report = ViewportReplayExecutionStateRuntime.Execute(input, sparse);
            Check(
                report.Execution.LastInputSequence == 40,
                $"execution last sequence {i + 1} should preserve source sequence.");
        }

        for (var i = 0; i < 10; i++)
        {
            using var composite = CreateComposite();
            var input = new ViewportCompositeInputRuntime<string>(composite);
            var report = ViewportReplayExecutionStateRuntime.Execute(input, sparse);
            var checkpoint = ViewportReplayCheckpointRuntime.Capture(report);
            Check(
                checkpoint.LastInputSequence == 40 &&
                checkpoint.InputCount == 3,
                $"report checkpoint sequence {i + 1} should use the actual last event sequence.");
        }

        for (var i = 0; i < 10; i++)
        {
            using var composite = CreateComposite();
            var input = new ViewportCompositeInputRuntime<string>(composite);
            var report = ViewportReplayExecutionStateRuntime.Execute(input, sparse);
            var checkpoint = ViewportReplayCheckpointRuntime.Capture(report, sparse);
            Check(
                ViewportReplayCheckpointRuntime.Validate(checkpoint).Count == 0 &&
                checkpoint.LastInputSequence == 40,
                $"explicit checkpoint sequence {i + 1} should match report semantics.");
        }

        for (var i = 0; i < 10; i++)
        {
            var checkpoint = new ViewportReplayCheckpoint(
                1, 1, 0, new string('a', 64), new string('b', 64), new string('c', 64), 1);
            Check(
                ViewportReplayCheckpointRuntime.Validate(checkpoint).Any(
                    item => item.Contains("positive last input sequence", StringComparison.Ordinal)),
                $"zero sequence non-empty checkpoint {i + 1} should fail.");
        }

        for (var i = 0; i < 10; i++)
        {
            var empty = new ViewportReplayCheckpoint(
                1, 0, 0, new string('a', 64), new string('b', 64), new string('c', 64), 0);
            Check(
                ViewportReplayCheckpointRuntime.Validate(empty).Count == 0 &&
                empty.IsEmpty,
                $"empty checkpoint {i + 1} should remain valid.");
        }

        for (var i = 0; i < 10; i++)
        {
            var store = new ViewportReplayCheckpointStore(4);
            store.Add(new ViewportReplayCheckpoint(
                1, 1, 10, new string('a', 64), new string('b', 64), new string('c', 64), 1));

            var rejected = false;
            try
            {
                store.Add(new ViewportReplayCheckpoint(
                    2, 2, 20, new string('a', 64), new string('b', 64), new string('c', 64), 0));
            }
            catch (InvalidOperationException)
            {
                rejected = true;
            }

            Check(
                rejected,
                $"descending generation {i + 1} should be rejected by checkpoint store.");
        }

        for (var i = 0; i < 10; i++)
        {
            using var composite = CreateComposite();
            var input = new ViewportCompositeInputRuntime<string>(composite);
            var prefix = sparse.Take(2).ToArray();
            var report = ViewportReplayExecutionStateRuntime.Execute(input, prefix);
            var checkpoint = ViewportReplayCheckpointRuntime.Capture(report);
            Check(
                checkpoint.InputCount == 2 &&
                checkpoint.LastInputSequence == 20,
                $"prefix sequence {i + 1} should retain sparse sequence identity.");
        }

        for (var i = 0; i < 10; i++)
        {
            using var composite = CreateComposite();
            var input = new ViewportCompositeInputRuntime<string>(composite);
            var report = ViewportReplayExecutionStateRuntime.Execute(input, Array.Empty<ViewportInputEvent>());
            var checkpoint = ViewportReplayCheckpointRuntime.Capture(report);
            Check(
                checkpoint.InputCount == 0 &&
                checkpoint.LastInputSequence == 0 &&
                checkpoint.IsEmpty,
                $"empty execution checkpoint {i + 1} should be deterministic.");
        }

        for (var i = 0; i < 10; i++)
        {
            using var composite = CreateComposite();
            var input = new ViewportCompositeInputRuntime<string>(composite);
            var report = ViewportReplayExecutionStateRuntime.Execute(input, sparse);
            var checkpoint = ViewportReplayCheckpointRuntime.Capture(report);
            Check(
                checkpoint.InputHash == report.Execution.InputHash &&
                checkpoint.ResultHash == report.Execution.ResultHash &&
                checkpoint.StateHash == report.FinalState.StateHash,
                $"checkpoint hashes {i + 1} should remain aligned.");
        }

        for (var i = 0; i < 10; i++)
        {
            using var composite = CreateComposite();
            var input = new ViewportCompositeInputRuntime<string>(composite);
            var report = ViewportReplayExecutionStateRuntime.Execute(input, sparse);
            var first = ViewportReplayCheckpointRuntime.Capture(report);
            var second = first with { Ordinal = first.Ordinal };
            Check(
                ViewportReplayCheckpointRuntime.AreEquivalent(first, second),
                $"checkpoint equality {i + 1} should remain deterministic.");
        }

        assert(
            round == 100,
            $"Checkpoint sequence smoke should execute exactly 100 numbered rounds; actual {round}.");
    }

    private sealed class LocalTileSource : ITileSource<string>
    {
        public ValueTask<string> LoadAsync(
            TileRequest request,
            System.Drawing.RectangleF imageRectangle,
            CancellationToken cancellationToken = default) =>
            ValueTask.FromResult($"tile:{request.Index.X},{request.Index.Y}");
    }
}
