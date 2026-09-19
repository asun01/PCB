namespace Asun.UI.Viewports;

public readonly record struct ViewportRenderBudget(
    int MaxTileWork,
    int MaxRoiWork,
    int MaxOverlayWork,
    int MaxTotalWork)
{
    public static ViewportRenderBudget Default =>
        new(64, 256, 8, 320);

    public ViewportRenderBudget Validate()
    {
        if (MaxTileWork < 0 ||
            MaxRoiWork < 0 ||
            MaxOverlayWork < 0 ||
            MaxTotalWork < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(ViewportRenderBudget));
        }

        return this;
    }
}

public static class ViewportRenderBudgetRuntime
{
    public static ViewportRenderWorkPlan Apply(
        ViewportRenderWorkPlan plan,
        ViewportRenderBudget budget)
    {
        ArgumentNullException.ThrowIfNull(plan);
        budget.Validate();

        var selected = new List<ViewportRenderWorkItem>();

        AddUpTo(
            selected,
            plan.Items.Where(item => item.Kind == ViewportRenderWorkKind.Tile),
            budget.MaxTileWork);

        AddUpTo(
            selected,
            plan.Items.Where(item => item.Kind == ViewportRenderWorkKind.Roi),
            budget.MaxRoiWork);

        AddUpTo(
            selected,
            plan.Items.Where(item =>
                item.Kind is ViewportRenderWorkKind.Overlay
                    or ViewportRenderWorkKind.Selection
                    or ViewportRenderWorkKind.FullSurface),
            budget.MaxOverlayWork);

        if (selected.Count > budget.MaxTotalWork)
            selected = selected
                .Take(budget.MaxTotalWork)
                .ToList();

        return new ViewportRenderWorkPlan(
            selected,
            plan.ConsumedFlags,
            plan.Generation);
    }

    private static void AddUpTo(
        List<ViewportRenderWorkItem> destination,
        IEnumerable<ViewportRenderWorkItem> source,
        int limit)
    {
        if (limit <= 0)
            return;

        destination.AddRange(source.Take(limit));
    }
}
