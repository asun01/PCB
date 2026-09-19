using Asun.Platform.Core;

public static class StatisticsValidationHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        var running = new RunningStatistics();
        running.AddRange(new[] { 1d, 2d, 3d, 4d, 5d });

        var latency = new LatencyStatistics(new[] { 5d, 1d, 3d, 2d, 4d });
        var p50 = Percentiles.Calculate(new[] { 1d, 2d, 3d, 4d, 5d }, 50);
        var p95 = Percentiles.Calculate(new[] { 10d, 20d, 30d, 40d, 50d }, 95);

        var original = new[] { 3d, 1d, 2d };
        var copy = original.ToArray();
        _ = Percentiles.Calculate(original, 50);

        var invalidRejected = false;
        try
        {
            running.Add(double.NaN);
        }
        catch (ArgumentOutOfRangeException)
        {
            invalidRejected = true;
        }

        var validRunning = StatisticsValidationRuntime.IsValid(running);
        var validLatency = StatisticsValidationRuntime.IsValid(latency);

        for (var i = 0; i < 10; i++)
            Check(running.Count == 5, $"running count round {i + 1} should be five.");

        for (var i = 0; i < 10; i++)
            Check(Math.Abs(running.Mean - 3) < 1e-12, $"running mean round {i + 1} should be three.");

        for (var i = 0; i < 10; i++)
            Check(running.Minimum == 1 && running.Maximum == 5, $"running extrema round {i + 1} should be bounded.");

        for (var i = 0; i < 10; i++)
            Check(Math.Abs(running.VariancePopulation - 2) < 1e-12, $"population variance round {i + 1} should be two.");

        for (var i = 0; i < 10; i++)
            Check(Math.Abs(running.VarianceSample - 2.5) < 1e-12, $"sample variance round {i + 1} should be 2.5.");

        for (var i = 0; i < 10; i++)
            Check(Math.Abs(latency.Median - 3) < 1e-12 && latency.P50 == 3, $"latency median round {i + 1} should be three.");

        for (var i = 0; i < 10; i++)
            Check(Math.Abs(p50 - 3) < 1e-12 && Math.Abs(p95 - 48) < 1e-12, $"percentile interpolation round {i + 1} should be deterministic.");

        for (var i = 0; i < 10; i++)
            Check(original.SequenceEqual(copy), $"caller sample order round {i + 1} should remain unchanged.");

        for (var i = 0; i < 10; i++)
            Check(invalidRejected && running.Count == 5, $"non-finite rejection round {i + 1} should preserve state.");

        for (var i = 0; i < 10; i++)
            Check(validRunning && validLatency, $"statistics validation round {i + 1} should pass.");

        assert(round == 100, $"Statistics validation smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
