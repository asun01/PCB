using System.Numerics;

namespace Asun.UI.Viewports;

public readonly record struct RoiConstraintProfile(
    Vector2 MinimumSize,
    Vector2 MaximumSize,
    bool PreserveAspectRatio,
    Vector2 GridSpacing,
    bool SnapCenterToGrid,
    bool ClampToImageBounds);

public static class RoiConstraintRuntime
{
    public static RoiGeometry Apply(
        RoiGeometry geometry,
        RoiConstraintProfile profile,
        Vector2 imageSize)
    {
        ArgumentNullException.ThrowIfNull(geometry);
        ValidateSize(profile.MinimumSize, nameof(profile.MinimumSize));
        ValidateSize(profile.MaximumSize, nameof(profile.MaximumSize));
        ValidateSize(imageSize, nameof(imageSize));

        if (profile.MaximumSize.X < profile.MinimumSize.X ||
            profile.MaximumSize.Y < profile.MinimumSize.Y)
        {
            throw new ArgumentOutOfRangeException(nameof(profile));
        }

        if (profile.GridSpacing.X < 0 || profile.GridSpacing.Y < 0 ||
            !float.IsFinite(profile.GridSpacing.X) ||
            !float.IsFinite(profile.GridSpacing.Y))
        {
            throw new ArgumentOutOfRangeException(nameof(profile.GridSpacing));
        }

        var result = geometry;

        if (!geometry.IsPolygon)
        {
            var size = ClampSize(geometry.Size, profile.MinimumSize, profile.MaximumSize);

            if (profile.PreserveAspectRatio)
                size = PreserveRatio(size, profile.MinimumSize, profile.MaximumSize);

            var center = profile.SnapCenterToGrid
                ? Snap(geometry.Center, profile.GridSpacing)
                : geometry.Center;

            result = geometry.Kind switch
            {
                RoiShapeKind.Rectangle =>
                    RoiGeometry.CreateRectangle(center, size),
                RoiShapeKind.RotatedRectangle =>
                    RoiGeometry.CreateRotatedRectangle(center, size, geometry.RotationRadians),
                RoiShapeKind.Ellipse =>
                    RoiGeometry.CreateEllipse(center, size, geometry.RotationRadians),
                _ => geometry
            };
        }
        else if (profile.SnapCenterToGrid)
        {
            result = geometry.WithCenter(
                Snap(geometry.Center, profile.GridSpacing));
        }

        if (profile.ClampToImageBounds)
            result = ClampToBounds(result, imageSize);

        return result;
    }

    public static Vector2 ClampSize(
        Vector2 requested,
        Vector2 minimum,
        Vector2 maximum)
    {
        ValidateSize(requested, nameof(requested));
        ValidateSize(minimum, nameof(minimum));
        ValidateSize(maximum, nameof(maximum));

        return new Vector2(
            Math.Clamp(requested.X, minimum.X, maximum.X),
            Math.Clamp(requested.Y, minimum.Y, maximum.Y));
    }

    public static Vector2 PreserveRatio(
        Vector2 size,
        Vector2 minimum,
        Vector2 maximum)
    {
        ValidateSize(size, nameof(size));
        ValidateSize(minimum, nameof(minimum));
        ValidateSize(maximum, nameof(maximum));

        var ratio = size.X / size.Y;
        var width = size.X;
        var height = size.Y;

        if (width < minimum.X)
        {
            width = minimum.X;
            height = width / ratio;
        }

        if (height < minimum.Y)
        {
            height = minimum.Y;
            width = height * ratio;
        }

        if (width > maximum.X)
        {
            width = maximum.X;
            height = width / ratio;
        }

        if (height > maximum.Y)
        {
            height = maximum.Y;
            width = height * ratio;
        }

        return new Vector2(
            Math.Clamp(width, minimum.X, maximum.X),
            Math.Clamp(height, minimum.Y, maximum.Y));
    }

    public static Vector2 Snap(Vector2 point, Vector2 gridSpacing)
    {
        if (!IsFinite(point))
            throw new ArgumentOutOfRangeException(nameof(point));

        if (!float.IsFinite(gridSpacing.X) || !float.IsFinite(gridSpacing.Y) ||
            gridSpacing.X < 0 || gridSpacing.Y < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(gridSpacing));
        }

        return new Vector2(
            gridSpacing.X > 0 ? MathF.Round(point.X / gridSpacing.X) * gridSpacing.X : point.X,
            gridSpacing.Y > 0 ? MathF.Round(point.Y / gridSpacing.Y) * gridSpacing.Y : point.Y);
    }

    private static RoiGeometry ClampToBounds(
        RoiGeometry geometry,
        Vector2 imageSize)
    {
        var bounds = geometry.GetBounds();
        var dx = bounds.Left < 0
            ? -bounds.Left
            : bounds.Right > imageSize.X
                ? imageSize.X - bounds.Right
                : 0f;
        var dy = bounds.Top < 0
            ? -bounds.Top
            : bounds.Bottom > imageSize.Y
                ? imageSize.Y - bounds.Bottom
                : 0f;

        return geometry.Translate(new Vector2(dx, dy));
    }

    private static void ValidateSize(Vector2 size, string name)
    {
        if (!IsFinite(size) || size.X <= 0 || size.Y <= 0)
            throw new ArgumentOutOfRangeException(name);
    }

    private static bool IsFinite(Vector2 point) =>
        float.IsFinite(point.X) && float.IsFinite(point.Y);
}
