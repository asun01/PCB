namespace Asun.Platform.Core;

/// <summary>
/// Online numerical statistics using Welford's stable accumulation algorithm.
/// The type has no domain acceptance semantics.
/// </summary>
public sealed class RunningStatistics
{
    private long _count;
    private double _mean;
    private double _m2;
    private double _minimum = double.PositiveInfinity;
    private double _maximum = double.NegativeInfinity;

    public long Count => _count;

    public double Sum =>
        _count == 0
            ? 0
            : _mean * _count;

    public double Mean =>
        _count == 0
            ? throw new InvalidOperationException("No samples have been added.")
            : _mean;

    public double Minimum =>
        _count == 0
            ? throw new InvalidOperationException("No samples have been added.")
            : _minimum;

    public double Maximum =>
        _count == 0
            ? throw new InvalidOperationException("No samples have been added.")
            : _maximum;

    public double VariancePopulation =>
        _count == 0
            ? throw new InvalidOperationException("No samples have been added.")
            : _m2 / _count;

    public double VarianceSample =>
        _count < 2
            ? throw new InvalidOperationException("At least two samples are required.")
            : _m2 / (_count - 1);

    public double StandardDeviationPopulation =>
        Math.Sqrt(VariancePopulation);

    public double StandardDeviationSample =>
        Math.Sqrt(VarianceSample);

    public void Add(double value)
    {
        if (!double.IsFinite(value))
            throw new ArgumentOutOfRangeException(nameof(value));

        _count++;

        var delta = value - _mean;
        _mean += delta / _count;
        var delta2 = value - _mean;
        _m2 += delta * delta2;

        _minimum = Math.Min(_minimum, value);
        _maximum = Math.Max(_maximum, value);
    }

    public void AddRange(IEnumerable<double> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        foreach (var value in values)
            Add(value);
    }

    public bool TryGetMean(out double mean)
    {
        if (_count == 0)
        {
            mean = 0;
            return false;
        }

        mean = _mean;
        return true;
    }

    public void Reset()
    {
        _count = 0;
        _mean = 0;
        _m2 = 0;
        _minimum = double.PositiveInfinity;
        _maximum = double.NegativeInfinity;
    }
}
