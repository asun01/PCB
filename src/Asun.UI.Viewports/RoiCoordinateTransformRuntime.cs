using System.Numerics;

namespace Asun.UI.Viewports;

public static class RoiCoordinateTransformRuntime
{
    public static RoiGeometry ScaleToPhysical(
        RoiGeometry geometry,
        Vector2 unitsPerPixel)
    {
        ArgumentNullException.ThrowIfNull(geometry);
        if (!float.IsFinite(unitsPerPixel.X) || !float.IsFinite(unitsPerPixel.Y) ||
            unitsPerPixel.X <= 0 || unitsPerPixel.Y <= 0)
            throw new ArgumentOutOfRangeException(nameof(unitsPerPixel));

        var center = geometry.Center * unitsPerPixel;
        if (geometry.IsPolygon)
            return RoiGeometry.CreatePolygon(
                geometry.Vertices.Select(p => p * unitsPerPixel));

        var size = geometry.Size * unitsPerPixel;
        return geometry.Kind switch
        {
            RoiShapeKind.Rectangle => RoiGeometry.CreateRectangle(center, size),
            RoiShapeKind.RotatedRectangle => RoiGeometry.CreateRotatedRectangle(center, size, geometry.RotationRadians),
            RoiShapeKind.Ellipse => RoiGeometry.CreateEllipse(center, size, geometry.RotationRadians),
            _ => geometry
        };
    }
}
