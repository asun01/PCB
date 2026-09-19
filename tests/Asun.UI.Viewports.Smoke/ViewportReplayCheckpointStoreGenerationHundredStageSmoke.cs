using Asun.UI.Viewports;

public static class ViewportReplayCheckpointStoreGenerationHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        static ViewportReplayCheckpoint Create(
            int ordinal,
            int inputCount,
            long sequence,
            long generation)
        {
            return new ViewportReplayCheckpoint(
                ordinal,
                inputCount,
                sequence,
                new string('a', 64),
                new string('b', 64),
                new string('c', 64),
                generation);
        }

        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        for (var i = 0; i < 10; i++)
        {
            var store = new ViewportReplayCheckpointStore(4);
            store.Add(Create(1, 1, 10, 1));
            store.Add(Create(2, 2, 20, 2));

            Check(
                store.Latest?.Generation == 2 &&
                store.Count == 2,
                $"generation retention {i + 1} should be correct.");
        }

        for (var i = 0; i < 10; i++)
        {
            var store = new ViewportReplayCheckpointStore(4);
            store.Add(Create(1, 1, 10, 2));
            var rejected = false;
            try
            {
                store.Add(Create(2, 2, 20, 1));
            }
            catch (InvalidOperationException)
            {
                rejected = true;
            }

            Check(
                rejected,
                $"descending generation {i + 1} should be rejected.");
        }

        for (var i = 0; i < 10; i++)
        {
            var store = new ViewportReplayCheckpointStore(2);
            store.Add(Create(1, 1, 1, 1));
            store.Add(Create(2, 2, 2, 2));
            store.Add(Create(3, 3, 3, 3));

            Check(
                store.Count == 2 &&
                store.Snapshot()[0].Generation == 2 &&
                store.Latest?.Generation == 3,
                $"bounded generation tail {i + 1} should be deterministic.");
        }

        for (var i = 0; i < 10; i++)
        {
            var store = new ViewportReplayCheckpointStore(4);
            store.Add(Create(1, 1, 10, 5));
            store.Add(Create(2, 2, 20, 5));

            Check(
                store.Latest?.Generation == 5,
                $"equal generation {i + 1} should remain valid.");
        }

        for (var i = 0; i < 10; i++)
        {
            var store = new ViewportReplayCheckpointStore(4);
            store.Add(Create(1, 1, 10, 1));
            store.Add(Create(2, 2, 20, 3));
            store.Clear();

            Check(
                store.Count == 0 &&
                store.Latest is null,
                $"generation store clear {i + 1} should reset state.");
        }

        for (var i = 0; i < 10; i++)
        {
            var store = new ViewportReplayCheckpointStore(4);
            var checkpoint = Create(1, 1, 10, 1);
            store.Add(checkpoint);

            Check(
                ViewportReplayCheckpointRuntime.Validate(
                    store.Latest!).Count == 0,
                $"stored checkpoint validation {i + 1} should remain clean.");
        }

        for (var i = 0; i < 10; i++)
        {
            var checkpoint = Create(1, 1, 10, 1);
            var changed = checkpoint with { Generation = 0 };

            Check(
                ViewportReplayCheckpointRuntime.Compare(
                    checkpoint,
                    changed).Differences.Any(
                        item => item.StartsWith(
                            "Generation",
                            StringComparison.Ordinal)),
                $"generation difference {i + 1} should be explicit.");
        }

        for (var i = 0; i < 10; i++)
        {
            var checkpoint = Create(1, 1, 10, 1);
            var invalid = checkpoint with { InputCount = 0 };
            var errors = ViewportReplayCheckpointRuntime.Validate(invalid);

            Check(
                errors.Any(
                    item => item.Contains(
                        "Empty checkpoint",
                        StringComparison.OrdinalIgnoreCase)),
                $"empty checkpoint sequence mismatch {i + 1} should fail.");
        }

        for (var i = 0; i < 10; i++)
        {
            var checkpoint = Create(1, 1, 10, 1);
            var invalid = checkpoint with { LastInputSequence = 0 };
            Check(
                ViewportReplayCheckpointRuntime.Validate(invalid).Count > 0,
                $"zero last sequence {i + 1} should fail.");
        }

        for (var i = 0; i < 10; i++)
        {
            var store = new ViewportReplayCheckpointStore(4);
            store.Add(Create(1, 1, 10, 1));
            store.Add(Create(2, 2, 20, 2));

            Check(
                store.Snapshot().Select(item => item.Generation).SequenceEqual(new long[] { 1, 2 }),
                $"generation order {i + 1} should remain monotonic.");
        }

        assert(
            round == 100,
            $"Checkpoint store generation smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
