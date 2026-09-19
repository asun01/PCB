namespace Asun.UI.Viewports;

public static class ViewportRenderPriorityValidationRuntime
{
    public static IReadOnlyList<string> Validate<TTile>(
        ViewportRenderWorkPlan source,
        ViewportRenderWorkPlan prioritized,
        ViewportCompositeFrame<TTile> frame)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(prioritized);
        ArgumentNullException.ThrowIfNull(frame);

        var errors = new List<string>();

        if (source.Generation != prioritized.Generation ||
            prioritized.Generation != frame.Generation)
        {
            errors.Add("Priority generation must match the source and frame.");
        }

        if (source.Items.Count != prioritized.Items.Count)
            errors.Add("Priority must preserve the total work-item count.");

        var sourceCounts = source.Items
            .GroupBy(item => item)
            .ToDictionary(group => group.Key, group => group.Count());

        foreach (var item in prioritized.Items)
        {
            if (!sourceCounts.TryGetValue(item, out var count) || count == 0)
            {
                errors.Add("Priority introduced a work item absent from the source plan.");
                continue;
            }

            sourceCounts[item] = count - 1;
        }

        if (sourceCounts.Values.Any(value => value != 0))
            errors.Add("Priority did not preserve every source work item.");

        var selectedIds = frame.Roi.Items
            .Where(item => item.IsSelected)
            .Select(item => item.Id)
            .ToHashSet();

        var priorityValues = prioritized.Items
            .Select(item => GetPriority(item, selectedIds))
            .ToArray();

        if (priorityValues.Zip(
                priorityValues.Skip(1),
                (left, right) => left <= right)
            .Any(valid => !valid))
        {
            errors.Add("Priority values must be non-decreasing.");
        }

        return errors;
    }

    public static bool IsValid<TTile>(
        ViewportRenderWorkPlan source,
        ViewportRenderWorkPlan prioritized,
        ViewportCompositeFrame<TTile> frame) =>
        Validate(source, prioritized, frame).Count == 0;

    private static int GetPriority(
        ViewportRenderWorkItem item,
        IReadOnlySet<Guid> selectedIds)
    {
        if (item.IsInvalidation)
            return -2;

        return item.Kind switch
        {
            ViewportRenderWorkKind.FullSurface => -1,
            ViewportRenderWorkKind.Tile => 0,
            ViewportRenderWorkKind.Roi when selectedIds.Contains(item.RoiId) => 2,
            ViewportRenderWorkKind.Roi => 3,
            ViewportRenderWorkKind.Selection => 4,
            ViewportRenderWorkKind.Overlay => 5,
            _ => 6
        };
    }
}
