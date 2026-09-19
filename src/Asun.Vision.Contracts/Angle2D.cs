namespace Asun.Vision.Contracts;

/// <summary>
/// Immutable angle value with deterministic normalization and shortest-delta helpers.
/// </summary>
public readonly record struct Angle2D(double Radians)
{
    public bool IsFinite => double.IsFinite(Radians);

    public static Angle2D FromVector(System.Numerics.Vector2 direction)
    {
        if (!float.IsFinite(direction.X) || !float.IsFinite(direction.Y))
            throw new ArgumentOutOfRangeException(nameof(direction));

        if (direction == System.Numerics.Vector2.Zero)
            throw new ArgumentException("Direction vector must be non-zero.", nameof(direction));

        return new Angle2D(Math.Atan2(direction.Y, direction.X));
    }
    public static Angle2D Zero => new(0);

    public static Angle2D FromDegrees(double degrees)
    {
        if (!double.IsFinite(degrees))
            throw new ArgumentOutOfRangeException(nameof(degrees));

        return new Angle2D(degrees * Math.PI / 180d);
    }

    public double Degrees =>
        double.IsFinite(Radians)
            ? Radians * 180d / Math.PI
            : throw new InvalidOperationException("The angle is non-finite.");

    public double NormalizedRadians =>
        Normalize(Radians);

    public Angle2D Normalized =>
        new(NormalizedRadians);

    public Angle2D Opposite =>
        new(NormalizedRadians + Math.PI).Normalized;

    public double ShortestDeltaTo(Angle2D target) =>
        Normalize(target.Radians - Radians);

    public bool ApproximatelyEquals(
        Angle2D other,
        double toleranceRadians)
    {
        if (!double.IsFinite(toleranceRadians) || toleranceRadians < 0)
            throw new ArgumentOutOfRangeException(nameof(toleranceRadians));

        return Math.Abs(ShortestDeltaTo(other)) <= toleranceRadians;
    }

    public Angle2D Subtract(double radians) =>
        Add(-radians);

    public Angle2D Add(double radians)
    {
        if (!double.IsFinite(radians))
            throw new ArgumentOutOfRangeException(nameof(radians));

        return new Angle2D(Radians + radians);
    }

    public static double Normalize(double radians)
    {
        if (!double.IsFinite(radians))
            throw new ArgumentOutOfRangeException(nameof(radians));

        var normalized = radians % (Math.PI * 2d);

        if (normalized <= -Math.PI)
            normalized += Math.PI * 2d;
        else if (normalized > Math.PI)
            normalized -= Math.PI * 2d;

        return normalized;
    }
}
