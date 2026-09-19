using System.Numerics;

namespace Asun.UI.Viewports;

/// <summary>
/// Deterministic ROI hit-testing with explicit handle precedence.
/// Tolerance is expressed in the same coordinate system as the ROI.
/// </summary>
public static class RoiHitTester
{
    public static RoiHitResult HitTest(
        RoiGeometry roi,
        Vector2 point,
        float handleTolerance = 8f,
        float bodyTolerance = 0f)
    {
        ArgumentNullException.ThrowIfNull(roi);

        if (!IsFinite(point))
            throw new ArgumentOutOfRangeException(nameof(point));

        if (!float.IsFinite(handleTolerance) || handleTolerance < 0f)
            throw new ArgumentOutOfRangeException(nameof(handleTolerance));

        if (!float.IsFinite(bodyTolerance) || bodyTolerance < 0f)
            throw new ArgumentOutOfRangeException(nameof(bodyTolerance));

        var controls = roi.GetControlPoints();
        var best = RoiHitResult.None;

        foreach (var control in controls)
        {
            var distance = Vector2.Distance(point, control.Position);
            if (distance <= handleTolerance &&
                (!best.Hit || distance < best.Distance))
            {
                best = new RoiHitResult(
                    true,
                    control.Kind,
                    control.Index,
                    distance);
            }
        }

        if (best.Hit)
            return best;

        if (roi.Contains(point, bodyTolerance))
            return new RoiHitResult(
                true,
                RoiHandleKind.Body,
                -1,
                0f);

        return RoiHitResult.None;
    }

    public static bool IsResizeHandle(RoiHandleKind handle) =>
        handle is RoiHandleKind.TopLeft or
            RoiHandleKind.Top or
            RoiHandleKind.TopRight or
            RoiHandleKind.Right or
            RoiHandleKind.BottomRight or
            RoiHandleKind.Bottom or
            RoiHandleKind.BottomLeft or
            RoiHandleKind.Left;

    public static bool IsTransformHandle(RoiHandleKind handle) =>
        IsResizeHandle(handle) || handle == RoiHandleKind.Rotation;

    public static Vector2 GetHandleDirection(RoiHandleKind handle)
    {
        return handle switch
        {
            RoiHandleKind.TopLeft => new Vector2(-1f, -1f),
            RoiHandleKind.Top => new Vector2(0f, -1f),
            RoiHandleKind.TopRight => new Vector2(1f, -1f),
            RoiHandleKind.Right => new Vector2(1f, 0f),
            RoiHandleKind.BottomRight => new Vector2(1f, 1f),
            RoiHandleKind.Bottom => new Vector2(0f, 1f),
            RoiHandleKind.BottomLeft => new Vector2(-1f, 1f),
            RoiHandleKind.Left => new Vector2(-1f, 0f),
            _ => Vector2.Zero
        };
    }

    private static bool IsFinite(Vector2 value) =>
        float.IsFinite(value.X) && float.IsFinite(value.Y);
}
