namespace Asun.Platform.Core;

/// <summary>
/// Immutable snapshot of latency measurements. The sample set is copied when
/// the snapshot is created, so later caller mutations cannot change results.
/// </summary>
public sealed class LatencyStatistics
{
    private readonly double[] _samples;

    public LatencyStatistics(IEnumerable<double> samples)
    {
        ArgumentNullException.ThrowIfNull(samples);

        _samples = samples.ToArray();

        if (_samples.Length == 0)
        {
            throw new ArgumentException("At least one sample is required.", nameof(samples));
        }

        if (_samples.Any(value => !double.IsFinite(value) || value < 0))
        {
            throw new ArgumentException("Latency samples must be finite and non-negative.", nameof(samples));
        }

        Array.Sort(_samples);

        Count = _samples.Length;
        Minimum = _samples[0];
        Maximum = _samples[^1];
        Average = _samples.Average();
    }

    public int Count { get; }

    public double Minimum { get; }

    public double Maximum { get; }

    public double Average { get; }

    public double Median => Percentiles.Calculate(_samples, 50);

    public IReadOnlyList<double> Samples =>
        Array.AsReadOnly(_samples);

    public IReadOnlyList<double> GetPercentiles(params double[] percentiles) =>
        Percentiles.CalculateMany(_samples, percentiles);

    public double P50 => Median;

    public double P95 => Percentiles.Calculate(_samples, 95);

    public double P99 => Percentiles.Calculate(_samples, 99);

    public double Percentile(double percentile) =>
        Percentiles.Calculate(_samples, percentile);
}
