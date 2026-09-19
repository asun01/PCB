namespace Asun.Domain.Pcb;

/// <summary>
/// Minimal immutable board geometry used by non-vendor-dependent domain logic.
/// Units are deliberately left to the caller's approved measurement contract.
/// </summary>
public readonly record struct BoardSize(double Width, double Height)
{
    public BoardSize
    {
        if (!double.IsFinite(Width) || Width <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(Width));
        }

        if (!double.IsFinite(Height) || Height <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(Height));
        }
    }
}

public readonly record struct BoardPoint(double X, double Y)
{
    public bool IsFinite => double.IsFinite(X) && double.IsFinite(Y);
}

public sealed record BoardRegion(
    string Id,
    BoardPoint Origin,
    BoardSize Size)
{
    public BoardPoint Center => new(
        Origin.X + Size.Width / 2d,
        Origin.Y + Size.Height / 2d);
}
