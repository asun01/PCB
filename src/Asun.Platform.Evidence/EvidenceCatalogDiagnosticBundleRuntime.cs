namespace Asun.Platform.Evidence;

public static class EvidenceCatalogDiagnosticBundleRuntime
{
    public static EvidenceCatalogDiagnosticBundle Create(
        EvidenceCatalogSnapshot snapshot,
        EvidenceCatalogQueryBatch queryBatch,
        EvidenceReferenceResolution referenceResolution)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentNullException.ThrowIfNull(queryBatch);
        ArgumentNullException.ThrowIfNull(referenceResolution);

        if(!EvidenceCatalogQueryBatchValidationRuntime.IsValid(snapshot,queryBatch))
            throw new ArgumentException(
                "Evidence catalog query batch is invalid.",
                nameof(queryBatch));

        var referenceHandles=referenceResolution.FoundHandles
            .Concat(referenceResolution.MissingHandles)
            .Distinct()
            .OrderBy(handle=>handle.Value,StringComparer.Ordinal)
            .ToArray();
        var referenceSet=new EvidenceReferenceSet(referenceHandles);

        if(!EvidenceReferenceResolutionValidationRuntime.IsValid(
            snapshot,
            referenceSet,
            referenceResolution))
        {
            throw new ArgumentException(
                "Evidence reference resolution is invalid.",
                nameof(referenceResolution));
        }

        var statistics=EvidenceCatalogStatisticsRuntime.Create(snapshot);

        return new EvidenceCatalogDiagnosticBundle(
            EvidenceCatalogSnapshotFingerprintRuntime.CreateFingerprint(snapshot),
            statistics,
            queryBatch,
            referenceResolution);
    }
}
