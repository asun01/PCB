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

    public AffineTransform2D Combine(AffineTransform2D next)
    {
        var combined = Matrix3x2.Multiply(_matrix, next._matrix);

        if (!IsFinite(combined))
            throw new InvalidOperationException("Combined transform is non-finite.");

        return new AffineTransform2D(combined);
    }

    public AffineTransform2D Inverse
    {
        get
        {
            if (!Matrix3x2.Invert(_matrix, out var inverse))
                throw new InvalidOperationException("The transform is not invertible.");

            return new AffineTransform2D(inverse);
        }
    }

    public double Determinant =>
        (double)_matrix.M11 * _matrix.M22 -
        (double)_matrix.M12 * _matrix.M21;

    public bool IsInvertible => Math.Abs(Determinant) > 1e-12;

    public Matrix3x2 Matrix => _matrix;

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
