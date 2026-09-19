using Asun.UI.Viewports;

public static class ViewportPresentationQueueValidationHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        for (var i = 0; i < 10; i++)
        {
            var statistics = new ViewportPresentationQueueStatistics(
                0, 0, 0, 0, 0, 0, 0,
                null, null, null, null, null,
                0, false, null, null);
            Check(
                ViewportPresentationQueueValidationRuntime.IsValid(statistics),
                $"empty queue {i + 1} should validate.");
        }

        for (var i = 0; i < 10; i++)
        {
            using var queue = new ViewportPresentationQueueRuntime<string>();
            var statistics = queue.Statistics;
            Check(
                ViewportPresentationQueueValidationRuntime.IsValid(statistics),
                $"real queue initial state {i + 1} should validate.");
        }

        for (var i = 0; i < 10; i++)
        {
            var statistics = new ViewportPresentationQueueStatistics(
                5, 2, 1, 2, 0, 0, 1,
                4, 3, 3, null, null,
                5, false, null, null);
            Check(
                ViewportPresentationQueueValidationRuntime.Validate(statistics).Any(
                    item => item.Contains("enqueued =", StringComparison.Ordinal)),
                $"queue accounting mutation {i + 1} should fail.");
        }

        for (var i = 0; i < 10; i++)
        {
            var statistics = new ViewportPresentationQueueStatistics(
                1, 1, 1, 0, 0, 0, 0,
                1, 2, 2, null, null,
                1, false, null, null);
            Check(
                ViewportPresentationQueueValidationRuntime.Validate(statistics).Any(
                    item => item.Contains("Presented sequence", StringComparison.Ordinal)),
                $"presented sequence overflow {i + 1} should fail.");
        }

        for (var i = 0; i < 10; i++)
        {
            var statistics = new ViewportPresentationQueueStatistics(
                1, 1, 0, 0, 0, 0, 0,
                1, null, null, 1, 1,
                1, true, null, 1);
            Check(
                ViewportPresentationQueueValidationRuntime.Validate(statistics).Any(
                    item => item.Contains("committing token", StringComparison.Ordinal)),
                $"incomplete commit token {i + 1} should fail.");
        }

        for (var i = 0; i < 10; i++)
        {
            var statistics = new ViewportPresentationQueueStatistics(
                1, 1, 0, 0, 0, 0, 0,
                1, null, null, null, 2,
                2, false, null, null);
            Check(
                ViewportPresentationQueueValidationRuntime.Validate(statistics).Any(
                    item => item.Contains("In-flight generation and sequence", StringComparison.Ordinal)),
                $"unpaired in-flight token {i + 1} should fail.");
        }

        for (var i = 0; i < 10; i++)
        {
            var statistics = new ViewportPresentationQueueStatistics(
                2, 1, 1, 0, 0, 0, 1,
                1, 1, 1, 1, 1,
                2, true, 1, 1);
            Check(
                ViewportPresentationQueueValidationRuntime.IsValid(statistics),
                $"committing queue {i + 1} should validate.");
        }

        for (var i = 0; i < 10; i++)
        {
            var statistics = new ViewportPresentationQueueStatistics(
                1, 1, 0, 0, 0, 1, 0,
                1, 1, 1, null, null,
                1, false, null, null);
            Check(
                ViewportPresentationQueueValidationRuntime.IsValid(statistics),
                $"cancelled queue {i + 1} should validate.");
        }

        for (var i = 0; i < 10; i++)
        {
            var statistics = new ViewportPresentationQueueStatistics(
                2, 1, 0, 1, 0, 0, 0,
                1, null, null, null, null,
                2, false, null, null);
            Check(
                ViewportPresentationQueueValidationRuntime.IsValid(statistics),
                $"dropped-pending queue {i + 1} should validate.");
        }

        for (var i = 0; i < 10; i++)
        {
            using var queue = new ViewportPresentationQueueRuntime<string>();
            Check(
                ViewportPresentationQueueValidationRuntime.IsValid(
                    queue.Statistics),
                $"reset-ready real queue {i + 1} should validate.");
        }

        assert(
            round == 100,
            $"Presentation queue validation smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
