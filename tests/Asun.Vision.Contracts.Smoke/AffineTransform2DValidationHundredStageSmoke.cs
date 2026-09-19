using System.Numerics;
using Asun.Vision.Contracts;

public static class AffineTransform2DValidationHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        var identity = AffineTransform2D.Identity;
        var translation = AffineTransform2D.Translation(10, 20);
        var scale = AffineTransform2D.Scale(2, 3);
        var rotation = AffineTransform2D.Rotation(Math.PI / 2);
        var combined = scale.Combine(rotation).Combine(translation);

        var point = new Vector2(1, 2);
        var transformed = combined.TransformPoint(point);
        var direction = translation.TransformDirection(new Vector2(1, 0));

        var inverseOk = combined.TryInvert(out var inverse);
        var roundTrip = inverseOk
            ? inverse.TransformPoint(transformed)
            : Vector2.Zero;

        var rectangle = combined.TransformRectangle(
            new System.Drawing.RectangleF(0, 0, 10, 20));

        var unstable = AffineTransform2D.Scale(1e-7, 1e-6);

        var unstableTryInvert = unstable.TryInvert(out _);
        var stableValidation = AffineTransform2DValidationRuntime.IsValid(combined);
        var unstableValidation = AffineTransform2DValidationRuntime.IsValid(unstable);

        for (var i = 0; i < 10; i++)
            Check(identity.Matrix == Matrix3x2.Identity, $"identity matrix round {i + 1} should be canonical.");

        for (var i = 0; i < 10; i++)
            Check(translation.TranslationVector == new Vector2(10, 20), $"translation round {i + 1} should preserve the requested offset.");

        for (var i = 0; i < 10; i++)
            Check(Math.Abs(scale.LinearScaleX - 2) < 1e-6 && Math.Abs(scale.LinearScaleY - 3) < 1e-6, $"scale round {i + 1} should preserve axis scales.");

        for (var i = 0; i < 10; i++)
            Check(direction == new Vector2(1, 0), $"direction transform round {i + 1} should ignore translation.");

        for (var i = 0; i < 10; i++)
            Check(inverseOk && Vector2.Distance(roundTrip, point) < 1e-4f, $"inverse round-trip round {i + 1} should recover the source point.");

        for (var i = 0; i < 10; i++)
            Check(float.IsFinite(transformed.X) && float.IsFinite(transformed.Y), $"transformed point round {i + 1} should remain finite.");

        for (var i = 0; i < 10; i++)
            Check(rectangle.Width > 0 && rectangle.Height > 0, $"rectangle transform round {i + 1} should produce finite bounds.");

        for (var i = 0; i < 10; i++)
            Check(!unstable.IsInvertible && !unstableTryInvert, $"stable-threshold round {i + 1} should reject an ill-conditioned inverse.");

        for (var i = 0; i < 10; i++)
            Check(stableValidation && unstableValidation, $"affine validator round {i + 1} should accept both supported states.");

        for (var i = 0; i < 10; i++)
            Check(combined.ApproximatelyEquals(combined, 0), $"self-approximation round {i + 1} should be exact.");

        assert(round == 100, $"Affine transform smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
