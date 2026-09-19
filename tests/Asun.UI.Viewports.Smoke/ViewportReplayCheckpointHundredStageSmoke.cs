using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportReplayCheckpointHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var events = Enumerable.Range(1, 10)
            .Select(i =>
                new ViewportInputEvent(
                    i,
                    ViewportInputEventKind.PointerMove,
                    new Vector2(i * 10, i * 5),
                    0,
                    ViewportMouseButton.Left))
            .ToArray();

        static ViewportCompositeInputRuntime<string> CreateInput()
        {
            var composite = new ViewportCompositeRuntime<string>(
                new Vector2(1200, 900),
                new Vector2(400, 300),
                new Vector2(100, 100),
                1,
                32,
                2,
                new LocalTileSource());

            return new ViewportCompositeInputRuntime<string>(
                composite);
        }

        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        for (var i = 0; i < 10; i++)
        {
            var input = CreateInput();

            using (input.Composite)
            {
                var report =
                    ViewportReplayExecutionStateRuntime.Execute(
                        input,
                        events.Take(i + 1).ToArray());

                var checkpoint =
                    ViewportReplayCheckpointRuntime.Capture(
                        report,
                        events.Take(i + 1).ToArray());

                Check(
                    ViewportReplayCheckpointRuntime.Validate(
                        checkpoint).Count == 0 &&
                    checkpoint.InputCount == i + 1 &&
                    checkpoint.LastInputSequence == i + 1,
                    $"prefix checkpoint {i + 1} should validate.");
            }
        }

        for (var i = 0; i < 10; i++)
        {
            var input = CreateInput();

            using (input.Composite)
            {
                var report =
                    ViewportReplayExecutionStateRuntime.Execute(
                        input,
                        events);

                var checkpoint =
                    ViewportReplayCheckpointRuntime.Capture(report);

                Check(
                    checkpoint.InputCount == events.Length &&
                    checkpoint.LastInputSequence ==
                    events.Length &&
                    checkpoint.StateHash ==
                    report.FinalState.StateHash,
                    $"full checkpoint {i + 1} should match execution state.");
            }
        }

        for (var i = 0; i < 10; i++)
        {
            var input = CreateInput();

            using (input.Composite)
            {
                var report =
                    ViewportReplayExecutionStateRuntime.Execute(
                        input,
                        events);

                var first =
                    ViewportReplayCheckpointRuntime.Capture(report);

                var second =
                    first with
                    {
                        Ordinal = first.Ordinal + 1
                    };

                var comparison =
                    ViewportReplayCheckpointRuntime.Compare(
                        first,
                        second);

                Check(
                    !comparison.IsEquivalent &&
                    comparison.Differences.Any(
                        item => item.StartsWith(
                            "Ordinal",
                            StringComparison.Ordinal)),
                    $"checkpoint ordinal diff {i + 1} should be visible.");
            }
        }

        for (var i = 0; i < 10; i++)
        {
            var store =
                new ViewportReplayCheckpointStore(3);

            store.Add(
                new ViewportReplayCheckpoint(
                    1,
                    1,
                    1,
                    new string('a', 64),
                    new string('b', 64),
                    new string('c', 64),
                    1));
            store.Add(
                new ViewportReplayCheckpoint(
                    2,
                    2,
                    2,
                    new string('a', 64),
                    new string('b', 64),
                    new string('c', 64),
                    2));
            store.Add(
                new ViewportReplayCheckpoint(
                    3,
                    3,
                    3,
                    new string('a', 64),
                    new string('b', 64),
                    new string('c', 64),
                    3));
            store.Add(
                new ViewportReplayCheckpoint(
                    4,
                    4,
                    4,
                    new string('a', 64),
                    new string('b', 64),
                    new string('c', 64),
                    4));

            Check(
                store.Count == 3 &&
                store.Latest?.Ordinal == 4 &&
                store.Snapshot()[0].Ordinal == 2,
                $"bounded checkpoint store {i + 1} should retain only its tail.");
        }

        for (var i = 0; i < 10; i++)
        {
            var store = new ViewportReplayCheckpointStore(8);
            var checkpoint =
                new ViewportReplayCheckpoint(
                    1,
                    1,
                    1,
                    new string('a', 64),
                    new string('b', 64),
                    new string('c', 64),
                    1);

            store.Add(checkpoint);
            store.Clear();

            Check(
                store.Count == 0 &&
                store.Latest is null &&
                store.Snapshot().Count == 0,
                $"checkpoint store clear {i + 1} should be deterministic.");
        }

        for (var i = 0; i < 10; i++)
        {
            var rejected = false;
            var store = new ViewportReplayCheckpointStore(8);

            var first =
                new ViewportReplayCheckpoint(
                    2,
                    2,
                    2,
                    new string('a', 64),
                    new string('b', 64),
                    new string('c', 64),
                    2);

            var second = first with
            {
                Ordinal = 2
            };

            store.Add(first);

            try
            {
                store.Add(second);
            }
            catch (InvalidOperationException)
            {
                rejected = true;
            }

            Check(
                rejected,
                $"duplicate checkpoint order {i + 1} should be rejected.");
        }

        for (var i = 0; i < 10; i++)
        {
            var checkpoint =
                new ViewportReplayCheckpoint(
                    1,
                    1,
                    1,
                    new string('a', 63),
                    new string('b', 64),
                    new string('c', 64),
                    1);

            Check(
                ViewportReplayCheckpointRuntime.Validate(
                    checkpoint).Count > 0,
                $"invalid checkpoint hash {i + 1} should be rejected.");
        }

        for (var i = 0; i < 10; i++)
        {
            var checkpoint =
                new ViewportReplayCheckpoint(
                    1,
                    0,
                    1,
                    new string('a', 64),
                    new string('b', 64),
                    new string('c', 64),
                    1);

            Check(
                ViewportReplayCheckpointRuntime.Validate(
                    checkpoint).Any(
                        item => item.Contains(
                            "Empty checkpoint",
                            StringComparison.Ordinal)),
                $"empty checkpoint sequence {i + 1} should be rejected.");
        }

        for (var i = 0; i < 10; i++)
        {
            var input = CreateInput();

            using (input.Composite)
            {
                var report =
                    ViewportReplayExecutionStateRuntime.Execute(
                        input,
                        Array.Empty<ViewportInputEvent>());

                var checkpoint =
                    ViewportReplayCheckpointRuntime.Capture(report);

                Check(
                    checkpoint.IsEmpty &&
                    checkpoint.InputCount == 0 &&
                    checkpoint.LastInputSequence == 0,
                    $"empty replay checkpoint {i + 1} should be empty.");
            }
        }

        for (var i = 0; i < 10; i++)
        {
            var input = CreateInput();

            using (input.Composite)
            {
                var report =
                    ViewportReplayExecutionStateRuntime.Execute(
                        input,
                        events.Take(3).ToArray());

                var checkpoint =
                    ViewportReplayCheckpointRuntime.Capture(
                        report,
                        events.Take(3).ToArray());

                var restored = checkpoint with
                {
                    Ordinal = checkpoint.Ordinal
                };

                Check(
                    ViewportReplayCheckpointRuntime.AreEquivalent(
                        checkpoint,
                        restored),
                    $"checkpoint deterministic equality {i + 1} should hold.");
            }
        }

        assert(
            round == 100,
            $"Replay checkpoint smoke should execute exactly 100 numbered rounds; actual {round}.");
    }

    private sealed class LocalTileSource : ITileSource<string>
    {
        public ValueTask<string> LoadAsync(
            TileRequest request,
            System.Drawing.RectangleF imageRectangle,
            CancellationToken cancellationToken = default) =>
            ValueTask.FromResult(
                $"tile:{request.Index.X},{request.Index.Y}");
    }
}
