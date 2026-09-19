using System.Numerics;

namespace Asun.Vision.Contracts;

public static class AffineTransform2DValidationRuntime
{
    public static IReadOnlyList<string> Validate(AffineTransform2D transform)
    {
        var errors = new List<string>();

        var matrix = transform.Matrix;

        if (!IsFinite(matrix))
            errors.Add("Affine matrix must remain finite.");

        if (!transform.IsInvertible && transform.TryInvert(out _))
            errors.Add("Affine inversion disagrees with the invertibility contract.");

        if (transform.IsInvertible && !transform.TryInvert(out _))
            errors.Add("Invertible affine transforms must be safely invertible.");

        return errors;
    }

    public static bool IsValid(AffineTransform2D transform) =>
        Validate(transform).Count == 0;

    private static bool IsFinite(Matrix3x2 matrix) =>
        float.IsFinite(matrix.M11) &&
        float.IsFinite(matrix.M12) &&
        float.IsFinite(matrix.M21) &&
        float.IsFinite(matrix.M22) &&
        float.IsFinite(matrix.M31) &&
        float.IsFinite(matrix.M32);
}
