namespace Asun.UI.Viewports;

public static class ViewportRenderSurfaceValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        ViewportRenderSurfaceSnapshot snapshot)
    {
        var errors = new List<string>();

        if (snapshot.PresentationSequence < 0)
            errors.Add("Surface presentation sequence must be non-negative.");

        if (snapshot.LastPlannedUnits < 0 ||
            snapshot.LastRenderedUnits < 0)
        {
            errors.Add("Surface unit counters must be non-negative.");
        }

        if (snapshot.LastRenderedUnits > snapshot.LastPlannedUnits)
            errors.Add("Surface rendered units cannot exceed planned units.");

        if (snapshot.PresentedRegions is null)
            errors.Add("Surface presented regions must not be null.");
        else
        {
            for (var i = 0; i < snapshot.PresentedRegions.Count; i++)
            {
                var region = snapshot.PresentedRegions[i];
                if (!float.IsFinite(region.X) ||
                    !float.IsFinite(region.Y) ||
                    !float.IsFinite(region.Width) ||
                    !float.IsFinite(region.Height) ||
                    region.Width < 0 ||
                    region.Height < 0)
                {
                    errors.Add($"Surface region {i + 1} is invalid.");
                }
            }
        }

        if (snapshot.State == ViewportRenderSurfaceState.Rendering)
        {
            if (snapshot.RenderingGeneration is null ||
                snapshot.RenderingSequence is null)
                errors.Add("Rendering surface must expose generation and sequence.");
        }
        else if (snapshot.RenderingGeneration is not null ||
                 snapshot.RenderingSequence is not null)
        {
            errors.Add("Non-rendering surface cannot retain a rendering transaction.");
        }

        if (snapshot.State == ViewportRenderSurfaceState.Presented &&
            snapshot.PresentedGeneration is null)
        {
            errors.Add("Presented surface must expose a presented generation.");
        }

        if (snapshot.State == ViewportRenderSurfaceState.Discarded &&
            snapshot.DiscardedGeneration is null)
        {
            errors.Add("Discarded surface must expose a discarded generation.");
        }

        if (snapshot.PresentedGeneration is not null &&
            snapshot.PresentationSequence == 0)
        {
            errors.Add("Presented generation requires a positive presentation sequence.");
        }

        return errors;
    }

    public static bool IsValid(ViewportRenderSurfaceSnapshot snapshot) =>
        Validate(snapshot).Count == 0;
}
