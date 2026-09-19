namespace Asun.UI.Viewports;

public static class ViewportRenderBudgetValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        ViewportRenderWorkPlan source,
        ViewportRenderWorkPlan budgeted,
        ViewportRenderBudget budget)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(budgeted);

        var errors = new List<string>();
        budget.Validate();

        if (budgeted.Generation != source.Generation)
            errors.Add("Budgeted plan generation must match the source plan.");

        if (budgeted.Items.Count > budget.MaxTotalWork)
            errors.Add("Budgeted item count exceeds MaxTotalWork.");

        if (budgeted.TileWorkCount > budget.MaxTileWork)
            errors.Add("Budgeted tile work exceeds MaxTileWork.");

        if (budgeted.RoiWorkCount > budget.MaxRoiWork)
            errors.Add("Budgeted ROI work exceeds MaxRoiWork.");

        var overlayWork = budgeted.Items.Count(item =>
            !item.IsInvalidation &&
            item.Kind is
                ViewportRenderWorkKind.Overlay or
                ViewportRenderWorkKind.Selection);

        if (overlayWork > budget.MaxOverlayWork)
            errors.Add("Budgeted overlay/selection work exceeds MaxOverlayWork.");

        var sourceSet = source.Items.ToHashSet();

        foreach (var item in budgeted.Items)
        {
            if (!sourceSet.Contains(item))
                errors.Add("Budgeted work contains an item not present in the source plan.");
        }

        return errors;
    }

    public static bool IsValid(
        ViewportRenderWorkPlan source,
        ViewportRenderWorkPlan budgeted,
        ViewportRenderBudget budget) =>
        Validate(source, budgeted, budget).Count == 0;
}
