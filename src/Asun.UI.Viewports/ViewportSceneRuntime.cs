using System.Collections.Immutable;
using System.Drawing;
using System.Numerics;

namespace Asun.UI.Viewports;

public enum ViewportSceneCommandKind
{
    RoiBody,
    RoiSelectionBounds,
    RoiControlPoint
}

public readonly record struct ViewportSceneCommand(
    ViewportSceneCommandKind Kind,
    Guid RoiId,
    RoiHandleKind Handle,
    int HandleIndex,
    Vector2 Start,
    Vector2 End,
    bool Selected,
    int ZIndex);

public sealed class ViewportSceneSnapshot
{
    internal ViewportSceneSnapshot(
        ViewportTransform transform,
        IReadOnlyList<ViewportSceneCommand> commands)
    {
        Transform = transform;
        Commands = commands.ToImmutableArray();
    }

    public ViewportTransform Transform { get; }

    public IReadOnlyList<ViewportSceneCommand> Commands { get; }
}

/// <summary>
/// Builds deterministic framework-neutral render commands from the current ROI
/// viewport snapshot. A later Skia/WPF adapter can consume these commands directly.
/// </summary>
public static class ViewportSceneRuntime
{
    public static ViewportSceneSnapshot Build(RoiViewportSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        var commands = new List<ViewportSceneCommand>();

        foreach (var item in snapshot.Items)
        {
            var corners = item.Geometry.GetCornerPoints();

            if (corners.Count >= 2)
            {
                for (var i = 0; i < corners.Count; i++)
                {
                    var next = corners[(i + 1) % corners.Count];
                    commands.Add(
                        new ViewportSceneCommand(
                            ViewportSceneCommandKind.RoiBody,
                            item.Id,
                            RoiHandleKind.None,
                            i,
                            corners[i],
                            next,
                            item.IsSelected,
                            item.ZIndex));
                }
            }

            if (item.IsSelected)
            {
                var bounds = item.Bounds;
                commands.Add(
                    new ViewportSceneCommand(
                        ViewportSceneCommandKind.RoiSelectionBounds,
                        item.Id,
                        RoiHandleKind.None,
                        -1,
                        new Vector2(bounds.Left, bounds.Top),
                        new Vector2(bounds.Right, bounds.Bottom),
                        true,
                        item.ZIndex));
            }

            foreach (var control in item.ControlPoints)
            {
                commands.Add(
                    new ViewportSceneCommand(
                        ViewportSceneCommandKind.RoiControlPoint,
                        item.Id,
                        control.Kind,
                        control.Index,
                        control.Position,
                        control.Position,
                        item.IsSelected,
                        item.ZIndex));
            }
        }

        return new ViewportSceneSnapshot(
            snapshot.Transform,
            commands);
    }

    public static RectangleF GetCommandBounds(
        ViewportSceneCommand command)
    {
        var left = Math.Min(command.Start.X, command.End.X);
        var right = Math.Max(command.Start.X, command.End.X);
        var top = Math.Min(command.Start.Y, command.End.Y);
        var bottom = Math.Max(command.Start.Y, command.End.Y);

        return RectangleF.FromLTRB(
            left,
            top,
            right,
            bottom);
    }
}
