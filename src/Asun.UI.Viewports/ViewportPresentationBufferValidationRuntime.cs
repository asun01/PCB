using System.Drawing;

namespace Asun.UI.Viewports;

public static class ViewportPresentationBufferValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        ViewportPresentationBufferSnapshot snapshot)
    {
        var errors = new List<string>();

        if (snapshot.LastPlannedUnits < 0 ||
            snapshot.LastRenderedUnits < 0)
        {
            errors.Add("Buffer unit counters must be non-negative.");
        }

        if (snapshot.LastRenderedUnits > snapshot.LastPlannedUnits)
            errors.Add("Buffer rendered units cannot exceed planned units.");

        if (snapshot.PresentedRegions is null)
            errors.Add("Buffer presented regions must not be null.");
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
                    errors.Add($"Buffer region {i + 1} is invalid.");
                }
            }
        }

        ValidateSlot(
            errors,
            snapshot.FirstState,
            snapshot.RenderingSlot == 0,
            snapshot.PresentedSlot == 0,
            "first");

        ValidateSlot(
            errors,
            snapshot.SecondState,
            snapshot.RenderingSlot == 1,
            snapshot.PresentedSlot == 1,
            "second");

        if (snapshot.RenderingSlot is int renderingSlot &&
            (renderingSlot < 0 || renderingSlot > 1))
        {
            errors.Add("Rendering slot index must be 0 or 1.");
        }

        if (snapshot.PresentedSlot is int presentedSlot &&
            (presentedSlot < 0 || presentedSlot > 1))
        {
            errors.Add("Presented slot index must be 0 or 1.");
        }

        if (snapshot.RenderingSlot is not null &&
            snapshot.PresentedSlot is not null &&
            snapshot.RenderingSlot == snapshot.PresentedSlot)
        {
            errors.Add("Rendering and presented slots cannot be identical.");
        }

        if (snapshot.RenderingSlot is null &&
            (snapshot.RenderingGeneration is not null ||
             snapshot.RenderingSequence is not null))
        {
            errors.Add("Rendering metadata requires a rendering slot.");
        }

        if (snapshot.PresentedSlot is null &&
            (snapshot.PresentedGeneration is not null ||
             snapshot.PresentedSequence is not null))
        {
            errors.Add("Presented metadata requires a presented slot.");
        }

        if (snapshot.RenderingSlot is not null &&
            (snapshot.RenderingGeneration is null ||
             snapshot.RenderingSequence is null))
        {
            errors.Add("Rendering slot requires generation and sequence metadata.");
        }

        if (snapshot.PresentedSlot is not null &&
            (snapshot.PresentedGeneration is null ||
             snapshot.PresentedSequence is null))
        {
            errors.Add("Presented slot requires generation and sequence metadata.");
        }

        return errors;
    }

    public static bool IsValid(ViewportPresentationBufferSnapshot snapshot) =>
        Validate(snapshot).Count == 0;

    private static void ValidateSlot(
        ICollection<string> errors,
        ViewportPresentationBufferState state,
        bool isRendering,
        bool isPresented,
        string name)
    {
        if (isRendering && state != ViewportPresentationBufferState.Rendering)
            errors.Add($"{name} slot index says rendering but its state disagrees.");

        if (isPresented && state != ViewportPresentationBufferState.Presented)
            errors.Add($"{name} slot index says presented but its state disagrees.");

        if (!isRendering &&
            !isPresented &&
            state is ViewportPresentationBufferState.Rendering or
                ViewportPresentationBufferState.Presented)
        {
            errors.Add($"{name} slot state requires an owning slot index.");
        }
    }
}
