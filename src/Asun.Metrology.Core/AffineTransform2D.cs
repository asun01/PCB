namespace Asun.Metrology.Core;

public readonly record struct AffineTransform2D(
    double M11,
    double M12,
    double M21,
    double M22,
    double Tx,
    double Ty)
{
    public static AffineTransform2D Identity=>
        new(1,0,0,1,0,0);

    public bool IsFinite=>
        double.IsFinite(M11) &&
        double.IsFinite(M12) &&
        double.IsFinite(M21) &&
        double.IsFinite(M22) &&
        double.IsFinite(Tx) &&
        double.IsFinite(Ty);

    public double Determinant=>
        M11*M22-M12*M21;

    public MetrologyPoint2D Transform(MetrologyPoint2D point)
    {
        if(!IsFinite)
            throw new InvalidOperationException("Affine transform must be finite.");

        if(!point.IsFinite)
            throw new ArgumentException("Point must be finite.",nameof(point));

        var result=new MetrologyPoint2D(
            M11*point.X+M12*point.Y+Tx,
            M21*point.X+M22*point.Y+Ty);

        if(!result.IsFinite)
            throw new InvalidOperationException("Affine transform produced a non-finite point.");

        return result;
    }

    public bool TryInvert(
        out AffineTransform2D inverse,
        double determinantThreshold=1e-12)
    {
        inverse=Identity;

        if(!IsFinite ||
           !double.IsFinite(determinantThreshold) ||
           determinantThreshold<=0)
        {
            return false;
        }

        var determinant=Determinant;

        if(!double.IsFinite(determinant) ||
           Math.Abs(determinant)<determinantThreshold)
        {
            return false;
        }

        var inv11=M22/determinant;
        var inv12=-M12/determinant;
        var inv21=-M21/determinant;
        var inv22=M11/determinant;

        inverse=new AffineTransform2D(
            inv11,
            inv12,
            inv21,
            inv22,
            -(inv11*Tx+inv12*Ty),
            -(inv21*Tx+inv22*Ty));

        return inverse.IsFinite;
    }

    public AffineTransform2D Then(
        AffineTransform2D next)
    {
        if(!IsFinite || !next.IsFinite)
            throw new InvalidOperationException("Affine transform must be finite.");

        return new AffineTransform2D(
            next.M11*M11+next.M12*M21,
            next.M11*M12+next.M12*M22,
            next.M21*M11+next.M22*M21,
            next.M21*M12+next.M22*M22,
            next.M11*Tx+next.M12*Ty+next.Tx,
            next.M21*Tx+next.M22*Ty+next.Ty);
    }
}
