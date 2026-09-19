namespace Asun.Platform.Core;

public static class StatisticsValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        RunningStatistics statistics)
    {
        ArgumentNullException.ThrowIfNull(statistics);

        var errors = new List<string>();

        if (statistics.Count < 0)
            errors.Add("Running statistics count cannot be negative.");

        if (statistics.Count > 0)
        {
            if (!double.IsFinite(statistics.Mean) ||
                !double.IsFinite(statistics.Minimum) ||
                !double.IsFinite(statistics.Maximum) ||
                !double.IsFinite(statistics.VariancePopulation))
            {
                errors.Add("Running statistics must expose finite populated values.");
            }

            if (statistics.Minimum > statistics.Maximum)
                errors.Add("Running statistics minimum cannot exceed maximum.");

            if (statistics.VariancePopulation < 0)
                errors.Add("Population variance cannot be negative.");
        }

        return errors;
    }

    public static IReadOnlyList<string> Validate(
        LatencyStatistics statistics)
    {
        ArgumentNullException.ThrowIfNull(statistics);

        var errors = new List<string>();

        if (statistics.Count <= 0)
            errors.Add("Latency statistics require at least one sample.");

        if (statistics.Minimum < 0 ||
            statistics.Maximum < statistics.Minimum ||
            statistics.Average < statistics.Minimum ||
            statistics.Average > statistics.Maximum)
        {
            errors.Add("Latency summary bounds are inconsistent.");
        }

        if (!statistics.Samples.SequenceEqual(statistics.Samples.OrderBy(value => value)))
            errors.Add("Latency samples must remain sorted.");

        return errors;
    }

    public static bool IsValid(RunningStatistics statistics) =>
        Validate(statistics).Count == 0;

    public static bool IsValid(LatencyStatistics statistics) =>
        Validate(statistics).Count == 0;
}
