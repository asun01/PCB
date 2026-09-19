namespace Asun.Metrology.Core;

public readonly record struct MetrologySegment2D(
    MetrologyPoint2D Start,
    MetrologyPoint2D End)
{
    public bool IsValid=>
        Start.IsFinite &&
        End.IsFinite;

    public double Length=>
        IsValid
            ? Start.DistanceTo(End)
            : throw new InvalidOperationException("Segment is invalid.");

    public MetrologyPoint2D Midpoint=>
        IsValid
            ? new MetrologyPoint2D(
                (Start.X+End.X)/2d,
                (Start.Y+End.Y)/2d)
            : throw new InvalidOperationException("Segment is invalid.");
}
