using System.Drawing;
using System.Numerics;

namespace Asun.UI.Viewports;

public enum RoiRenderCommandKind
{
    Fill,
    Outline,
    Handle,
    RotationHandle,
    HoverOutline
}

public readonly record struct RoiRenderCommand(
    RoiRenderCommandKind Kind,
    Guid RoiId,
    bool Selected,
    bool Hovered,
    RoiShapeKind Shape,
    IReadOnlyList<Vector2> Points,
    RectangleF Bounds);

public static class RoiRenderCommandBuilder
{
    public static IReadOnlyList<RoiRenderCommand> Build(
        RoiViewportSnapshot snapshot,
        float handleRadiusPixels = 4f)
    {
        if (!float.IsFinite(handleRadiusPixels) || handleRadiusPixels <= 0)
            throw new ArgumentOutOfRangeException(nameof(handleRadiusPixels));

        var commands = new List<RoiRenderCommand>();

        foreach (var item in snapshot.Items.OrderBy(item => item.ZIndex))
        {
            var hover = snapshot.Hover.Id == item.Id && snapshot.Hover.Hit.Hit;

            commands.Add(new RoiRenderCommand(
                RoiRenderCommandKind.Fill,
                item.Id,
                item.IsSelected,
                hover,
                item.Geometry.Kind,
                item.Geometry.GetCornerPoints(),
                item.Bounds));

            commands.Add(new RoiRenderCommand(
                RoiRenderCommandKind.Outline,
                item.Id,
                item.IsSelected,
                hover,
                item.Geometry.Kind,
                item.Geometry.GetCornerPoints(),
                item.Bounds));

            if (hover && !item.IsSelected)
            {
                commands.Add(new RoiRenderCommand(
                    RoiRenderCommandKind.HoverOutline,
                    item.Id,
                    false,
                    true,
                    item.Geometry.Kind,
                    item.Geometry.GetCornerPoints(),
                    item.Bounds));
            }

            if (!item.IsSelected)
                continue;

            foreach (var control in item.ControlPoints)
            {
                commands.Add(new RoiRenderCommand(
                    control.Kind == RoiHandleKind.Rotation
                        ? RoiRenderCommandKind.RotationHandle
                        : RoiRenderCommandKind.Handle,
                    item.Id,
                    true,
                    hover,
                    item.Geometry.Kind,
                    new[] { control.Position },
                    new RectangleF(
                        control.Position.X - handleRadiusPixels,
                        control.Position.Y - handleRadiusPixels,
                        handleRadiusPixels * 2f,
                        handleRadiusPixels * 2f)));
            }
        }

        return commands;
    }
}
