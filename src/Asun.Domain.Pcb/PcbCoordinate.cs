namespace Asun.Domain.Pcb;

public readonly record struct PcbCoordinate(
    double Xmm,
    double Ymm)
{
    public bool IsFinite =>
        double.IsFinite(Xmm) &&
        double.IsFinite(Ymm);

    public static PcbCoordinate Zero => new(0, 0);

    public double DistanceSquaredTo(PcbCoordinate other)
    {
        EnsureFinite();
        other.EnsureFinite();

        var dx = Xmm - other.Xmm;
        var dy = Ymm - other.Ymm;
        return dx * dx + dy * dy;
    }

    public double DistanceTo(PcbCoordinate other) =>
        Math.Sqrt(DistanceSquaredTo(other));

    public PcbCoordinate Translate(double deltaXmm, double deltaYmm)
    {
        EnsureFinite();

        if (!double.IsFinite(deltaXmm) ||
            !double.IsFinite(deltaYmm))
        {
            throw new ArgumentOutOfRangeException(nameof(deltaXmm));
        }

        var x = Xmm + deltaXmm;
        var y = Ymm + deltaYmm;

        if (!double.IsFinite(x) || !double.IsFinite(y))
            throw new ArgumentOutOfRangeException(nameof(deltaXmm));

        return new PcbCoordinate(x, y);
    }

    public PcbCoordinate Midpoint(PcbCoordinate other)
    {
        EnsureFinite();
        other.EnsureFinite();

        return new PcbCoordinate(
            (Xmm + other.Xmm) / 2d,
            (Ymm + other.Ymm) / 2d);
    }

    private void EnsureFinite()
    {
        if (!IsFinite)
            throw new InvalidOperationException("PCB coordinate must be finite.");
    }
}
