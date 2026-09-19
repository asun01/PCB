namespace Asun.Metrology.Core;

public readonly record struct MetrologyPoint2D(
    double X,
    double Y)
{
    public static MetrologyPoint2D Zero=>new(0,0);

    public bool IsFinite=>
        double.IsFinite(X) &&
        double.IsFinite(Y);

    public double DistanceSquaredTo(MetrologyPoint2D other)
    {
        EnsureFinite();
        other.EnsureFinite();

        var dx=X-other.X;
        var dy=Y-other.Y;
        return dx*dx+dy*dy;
    }

    public double DistanceTo(MetrologyPoint2D other)=>
        Math.Sqrt(DistanceSquaredTo(other));

    public MetrologyPoint2D Translate(double dx,double dy)
    {
        EnsureFinite();

        if(!double.IsFinite(dx) || !double.IsFinite(dy))
            throw new ArgumentOutOfRangeException(nameof(dx));

        var result=new MetrologyPoint2D(X+dx,Y+dy);

        if(!result.IsFinite)
            throw new ArgumentOutOfRangeException(nameof(dx));

        return result;
    }

    private void EnsureFinite()
    {
        if(!IsFinite)
            throw new InvalidOperationException("Metrology point must be finite.");
    }
}
