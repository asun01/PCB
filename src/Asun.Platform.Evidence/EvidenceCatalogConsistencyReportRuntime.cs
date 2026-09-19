namespace Asun.Platform.Evidence;

public static class EvidenceCatalogConsistencyReportRuntime
{
    public static EvidenceCatalogConsistencyReport Create(
        EvidenceCatalogSnapshot snapshot,
        EvidenceReferenceSet referenceSet,
        EvidenceReferenceResolution resolution)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentNullException.ThrowIfNull(referenceSet);
        ArgumentNullException.ThrowIfNull(resolution);

        if(!EvidenceCatalogSnapshotValidationRuntime.IsValid(snapshot))
            throw new ArgumentException(
                "Evidence catalog snapshot is invalid.",
                nameof(snapshot));

        if(!EvidenceReferenceResolutionValidationRuntime.IsValid(
            snapshot,
            referenceSet,
            resolution))
        {
            throw new ArgumentException(
                "Evidence reference resolution is invalid.",
                nameof(resolution));
        }

        var statistics=EvidenceCatalogStatisticsRuntime.Create(snapshot);

        return new EvidenceCatalogConsistencyReport(
            EvidenceCatalogSnapshotFingerprintRuntime.CreateFingerprint(snapshot),
            EvidenceCatalogStatisticsFingerprintRuntime.CreateFingerprint(statistics),
            EvidenceReferenceResolutionFingerprintRuntime.CreateFingerprint(resolution),
            snapshot.Count,
            referenceSet.Handles.Count,
            resolution.MissingHandles.Count);
    }
}
