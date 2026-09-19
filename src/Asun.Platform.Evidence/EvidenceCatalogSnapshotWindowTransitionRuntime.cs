namespace Asun.Platform.Evidence;

public static class EvidenceCatalogSnapshotWindowTransitionRuntime
{
    public static EvidenceCatalogSnapshotWindowTransition Create(
        EvidenceCatalogSnapshotWindow previous,
        EvidenceCatalogSnapshotWindow current)
    {
        ArgumentNullException.ThrowIfNull(previous);
        ArgumentNullException.ThrowIfNull(current);

        if(!EvidenceCatalogSnapshotWindowValidationRuntime.IsValid(previous) ||
           !EvidenceCatalogSnapshotWindowValidationRuntime.IsValid(current))
        {
            throw new ArgumentException(
                "Both evidence catalog snapshot windows must be valid.");
        }

        return new EvidenceCatalogSnapshotWindowTransition(
            EvidenceCatalogSnapshotWindowIntegritySummaryRuntime.Create(previous).WindowFingerprint,
            EvidenceCatalogSnapshotWindowIntegritySummaryRuntime.Create(current).WindowFingerprint,
            EvidenceCatalogSnapshotWindowDiffRuntime.Diff(previous,current));
    }
}
