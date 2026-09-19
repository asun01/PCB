namespace Asun.Platform.Evidence;

public static class EvidenceCatalogSnapshotWindowTransitionValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        EvidenceCatalogSnapshotWindow previous,
        EvidenceCatalogSnapshotWindow current,
        EvidenceCatalogSnapshotWindowTransition transition)
    {
        ArgumentNullException.ThrowIfNull(previous);
        ArgumentNullException.ThrowIfNull(current);
        ArgumentNullException.ThrowIfNull(transition);

        var errors=new List<string>();

        if(!EvidenceCatalogSnapshotWindowIntegritySummaryValidationRuntime.IsValid(
            previous,
            new EvidenceCatalogSnapshotWindowIntegritySummary(
                previous.Count,
                previous.Count==0 ? null : previous.Entries[0].Sequence,
                previous.Count==0 ? null : previous.Entries[^1].Sequence,
                transition.PreviousWindowFingerprint)))
        {
            errors.Add("Previous window fingerprint is invalid.");
        }

        if(!EvidenceCatalogSnapshotWindowIntegritySummaryValidationRuntime.IsValid(
            current,
            new EvidenceCatalogSnapshotWindowIntegritySummary(
                current.Count,
                current.Count==0 ? null : current.Entries[0].Sequence,
                current.Count==0 ? null : current.Entries[^1].Sequence,
                transition.CurrentWindowFingerprint)))
        {
            errors.Add("Current window fingerprint is invalid.");
        }

        var expected=EvidenceCatalogSnapshotWindowDiffRuntime.Diff(previous,current);
        if(!EvidenceCatalogSnapshotWindowDiffValidationRuntime.IsValid(transition.Diff))
            errors.AddRange(EvidenceCatalogSnapshotWindowDiffValidationRuntime.Validate(transition.Diff));

        if(!expected.Equals(transition.Diff))
            errors.Add("Window transition diff does not match the supplied previous/current windows.");

        return errors;
    }

    public static bool IsValid(
        EvidenceCatalogSnapshotWindow previous,
        EvidenceCatalogSnapshotWindow current,
        EvidenceCatalogSnapshotWindowTransition transition)=>
        Validate(previous,current,transition).Count==0;
}
