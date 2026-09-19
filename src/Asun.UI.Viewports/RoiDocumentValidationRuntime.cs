namespace Asun.UI.Viewports;

public static class RoiDocumentValidationRuntime
{
    public static IReadOnlyList<string> ValidateCancelEvent(
        RoiDocumentEvent result,
        Guid expectedRoiId)
    {
        var errors = new List<string>();

        if (result.EditorEvent.Kind != RoiEditorEventKind.Cancelled)
            errors.Add("Cancel result must report the cancelled editor event.");

        if (result.RoiId != expectedRoiId)
            errors.Add("Cancel result lost the identity of the ROI being cancelled.");

        if (result.DocumentChanged)
            errors.Add("Cancel result must not report a committed document change.");

        return errors;
    }

    public static IReadOnlyList<string> ValidateSnapshot(
        RoiDocumentSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        var errors = new List<string>();

        if (snapshot.Items.Select(item => item.Id).Distinct().Count() !=
            snapshot.Items.Count)
        {
            errors.Add("Document snapshot contains duplicate ROI ids.");
        }

        if (snapshot.Items.Any(item => item.Id == Guid.Empty))
            errors.Add("Document snapshot contains an empty ROI id.");

        if (snapshot.SelectedId is Guid selected &&
            snapshot.Items.All(item => item.Id != selected))
        {
            errors.Add("Document selection points to a missing ROI.");
        }

        return errors;
    }

    public static bool IsValidCancelEvent(
        RoiDocumentEvent result,
        Guid expectedRoiId) =>
        ValidateCancelEvent(result, expectedRoiId).Count == 0;
}
