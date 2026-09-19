namespace Asun.Platform.Core;

/// <summary>
/// Deterministic percentile calculation for performance measurements.
/// The input is copied and sorted; the caller's collection is never mutated.
/// </summary>
public static class Percentiles
{
    public static IReadOnlyList<double> CalculateMany(
        IEnumerable<double> samples,
        IEnumerable<double> percentiles)
    {
        ArgumentNullException.ThrowIfNull(samples);
        ArgumentNullException.ThrowIfNull(percentiles);

        var requested = percentiles.ToArray();
        foreach (var percentile in requested)
        {
            if (!double.IsFinite(percentile) || percentile < 0 || percentile > 100)
                throw new ArgumentOutOfRangeException(nameof(percentiles), percentile, "Percentile must be between 0 and 100.");
        }

        var values = samples.ToArray();
        if (values.Length == 0)
            throw new ArgumentException("At least one sample is required.", nameof(samples));

        if (values.Any(value => !double.IsFinite(value)))
            throw new ArgumentException("Samples must contain only finite values.", nameof(samples));

        Array.Sort(values);

        var results = new double[requested.Length];
        for (var index = 0; index < requested.Length; index++)
            results[index] = CalculateSorted(values, requested[index]);

        return Array.AsReadOnly(results);
    }

    public static double Calculate(IEnumerable<double> samples, double percentile)
    {
        ArgumentNullException.ThrowIfNull(samples);

        if (!double.IsFinite(percentile) || percentile < 0 || percentile > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(percentile), percentile, "Percentile must be between 0 and 100.");
        }

        var values = samples.ToArray();

        if (values.Length == 0)
        {
            throw new ArgumentException("At least one sample is required.", nameof(samples));
        }

        if (values.Any(value => !double.IsFinite(value)))
        {
            throw new ArgumentException("Samples must contain only finite values.", nameof(samples));
        }

        Array.Sort(values);
        return CalculateSorted(values, percentile);
    }

    private static double CalculateSorted(
        IReadOnlyList<double> values,
        double percentile)
    {
        if (values.Count == 1)
            return values[0];

        var position = percentile / 100d * (values.Count - 1);
        var lower = (int)Math.Floor(position);
        var upper = (int)Math.Ceiling(position);

        if (lower == upper)
            return values[lower];

        var fraction = position - lower;
        return values[lower] + (values[upper] - values[lower]) * fraction;
    }
}
