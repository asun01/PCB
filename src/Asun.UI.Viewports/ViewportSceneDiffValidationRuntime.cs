namespace Asun.UI.Viewports;

public static class ViewportSceneDiffValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        ViewportSceneSnapshot previous,
        ViewportSceneSnapshot current,
        IReadOnlyList<ViewportSceneDiff> diffs)
    {
        ArgumentNullException.ThrowIfNull(previous);
        ArgumentNullException.ThrowIfNull(current);
        ArgumentNullException.ThrowIfNull(diffs);

        var errors = new List<string>();

        if ((previous.Transform != current.Transform) !=
            diffs.Any(diff => diff.Kind == ViewportSceneDiffKind.TransformChanged))
        {
            errors.Add("Transform diff coverage does not match transform change state.");
        }

        var previousByKey = previous.Commands
            .GroupBy(Key)
            .ToDictionary(group => group.Key, group => group.Single());

        var currentByKey = current.Commands
            .GroupBy(Key)
            .ToDictionary(group => group.Key, group => group.Single());

        foreach (var pair in previousByKey)
        {
            if (currentByKey.ContainsKey(pair.Key))
                continue;

            if (!diffs.Any(diff =>
                diff.Kind == ViewportSceneDiffKind.Removed &&
                diff.Previous == pair.Value &&
                diff.Current is null))
            {
                errors.Add("A removed scene command is missing from the diff.");
            }
        }

        foreach (var pair in currentByKey)
        {
            if (!previousByKey.TryGetValue(pair.Key, out var oldCommand))
            {
                if (!diffs.Any(diff =>
                    diff.Kind == ViewportSceneDiffKind.Added &&
                    diff.Previous is null &&
                    diff.Current == pair.Value))
                {
                    errors.Add("An added scene command is missing from the diff.");
                }

                continue;
            }

            if (oldCommand == pair.Value)
                continue;

            var expectedKind =
                oldCommand.Selected != pair.Value.Selected
                    ? ViewportSceneDiffKind.SelectionChanged
                    : ViewportSceneDiffKind.Changed;

            if (!diffs.Any(diff =>
                diff.Kind == expectedKind &&
                diff.Previous == oldCommand &&
                diff.Current == pair.Value))
            {
                errors.Add("A changed scene command is missing from the diff.");
            }
        }

        return errors;
    }

    public static bool IsValid(
        ViewportSceneSnapshot previous,
        ViewportSceneSnapshot current,
        IReadOnlyList<ViewportSceneDiff> diffs) =>
        Validate(previous, current, diffs).Count == 0;

    private static (Guid RoiId, ViewportSceneCommandKind Kind, RoiHandleKind Handle, int Index) Key(
        ViewportSceneCommand command) =>
        (
            command.RoiId,
            command.Kind,
            command.Handle,
            command.HandleIndex);
}
