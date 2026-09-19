namespace Asun.UI.Viewports;

public static class RoiEditorValidationRuntime
{
    public static bool IsValid(RoiEditorSnapshot snapshot)
    {
        if (!Enum.IsDefined(snapshot.Mode))
            return false;

        if (snapshot.Geometry is null &&
            snapshot.CommittedGeometry is not null)
            return false;

        if (snapshot.Interaction.IsActive)
        {
            if (snapshot.Interaction.Kind == RoiInteractionKind.Idle ||
                snapshot.Interaction.StartGeometry is null &&
                snapshot.Interaction.Kind != RoiInteractionKind.Creating)
                return false;
        }

        if (!snapshot.Interaction.IsActive &&
            snapshot.Interaction.Kind != RoiInteractionKind.Idle)
            return false;

        return true;
    }

    public static bool IsCancelledToCommitted(
        RoiEditorSnapshot before,
        RoiEditorSnapshot after) =>
        !after.Interaction.IsActive &&
        Equals(after.Geometry, before.CommittedGeometry) &&
        Equals(after.CommittedGeometry, before.CommittedGeometry);
}
