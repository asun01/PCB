using System.Drawing;

namespace Asun.UI.Viewports;

public static class ViewportRenderCommandStreamValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        ViewportRenderCommandStream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        var errors = new List<string>();

        if (stream.Generation < 0)
            errors.Add("Command stream generation must be non-negative.");

        for (var index = 0; index < stream.Commands.Count; index++)
        {
            var command = stream.Commands[index];
            var expectedSequence = index + 1;

            if (command.Sequence != expectedSequence)
                errors.Add("Command sequence must start at one and increase strictly.");

            if (!IsFinite(command.Bounds) ||
                command.Bounds.Width < 0 ||
                command.Bounds.Height < 0)
            {
                errors.Add($"Command {command.Sequence} bounds must be finite and non-negative.");
            }

            if (command.WorkItem.Generation != stream.Generation)
                errors.Add($"Command {command.Sequence} work generation must match the stream.");

            if (!MatchesKind(command))
                errors.Add($"Command {command.Sequence} kind does not match its work item.");
        }

        for (var index = 0; index < stream.Regions.Count; index++)
        {
            if (!IsFinite(stream.Regions[index]) ||
                stream.Regions[index].Width < 0 ||
                stream.Regions[index].Height < 0)
            {
                errors.Add($"Region {index + 1} must be finite and non-negative.");
            }
        }

        return errors;
    }

    public static bool IsValid(ViewportRenderCommandStream stream) =>
        Validate(stream).Count == 0;

    private static bool MatchesKind(ViewportRenderCommand command) =>
        command.Kind switch
        {
            ViewportRenderCommandKind.DrawTile =>
                command.WorkItem.Kind == ViewportRenderWorkKind.Tile &&
                !command.WorkItem.IsInvalidation,
            ViewportRenderCommandKind.DrawRoi =>
                command.WorkItem.Kind == ViewportRenderWorkKind.Roi &&
                !command.WorkItem.IsInvalidation,
            ViewportRenderCommandKind.ClearInvalidatedRegion =>
                command.WorkItem.Kind == ViewportRenderWorkKind.Roi &&
                command.WorkItem.IsInvalidation,
            ViewportRenderCommandKind.DrawOverlay =>
                command.WorkItem.Kind == ViewportRenderWorkKind.Overlay &&
                !command.WorkItem.IsInvalidation,
            ViewportRenderCommandKind.FullSurfaceClear =>
                command.WorkItem.Kind == ViewportRenderWorkKind.FullSurface &&
                !command.WorkItem.IsInvalidation,
            _ => false
        };

    private static bool IsFinite(RectangleF rectangle) =>
        float.IsFinite(rectangle.X) &&
        float.IsFinite(rectangle.Y) &&
        float.IsFinite(rectangle.Width) &&
        float.IsFinite(rectangle.Height);
}
