namespace Asun.UI.Viewports;

public enum ViewportSceneDiffKind
{
    Added,
    Removed,
    Changed,
    SelectionChanged,
    TransformChanged
}

public readonly record struct ViewportSceneDiff(
    ViewportSceneDiffKind Kind,
    Guid RoiId,
    ViewportSceneCommand? Previous,
    ViewportSceneCommand? Current);

/// <summary>
/// Computes stable scene differences so a renderer can redraw only affected
/// command regions rather than rebuilding the complete viewport surface.
/// </summary>
public static class ViewportSceneDiffRuntime
{
    public static IReadOnlyList<ViewportSceneDiff> Diff(
        ViewportSceneSnapshot previous,
        ViewportSceneSnapshot current)
    {
        ArgumentNullException.ThrowIfNull(previous);
        ArgumentNullException.ThrowIfNull(current);

        var result = new List<ViewportSceneDiff>();

        if (previous.Transform != current.Transform)
        {
            result.Add(
                new ViewportSceneDiff(
                    ViewportSceneDiffKind.TransformChanged,
                    Guid.Empty,
                    null,
                    null));
        }

        var previousByKey = previous.Commands
            .GroupBy(Key)
            .ToDictionary(group => group.Key, group => group.ToArray());

        var currentByKey = current.Commands
            .GroupBy(Key)
            .ToDictionary(group => group.Key, group => group.ToArray());

        foreach (var pair in previousByKey)
        {
            if (currentByKey.ContainsKey(pair.Key))
                continue;

            foreach (var command in pair.Value)
            {
                result.Add(
                    new ViewportSceneDiff(
                        ViewportSceneDiffKind.Removed,
                        command.RoiId,
                        command,
                        null));
            }
        }

        foreach (var pair in currentByKey)
        {
            if (!previousByKey.TryGetValue(pair.Key, out var oldCommands))
            {
                foreach (var command in pair.Value)
                {
                    result.Add(
                        new ViewportSceneDiff(
                            ViewportSceneDiffKind.Added,
                            command.RoiId,
                            null,
                            command));
                }

                continue;
            }

            var old = oldCommands[0];
            var currentCommand = pair.Value[0];

            if (old != currentCommand)
            {
                var kind =
                    old.Selected != currentCommand.Selected
                        ? ViewportSceneDiffKind.SelectionChanged
                        : ViewportSceneDiffKind.Changed;

                result.Add(
                    new ViewportSceneDiff(
                        kind,
                        currentCommand.RoiId,
                        old,
                        currentCommand));
            }
        }

        return result;
    }

    private static (Guid RoiId, ViewportSceneCommandKind Kind, RoiHandleKind Handle, int Index) Key(
        ViewportSceneCommand command) =>
        (
            command.RoiId,
            command.Kind,
            command.Handle,
            command.HandleIndex);
}
