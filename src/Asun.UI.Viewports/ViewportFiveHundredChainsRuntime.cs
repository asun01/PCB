using System.Drawing;
using System.Numerics;

namespace Asun.UI.Viewports;

public readonly record struct BulkChainOutcome(
    int Id,
    string Domain,
    bool Success,
    double Metric,
    Vector2 Point,
    RectangleF Bounds);

public sealed class BulkChainContext
{
    public BulkChainContext(
        ViewportTransform transform,
        RoiGeometry geometry,
        IReadOnlyList<RoiDocumentItem> items)
    {
        Transform = transform;
        Geometry = geometry;
        Items = items;
        Point = geometry.Center;
        Bounds = geometry.GetBounds();
    }

    public ViewportTransform Transform { get; }
    public RoiGeometry Geometry { get; }
    public IReadOnlyList<RoiDocumentItem> Items { get; }
    public Vector2 Point { get; }
    public RectangleF Bounds { get; }

    public static BulkChainContext CreateDefault()
    {
        var transform = ViewportTransform.Fit(
            new Vector2(2000, 1000),
            new Vector2(800, 600));

        var geometry = RoiGeometry.CreateRotatedRectangle(
            new Vector2(700, 420),
            new Vector2(240, 120),
            0.18f);

        var items = new[]
        {
            new RoiDocumentItem(Guid.NewGuid(), geometry, 0),
            new RoiDocumentItem(Guid.NewGuid(), geometry.Translate(new Vector2(400, 80)), 1),
            new RoiDocumentItem(Guid.NewGuid(), RoiGeometry.CreateEllipse(
                new Vector2(1100, 720), new Vector2(180, 90)), 2)
        };

        return new BulkChainContext(transform, geometry, items);
    }
}

internal static class BulkChainKernel
{
    public static BulkChainOutcome Execute(
        int id,
        string domain,
        BulkChainContext context,
        int mode,
        double parameter)
    {
        ArgumentNullException.ThrowIfNull(context);

        var point = context.Point;
        var bounds = context.Bounds;
        var metric = 0d;

        for (var stage = 0; stage < 5; stage++)
        {
            switch ((mode + stage) % 8)
            {
                case 0:
                    point = context.Transform.ImageToViewport(point);
                    point = context.Transform.ViewportToImage(point);
                    metric += point.Length();
                    break;
                case 1:
                    point = context.Geometry.Translate(
                        new Vector2(
                            (float)(parameter * (stage + 1)),
                            (float)(-parameter * stage)))
                        .Center;
                    metric += context.Geometry.GetBounds().Area();
                    break;
                case 2:
                    var projected = context.Transform.ImageToViewportRectangle(bounds);
                    metric += projected.Width * projected.Height;
                    point = new Vector2(projected.X + projected.Width / 2f, projected.Y + projected.Height / 2f);
                    break;
                case 3:
                    var local = context.Geometry.ToLocal(point);
                    point = context.Geometry.ToWorld(local);
                    metric += local.LengthSquared();
                    break;
                case 4:
                    var snap = RoiConstraintRuntime.Snap(
                        point,
                        new Vector2(
                            parameter <= 0 ? 1f : (float)parameter,
                            parameter <= 0 ? 1f : (float)parameter));
                    metric += Vector2.Distance(point, snap);
                    point = snap;
                    break;
                case 5:
                    var union = context.Items[0].Geometry.GetBounds();
                    foreach (var item in context.Items.Skip(1))
                        union = RectangleF.Union(union, item.Geometry.GetBounds());
                    bounds = union;
                    metric += union.Width + union.Height;
                    point = new Vector2(union.X + union.Width / 2f, union.Y + union.Height / 2f);
                    break;
                case 6:
                    var nearest = context.Items
                        .OrderBy(item => Vector2.DistanceSquared(item.Geometry.Center, point))
                        .First();
                    metric += Vector2.Distance(nearest.Geometry.Center, point);
                    point = nearest.Geometry.Center;
                    break;
                default:
                    var visible = context.Transform.GetVisibleImageRectangle();
                    metric += visible.Width * visible.Height;
                    bounds = visible;
                    point = visible.Center.ToVector2();
                    break;
            }
        }

        var finite = float.IsFinite(point.X) &&
                     float.IsFinite(point.Y) &&
                     double.IsFinite(metric) &&
                     float.IsFinite(bounds.X) &&
                     float.IsFinite(bounds.Y) &&
                     float.IsFinite(bounds.Width) &&
                     float.IsFinite(bounds.Height);

        return new BulkChainOutcome(
            id,
            domain,
            finite,
            metric + Math.Abs(parameter),
            point,
            bounds);
    }
}
