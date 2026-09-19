using System.Numerics;

namespace Asun.Vision.Contracts;

/// <summary>
/// Immutable 2D affine transform for image, fixture, and measurement coordinate mapping.
/// It carries no calibration state or production schema semantics.
/// </summary>
public readonly record struct AffineTransform2D
{
    private readonly Matrix3x2 _matrix;

    private AffineTransform2D(Matrix3x2 matrix)
    {
        _matrix = matrix;
    }

    public static AffineTransform2D Identity => new(Matrix3x2.Identity);

    public static AffineTransform2D Create(Matrix3x2 matrix)
    {
        if (!IsFinite(matrix))
            throw new ArgumentOutOfRangeException(nameof(matrix), "Matrix must contain finite values.");

        return new AffineTransform2D(matrix);
    }

    public static AffineTransform2D Translation(double x, double y)
    {
        ValidateFinite(x, nameof(x));
        ValidateFinite(y, nameof(y));
        return new AffineTransform2D(Matrix3x2.CreateTranslation((float)x, (float)y));
    }

    public static AffineTransform2D Scale(double x, double y)
    {
        ValidateFinite(x, nameof(x));
        ValidateFinite(y, nameof(y));
        return new AffineTransform2D(Matrix3x2.CreateScale((float)x, (float)y));
    }

    public static AffineTransform2D UniformScale(double scale) =>
        Scale(scale, scale);

    public static AffineTransform2D Rotation(double radians)
    {
        ValidateFinite(radians, nameof(radians));
        return new AffineTransform2D(Matrix3x2.CreateRotation((float)radians));
    }

    public Vector2 TransformPoint(Vector2 point)
    {
        ValidateFinite(point, nameof(point));
        return Vector2.Transform(point, _matrix);
    }

    public Vector2 TransformDirection(Vector2 direction)
    {
        ValidateFinite(direction, nameof(direction));

        var transformed = Vector2.TransformNormal(direction, _matrix);

        if (!IsFinite(transformed))
            throw new InvalidOperationException("The transformed direction is non-finite.");

        return transformed;
    }

    public RectangleF TransformRectangle(System.Drawing.RectangleF rectangle)
    {
        if (!float.IsFinite(rectangle.X) ||
            !float.IsFinite(rectangle.Y) ||
            !float.IsFinite(rectangle.Width) ||
            !float.IsFinite(rectangle.Height) ||
            rectangle.Width < 0 ||
            rectangle.Height < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(rectangle));
        }

        var p1 = TransformPoint(new Vector2(rectangle.Left, rectangle.Top));
        var p2 = TransformPoint(new Vector2(rectangle.Right, rectangle.Top));
        var p3 = TransformPoint(new Vector2(rectangle.Right, rectangle.Bottom));
        var p4 = TransformPoint(new Vector2(rectangle.Left, rectangle.Bottom));

        return new System.Drawing.RectangleF(
            Math.Min(Math.Min(p1.X, p2.X), Math.Min(p3.X, p4.X)),
            Math.Min(Math.Min(p1.Y, p2.Y), Math.Min(p3.Y, p4.Y)),
            Math.Max(Math.Max(p1.X, p2.X), Math.Max(p3.X, p4.X)) -
                Math.Min(Math.Min(p1.X, p2.X), Math.Min(p3.X, p4.X)),
            Math.Max(Math.Max(p1.Y, p2.Y), Math.Max(p3.Y, p4.Y)) -
                Math.Min(Math.Min(p1.Y, p2.Y), Math.Min(p3.Y, p4.Y)));
    }

    /// <summary>
    /// Returns a transform that applies this transform first and <paramref name="next"/> second.
    /// </summary>
    public AffineTransform2D Combine(AffineTransform2D next)
    {
        var combined = Matrix3x2.Multiply(_matrix, next._matrix);

        if (!IsFinite(combined))
            throw new InvalidOperationException("Combined transform is non-finite.");

        return new AffineTransform2D(combined);
    }

    public bool TryInvert(out AffineTransform2D inverse)
    {
        if (!Matrix3x2.Invert(_matrix, out var matrix))
        {
            inverse = default;
            return false;
        }

        inverse = new AffineTransform2D(matrix);
        return true;
    }

    public AffineTransform2D Inverse
    {
        get
        {
            if (!TryInvert(out var inverse))
                throw new InvalidOperationException("The transform is not invertible.");

            return inverse;
        }
    }

    public Vector2 TranslationVector =>
        new(_matrix.M31, _matrix.M32);

    public double LinearDeterminant =>
        (double)_matrix.M11 * _matrix.M22 -
        (double)_matrix.M12 * _matrix.M21;

    public double Determinant => LinearDeterminant;

    public bool IsInvertible => Math.Abs(Determinant) > 1e-12;

    public Matrix3x2 Matrix => _matrix;

    public double LinearScaleX => Math.Sqrt(
        (double)_matrix.M11 * _matrix.M11 +
        (double)_matrix.M12 * _matrix.M12);

    public double LinearScaleY => Math.Sqrt(
        (double)_matrix.M21 * _matrix.M21 +
        (double)_matrix.M22 * _matrix.M22);

    private static void ValidateFinite(Vector2 value, string parameterName)
    {
        if (!IsFinite(value))
            throw new ArgumentOutOfRangeException(parameterName, "Point must contain finite values.");
    }

    private static void ValidateFinite(double value, string parameterName)
    {
        if (!double.IsFinite(value))
            throw new ArgumentOutOfRangeException(parameterName, value, "Value must be finite.");
    }

    public bool ApproximatelyEquals(
        AffineTransform2D other,
        float tolerance = 1e-5f)
    {
        if (!float.IsFinite(tolerance) || tolerance < 0)
            throw new ArgumentOutOfRangeException(nameof(tolerance));

        return MathF.Abs(_matrix.M11 - other._matrix.M11) <= tolerance &&
               MathF.Abs(_matrix.M12 - other._matrix.M12) <= tolerance &&
               MathF.Abs(_matrix.M21 - other._matrix.M21) <= tolerance &&
               MathF.Abs(_matrix.M22 - other._matrix.M22) <= tolerance &&
               MathF.Abs(_matrix.M31 - other._matrix.M31) <= tolerance &&
               MathF.Abs(_matrix.M32 - other._matrix.M32) <= tolerance;
    }

    private static bool IsFinite(Matrix3x2 matrix) =>
        float.IsFinite(matrix.M11) &&
        float.IsFinite(matrix.M12) &&
        float.IsFinite(matrix.M21) &&
        float.IsFinite(matrix.M22) &&
        float.IsFinite(matrix.M31) &&
        float.IsFinite(matrix.M32);

    private static bool IsFinite(Vector2 value) =>
        float.IsFinite(value.X) && float.IsFinite(value.Y);
}
