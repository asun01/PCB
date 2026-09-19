namespace Asun.Vision.Contracts;

public static class ImageGeometryValidationRuntime
{
    public static IReadOnlyList<string> Validate(ImageSize image)
    {
        var errors = new List<string>();

        if (image.Width <= 0 || image.Height <= 0)
            errors.Add("Image dimensions must be positive.");

        if (image.PixelCount <= 0)
            errors.Add("Image pixel count must be positive.");

        if (!double.IsFinite(image.AspectRatio) ||
            !double.IsFinite(image.DiagonalLength))
        {
            errors.Add("Image geometric summaries must remain finite.");
        }

        return errors;
    }

    public static IReadOnlyList<string> Validate(PixelRect rectangle)
    {
        var errors = new List<string>();

        if (!rectangle.IsValid || !rectangle.AreBoundsFinite)
            errors.Add("Pixel rectangle must remain finite and valid.");

        if (rectangle.IsValid && rectangle.Area < 0)
            errors.Add("Pixel rectangle area cannot be negative.");

        return errors;
    }

    public static bool IsValid(ImageSize image) =>
        Validate(image).Count == 0;

    public static bool IsValid(PixelRect rectangle) =>
        Validate(rectangle).Count == 0;
}
