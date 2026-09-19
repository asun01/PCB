namespace Asun.Vision.Contracts;

/// <summary>
/// Small deterministic floating-point comparison helpers for geometry and vision calculations.
/// This type does not define measurement acceptance criteria.
/// </summary>
public readonly record struct NumericTolerance(
    double Absolute,
    double Relative)
{
    public static NumericTolerance Exact => new(0, 0);

    public static NumericTolerance Default => new(1e-9, 1e-9);

    public static NumericTolerance AbsoluteOnly(double absolute) =>
        new(
            ValidateNonNegativeFinite(absolute, nameof(absolute)),
            0);

    public static NumericTolerance RelativeOnly(double relative) =>
        new(
            0,
            ValidateNonNegativeFinite(relative, nameof(relative)));

    public bool IsValid =>
        double.IsFinite(Absolute) &&
        double.IsFinite(Relative) &&
        Absolute >= 0 &&
        Relative >= 0;

    public bool AreEqual(double left, double right)
    {
        if (!IsValid)
            throw new InvalidOperationException("Numeric tolerance is invalid.");

        if (!double.IsFinite(left) || !double.IsFinite(right))
            return false;

        if (left == right)
            return true;

        var difference = Math.Abs(left - right);
        var scale = Math.Max(Math.Abs(left), Math.Abs(right));

        return difference <= Absolute ||
               difference <= scale * Relative;
    }

    public bool IsNearlyZero(double value)
    {
        if (!IsValid)
            throw new InvalidOperationException("Numeric tolerance is invalid.");

        if (!double.IsFinite(value))
            return false;

        return Math.Abs(value) <= Absolute;
    }

    public double ClampNearZero(double value)
    {
        if (!double.IsFinite(value))
            throw new ArgumentOutOfRangeException(nameof(value));

        return IsNearlyZero(value) ? 0d : value;
    }

    public NumericTolerance WithAbsolute(double absolute) =>
        new(
            ValidateNonNegativeFinite(absolute, nameof(absolute)),
            Relative);

    public NumericTolerance WithRelative(double relative) =>
        new(
            Absolute,
            ValidateNonNegativeFinite(relative, nameof(relative)));

    private static double ValidateNonNegativeFinite(double value, string parameterName)
    {
        if (!double.IsFinite(value) || value < 0)
            throw new ArgumentOutOfRangeException(parameterName, value);

        return value;
    }
}
