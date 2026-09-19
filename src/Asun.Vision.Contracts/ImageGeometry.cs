using System.Numerics;

namespace Asun.Vision.Contracts;

public readonly record struct ImageSize
{
    public ImageSize(int width, int height)
    {
        if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
        if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

        Width = width;
        Height = height;
    }

    public int Width { get; }
    public int Height { get; }
    public Vector2 Center => new(Width / 2f, Height / 2f);
}

public readonly record struct PixelPoint(double X, double Y)
{
    public bool IsFinite => double.IsFinite(X) && double.IsFinite(Y);
}

public readonly record struct PixelRect(double X, double Y, double Width, double Height)
{
    public bool IsFinite =>
        double.IsFinite(X) && double.IsFinite(Y) &&
        double.IsFinite(Width) && double.IsFinite(Height);

    public bool IsValid => IsFinite && Width >= 0 && Height >= 0;
    public PixelPoint Center => new(X + Width / 2d, Y + Height / 2d);
}
