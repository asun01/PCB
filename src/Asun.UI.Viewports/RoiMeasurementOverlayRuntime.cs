using System.Numerics;
using System.Globalization;

namespace Asun.UI.Viewports;

public readonly record struct RoiMeasurementOverlay(
    Guid Id,
    string Width,
    string Height,
    string Area,
    string Center);

public static class RoiMeasurementOverlayRuntime
{
    public static RoiMeasurementOverlay Build(
        RoiDocumentItem item, int decimals = 3)
    {
        if (decimals < 0 || decimals > 8) throw new ArgumentOutOfRangeException(nameof(decimals));
        var b = item.Geometry.GetBounds();
        var area = Math.Abs(PolygonArea(item.Geometry));
        var f = "F" + decimals;
        return new RoiMeasurementOverlay(
            item.Id,
            b.Width.ToString(f, CultureInfo.InvariantCulture),
            b.Height.ToString(f, CultureInfo.InvariantCulture),
            area.ToString(f, CultureInfo.InvariantCulture),
            $"({item.Geometry.Center.X.ToString(f, CultureInfo.InvariantCulture)}, {item.Geometry.Center.Y.ToString(f, CultureInfo.InvariantCulture)})");
    }

    private static double PolygonArea(RoiGeometry g)
    {
        if (!g.IsPolygon) return g.Size.X * g.Size.Y * (g.IsEllipse ? Math.PI / 4d : 1d);
        double a = 0;
        for (var i = 0; i < g.Vertices.Count; i++)
        {
            var p = g.Vertices[i];
            var q = g.Vertices[(i + 1) % g.Vertices.Count];
            a += (double)p.X * q.Y - (double)q.X * p.Y;
        }
        return a / 2d;
    }
}
