namespace Asun.UI.Viewports;

public static class ViewportRenderReuseValidationRuntime
{
    public static IReadOnlyList<string> Validate<TTile>(
        ViewportRenderReuseRuntime<TTile> reuse,
        long generation)
    {
        ArgumentNullException.ThrowIfNull(reuse);

        var errors = new List<string>();

        if (reuse.TryReuse(generation, out var frame))
        {
            if (frame.HasDeferredWork)
                errors.Add("Reuse cannot expose a deferred frame.");

            if (frame.Composite.Generation != generation)
                errors.Add("Reused frame generation must match the requested generation.");

            if (frame.WorkPlan.Generation != generation)
                errors.Add("Reused work-plan generation must match the requested generation.");
        }

        if (reuse.LatestGeneration is long latest &&
            latest > generation)
        {
            errors.Add("Reuse cache cannot expose a generation newer than the requested generation.");
        }

        return errors;
    }

    public static bool IsValid<TTile>(
        ViewportRenderReuseRuntime<TTile> reuse,
        long generation) =>
        Validate(reuse, generation).Count == 0;
}
