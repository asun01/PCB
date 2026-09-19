namespace Asun.Metrology.Core;

public readonly record struct MetrologyCoordinateSystem2D(
    MetrologyPoint2D Origin,
    MetrologyPoint2D XAxis,
    MetrologyPoint2D YAxis)
{
    public bool IsValid
    {
        get
        {
            if(!Origin.IsFinite || !XAxis.IsFinite || !YAxis.IsFinite)
                return false;

            var xLength=Math.Sqrt(XAxis.X*XAxis.X+XAxis.Y*XAxis.Y);
            var yLength=Math.Sqrt(YAxis.X*YAxis.X+YAxis.Y*YAxis.Y);
            var dot=XAxis.X*YAxis.X+XAxis.Y*YAxis.Y;

            return xLength>0 &&
                   yLength>0 &&
                   Math.Abs(xLength-1)<1e-9 &&
                   Math.Abs(yLength-1)<1e-9 &&
                   Math.Abs(dot)<1e-9;
        }
    }

    public MetrologyPoint2D ToLocal(MetrologyPoint2D point)
    {
        if(!IsValid)
            throw new InvalidOperationException("Coordinate system is invalid.");
        if(!point.IsFinite)
            throw new ArgumentException("Point must be finite.",nameof(point));

        var dx=point.X-Origin.X;
        var dy=point.Y-Origin.Y;

        return new MetrologyPoint2D(
            dx*XAxis.X+dy*XAxis.Y,
            dx*YAxis.X+dy*YAxis.Y);
    }

    public MetrologyPoint2D ToWorld(MetrologyPoint2D local)
    {
        if(!IsValid)
            throw new InvalidOperationException("Coordinate system is invalid.");
        if(!local.IsFinite)
            throw new ArgumentException("Point must be finite.",nameof(local));

        return new MetrologyPoint2D(
            Origin.X+local.X*XAxis.X+local.Y*YAxis.X,
            Origin.Y+local.X*XAxis.Y+local.Y*YAxis.Y);
    }
}
