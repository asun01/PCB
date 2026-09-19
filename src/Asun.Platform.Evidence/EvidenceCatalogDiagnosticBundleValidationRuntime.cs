namespace Asun.Platform.Evidence;

public static class EvidenceCatalogDiagnosticBundleValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        EvidenceCatalogSnapshot snapshot,
        EvidenceCatalogDiagnosticBundle bundle)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentNullException.ThrowIfNull(bundle);

        var errors=new List<string>();

        errors.AddRange(
            EvidenceCatalogSnapshotValidationRuntime.Validate(snapshot));

        errors.AddRange(
            EvidenceCatalogQueryBatchValidationRuntime.Validate(
                snapshot,
                bundle.QueryBatch));

        var referenceHandles=bundle.ReferenceResolution.FoundHandles
            .Concat(bundle.ReferenceResolution.MissingHandles)
            .Distinct()
            .OrderBy(handle=>handle.Value,StringComparer.Ordinal)
            .ToArray();
        var referenceSet=new EvidenceReferenceSet(referenceHandles);

        errors.AddRange(
            EvidenceReferenceResolutionValidationRuntime.Validate(
                snapshot,
                referenceSet,
                bundle.ReferenceResolution));

        errors.AddRange(
            EvidenceCatalogStatisticsValidationRuntime.Validate(
                snapshot,
                bundle.Statistics));

        var snapshotFingerprint=
            EvidenceCatalogSnapshotFingerprintRuntime.CreateFingerprint(snapshot);

        if(bundle.SnapshotFingerprint!=snapshotFingerprint)
            errors.Add("Diagnostic bundle snapshot fingerprint does not match the snapshot.");

        return errors;
    }

    public static bool IsValid(
        EvidenceCatalogSnapshot snapshot,
        EvidenceCatalogDiagnosticBundle bundle)=>
        Validate(snapshot,bundle).Count==0;
}
