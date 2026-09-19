using Asun.Vision.Contracts;

public static class ImageGeometryValidationHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        var image = new ImageSize(100, 50);
        var first = PixelPoint(10, 20);
        var second = PixelPoint(40, 60);
        var rectangle = PixelRect.FromPoints(first, second);
        var other = PixelRect.FromCenter(new PixelPoint(35, 40), 20, 20);
        var intersection = rectangle.Intersect(other);
        var union = rectangle.Union(other);
        var translated = rectangle.Translate(5, -5);
        var scaled = rectangle.ScaleAroundCenter(2, 0.5);
        var clamped = union.ClampTo(new PixelRect(0, 0, 50, 50));
        var inflated = rectangle.Inflate(2, 3);
        var midpoint = first.Midpoint(second);
        var lerp = first.Lerp(second, 0.5);

        var imageValid = ImageGeometryValidationRuntime.IsValid(image);
        var rectValid = ImageGeometryValidationRuntime.IsValid(rectangle);

        for (var i = 0; i < 10; i++)
            Check(imageValid && image.PixelCount == 5000, $"image validation round {i + 1} should be stable.");

        for (var i = 0; i < 10; i++)
            Check(image.Contains(new PixelPoint(100, 50)) && !image.Contains(new PixelPoint(101, 50)), $"image bounds round {i + 1} should use canonical geometry bounds.");

        for (var i = 0; i < 10; i++)
            Check(rectValid && rectangle.Width == 30 && rectangle.Height == 40, $"rectangle construction round {i + 1} should preserve dimensions.");

        for (var i = 0; i < 10; i++)
            Check(rectangle.Contains(new PixelPoint(20, 30)) && !rectangle.Contains(new PixelPoint(100, 100)), $"rectangle containment round {i + 1} should be deterministic.");

        for (var i = 0; i < 10; i++)
            Check(intersection.IsValid && intersection.Width > 0 && intersection.Height > 0, $"intersection round {i + 1} should be valid.");

        for (var i = 0; i < 10; i++)
            Check(union.Contains(rectangle) && union.Contains(other), $"union round {i + 1} should contain both inputs.");

        for (var i = 0; i < 10; i++)
            Check(translated.Left == 15 && translated.Top == 15, $"translation round {i + 1} should move both coordinates.");

        for (var i = 0; i < 10; i++)
            Check(scaled.Width == 60 && scaled.Height == 20, $"scale round {i + 1} should preserve the center while changing size.");

        for (var i = 0; i < 10; i++)
            Check(clamped.Right <= 50 && clamped.Bottom <= 50, $"clamp round {i + 1} should remain inside bounds.");

        for (var i = 0; i < 10; i++)
            Check(inflated.Width == 34 && inflated.Height == 46 && midpoint == lerp, $"point arithmetic round {i + 1} should remain deterministic.");

        assert(round == 100, $"Image geometry smoke should execute exactly 100 numbered rounds; actual {round}.");

        static PixelPoint PixelPoint(double x, double y) => new(x, y);
    }
}
