namespace Asun.UI.Viewports.AdvancedChains;

public readonly record struct AdvancedChainResult(
    double Value,
    int Steps,
    bool Valid);

internal static class AdvancedChainMath
{
    public static double Validate(double value)
    {
        if (!double.IsFinite(value))
            throw new ArgumentOutOfRangeException(nameof(value));

        return value;
    }

    public static double Clamp(double value, double min, double max) =>
        Math.Clamp(Validate(value), min, max);

    public static double Normalize(double value)
    {
        value = Validate(value);
        return Math.Abs(value) <= 1e-12 ? 0d : value;
    }

    public static double Quantize(double value, double step)
    {
        value = Validate(value);
        if (!double.IsFinite(step) || step <= 0)
            throw new ArgumentOutOfRangeException(nameof(step));

        return Math.Round(value / step, MidpointRounding.AwayFromZero) * step;
    }

    public static double Scale(double value, double factor) =>
        Validate(value) * factor;

    public static double Blend(double current, double target, double weight)
    {
        current = Validate(current);
        target = Validate(target);
        if (!double.IsFinite(weight) || weight < 0 || weight > 1)
            throw new ArgumentOutOfRangeException(nameof(weight));

        return current + (target - current) * weight;
    }

    public static double Finish(double value) =>
        double.IsFinite(value)
            ? value
            : throw new InvalidOperationException("Advanced chain produced a non-finite result.");
}
