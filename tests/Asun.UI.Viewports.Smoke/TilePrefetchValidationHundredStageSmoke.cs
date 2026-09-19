using Asun.UI.Viewports;

public static class TilePrefetchValidationHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var source = new[]
        {
            new TileRequest(new TileIndex(0, 0), true, 1),
            new TileRequest(new TileIndex(1, 0), true, 4),
            new TileRequest(new TileIndex(2, 0), false, 9),
            new TileRequest(new TileIndex(0, 1), false, 16)
        };

        var policy =
            new TilePrefetchPolicy(1, 3, true, false);

        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        for (var i = 0; i < 10; i++)
        {
            var result =
                TilePrefetchPolicyRuntime.Apply(
                    source,
                    policy);

            Check(
                TilePrefetchValidationRuntime.IsValid(
                    source,
                    result,
                    policy),
                $"prefetch validation {i + 1} should succeed.");
        }

        for (var i = 0; i < 10; i++)
        {
            var result =
                TilePrefetchPolicyRuntime.Apply(
                    source,
                    policy);

            Check(
                result.Count == 3 &&
                result[0].IsVisible &&
                result[1].IsVisible,
                $"visible-first policy {i + 1} should retain visible requests first.");
        }

        for (var i = 0; i < 10; i++)
        {
            var result =
                TilePrefetchPolicyRuntime.Apply(
                    source,
                    new TilePrefetchPolicy(0, 2, true, false));

            Check(
                result.Count == 2 &&
                result.All(
                    item => source.Any(
                        candidate => candidate.Index == item.Index)),
                $"prefetch max limit {i + 1} should bound the result.");
        }

        for (var i = 0; i < 10; i++)
        {
            var duplicateSource = source.Concat(
                new[]
                {
                    new TileRequest(
                        new TileIndex(0, 0),
                        false,
                        100)
                }).ToArray();

            var result =
                TilePrefetchPolicyRuntime.Apply(
                    duplicateSource,
                    policy);

            Check(
                result.Select(item => item.Index).Distinct().Count() ==
                result.Count,
                $"prefetch deduplication {i + 1} should remove duplicate indices.");
        }

        for (var i = 0; i < 10; i++)
        {
            var first =
                TilePrefetchPolicyRuntime.Apply(
                    source,
                    policy);
            var second =
                TilePrefetchPolicyRuntime.Apply(
                    source,
                    policy);

            Check(
                first.SequenceEqual(second),
                $"prefetch determinism {i + 1} should hold.");
        }

        for (var i = 0; i < 10; i++)
        {
            var invalid = new TilePrefetchPolicy(-1, 3, true, false);
            var rejected = false;
            try
            {
                TilePrefetchPolicyRuntime.Apply(source, invalid);
            }
            catch (ArgumentOutOfRangeException)
            {
                rejected = true;
            }

            Check(
                rejected,
                $"negative margin {i + 1} should be rejected.");
        }

        for (var i = 0; i < 10; i++)
        {
            var invalid = new TilePrefetchPolicy(1, 0, true, false);
            var rejected = false;
            try
            {
                TilePrefetchPolicyRuntime.Apply(source, invalid);
            }
            catch (ArgumentOutOfRangeException)
            {
                rejected = true;
            }

            Check(
                rejected,
                $"zero maximum {i + 1} should be rejected.");
        }

        for (var i = 0; i < 10; i++)
        {
            var result =
                TilePrefetchPolicyRuntime.Apply(
                    source,
                    new TilePrefetchPolicy(1, 1, false, true));

            Check(
                result.Count == 1 &&
                result[0].IsVisible,
                $"vertical preference {i + 1} should still preserve visible priority.");
        }

        for (var i = 0; i < 10; i++)
        {
            var result =
                TilePrefetchPolicyRuntime.Apply(
                    source,
                    new TilePrefetchPolicy(1, 4, false, false));

            Check(
                result.All(
                    item => source.Any(
                        candidate => candidate.Index == item.Index)),
                $"neutral preference {i + 1} should preserve source membership.");
        }

        for (var i = 0; i < 10; i++)
        {
            var result =
                TilePrefetchPolicyRuntime.Apply(
                    source,
                    policy);

            Check(
                TilePrefetchValidationRuntime.Validate(
                    source,
                    result,
                    policy).Count == 0,
                $"prefetch final invariant {i + 1} should remain clean.");
        }

        assert(
            round == 100,
            $"Tile prefetch validation smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
