namespace Asun.Platform.Evidence;

public static class EvidenceCatalogSnapshotIntegrityReportRuntime
{
    public static EvidenceCatalogSnapshotIntegrityReport Create(
        EvidenceCatalogSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        if(!EvidenceCatalogSnapshotValidationRuntime.IsValid(snapshot))
            throw new ArgumentException(
                "Evidence catalog snapshot is invalid.",
                nameof(snapshot));

        return new EvidenceCatalogSnapshotIntegrityReport(
            snapshot.Count,
            EvidenceCatalogSnapshotFingerprintRuntime.CreateFingerprint(snapshot));
    }
}
