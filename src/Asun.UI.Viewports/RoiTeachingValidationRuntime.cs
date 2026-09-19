namespace Asun.UI.Viewports;

public static class RoiTeachingValidationRuntime
{
    public static bool IsValid(RoiTeachingSnapshot snapshot) =>
        snapshot.CurrentStepIndex >= 0 &&
        (snapshot.CurrentStepIndex < 1_000_000 ||
         !snapshot.IsActive);

    public static bool HasCurrentStepWhenActive(
        RoiTeachingSnapshot snapshot,
        IReadOnlyList<RoiTeachingStep> steps)
    {
        ArgumentNullException.ThrowIfNull(steps);

        return !snapshot.IsActive ||
            snapshot.CurrentStepIndex < steps.Count &&
            snapshot.CurrentStep is not null;
    }

    public static bool HasUniqueIds(
        IReadOnlyList<RoiTeachingStep> steps)
    {
        ArgumentNullException.ThrowIfNull(steps);

        return steps.Select(step => step.Id)
            .Distinct(StringComparer.Ordinal)
            .Count() == steps.Count;
    }
}
