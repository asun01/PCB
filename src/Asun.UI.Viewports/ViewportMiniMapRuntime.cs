using System.Drawing;
using System.Numerics;

namespace Asun.UI.Viewports;

public readonly record struct ViewportMiniMapSnapshot(
    ViewportTransform MiniMapTransform,
    RectangleF VisibleImageRectangle,
    RectangleF ViewportRectangle);

public sealed class ViewportMiniMapRuntime
{
    public static ViewportMiniMapSnapshot CreateSnapshot(
        ViewportTransform mainTransform,
        Vector2 miniMapSize,
        float padding = 4f)
    {
        ValidateSize(miniMapSize);

        if (!float.IsFinite(padding) || padding < 0)
            throw new ArgumentOutOfRangeException(nameof(padding));

        var innerSize = new Vector2(
            Math.Max(1f, miniMapSize.X - padding * 2f),
            Math.Max(1f, miniMapSize.Y - padding * 2f));

        var mapTransform = ViewportTransform.Fit(
            mainTransform.ImageSize,
            innerSize);

        mapTransform = mapTransform.WithTranslation(
            mapTransform.Translation + new Vector2(padding, padding));

        var visible = mainTransform.GetVisibleImageRectangle();

        var topLeft = mapTransform.ImageToViewport(
            new Vector2(visible.Left, visible.Top));
        var bottomRight = mapTransform.ImageToViewport(
            new Vector2(visible.Right, visible.Bottom));

        var viewportRectangle = RectangleF.FromLTRB(
            topLeft.X,
            topLeft.Y,
            bottomRight.X,
            bottomRight.Y);

        return new ViewportMiniMapSnapshot(
            mapTransform,
            visible,
            viewportRectangle);
    }

    public static Vector2 MiniMapToImage(
        ViewportMiniMapSnapshot snapshot,
        Vector2 miniMapPoint)
    {
        if (!float.IsFinite(miniMapPoint.X) || !float.IsFinite(miniMapPoint.Y))
            throw new ArgumentOutOfRangeException(nameof(miniMapPoint));

        return snapshot.MiniMapTransform.ViewportToImage(miniMapPoint);
    }

    public static ViewportTransform CenterMainViewportOnMiniMapPoint(
        ViewportTransform mainTransform,
        ViewportMiniMapSnapshot snapshot,
        Vector2 miniMapPoint)
    {
        var imagePoint = MiniMapToImage(snapshot, miniMapPoint);
        return mainTransform.CenterOnImagePoint(imagePoint);
    }

    private static void ValidateSize(Vector2 size)
    {
        if (!float.IsFinite(size.X) || !float.IsFinite(size.Y) ||
            size.X <= 0 || size.Y <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(size));
        }
    }
}
