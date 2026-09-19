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

        if (budget.MaxTotalWork > 0)
        {
            AddUpTo(
                selected,
                plan.Items.Where(item =>
                    !item.IsInvalidation &&
                    item.Kind == ViewportRenderWorkKind.FullSurface),
                1);
        }

        var remaining = Math.Max(
            0,
            budget.MaxTotalWork - selected.Count);

        AddUpTo(
            selected,
            plan.Items.Where(item => item.IsInvalidation),
            remaining);

        remaining = Math.Max(
            0,
            budget.MaxTotalWork - selected.Count);

        AddUpTo(
            selected,
            plan.Items.Where(item =>
                !item.IsInvalidation &&
                item.Kind == ViewportRenderWorkKind.Tile),
            Math.Min(budget.MaxTileWork, remaining));

        remaining = Math.Max(0, budget.MaxTotalWork - selected.Count);
        AddUpTo(
            selected,
            plan.Items.Where(item =>
                !item.IsInvalidation &&
                item.Kind == ViewportRenderWorkKind.Roi),
            Math.Min(budget.MaxRoiWork, remaining));

        remaining = Math.Max(0, budget.MaxTotalWork - selected.Count);
        AddUpTo(
            selected,
            plan.Items.Where(item =>
                !item.IsInvalidation &&
                item.Kind is ViewportRenderWorkKind.Overlay
                    or ViewportRenderWorkKind.Selection),
            Math.Min(budget.MaxOverlayWork, remaining));

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
