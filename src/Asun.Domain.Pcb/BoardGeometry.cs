namespace Asun.Domain.Pcb;

/// <summary>
/// Minimal immutable board geometry used by non-vendor-dependent domain logic.
/// Units are deliberately left to the caller's approved measurement contract.
/// </summary>
public readonly record struct BoardSize
{
    public BoardSize(double width, double height)
    {
        if (!double.IsFinite(width) || width <= 0)
            throw new ArgumentOutOfRangeException(nameof(width));
        if (!double.IsFinite(height) || height <= 0)
            throw new ArgumentOutOfRangeException(nameof(height));

        Width = width;
        Height = height;
    }

    public double Width { get; }

    public double Height { get; }
}

public readonly record struct BoardPoint(double X, double Y)
{
    public bool IsFinite => double.IsFinite(X) && double.IsFinite(Y);
}

public sealed record BoardRegion(string Id, BoardPoint Origin, BoardSize Size)
{
    public BoardPoint Center => new(
        Origin.X + Size.Width / 2d,
        Origin.Y + Size.Height / 2d);
}
