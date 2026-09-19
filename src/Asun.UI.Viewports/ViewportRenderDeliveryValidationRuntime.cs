namespace Asun.UI.Viewports;

public static class ViewportRenderDeliveryValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        ViewportRenderDeliveryResult result)
    {
        var errors = new List<string>();
        var deferredCount = result.DeferredWorkItems?.Count ?? -1;

        if (result.Generation < 0)
            errors.Add("Delivery generation must be non-negative.");

        if (result.RenderedUnits < 0 ||
            result.PlannedUnits < 0 ||
            result.DeferredUnits < 0)
        {
            errors.Add("Delivery unit counters must be non-negative.");
        }

        if (result.RenderedUnits > result.PlannedUnits)
            errors.Add("Rendered units cannot exceed planned units.");

        if (result.DeferredWorkItems is null)
            errors.Add("Deferred work items must not be null.");

        if (result.Deferred &&
            deferredCount == 0)
        {
            errors.Add("Deferred delivery must retain deferred work items.");
        }

        if (!result.Deferred &&
            deferredCount > 0)
        {
            errors.Add("Non-deferred delivery cannot retain deferred work items.");
        }

        if (result.Deferred &&
            result.DeferredUnits != deferredCount)
        {
            errors.Add("Deferred unit count must match deferred work item count.");
        }

        if (result.Succeeded && (result.Cancelled || result.Deferred))
            errors.Add("A successful delivery cannot be cancelled or deferred.");

        if (result.Cancelled && result.Deferred)
            errors.Add("A delivery cannot be cancelled and deferred.");

        if (result.Succeeded && result.Error is not null)
            errors.Add("A successful delivery cannot contain an error.");

        if (!result.Succeeded &&
            !result.Cancelled &&
            !result.Deferred &&
            result.Error is null)
        {
            errors.Add("A failed delivery must contain an error.");
        }

        if (result.RegionCount < 0)
            errors.Add("Delivery region count must be non-negative.");

        return errors;
    }

    public static bool IsValid(ViewportRenderDeliveryResult result) =>
        Validate(result).Count == 0;
}
